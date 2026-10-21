using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using ScopeCreep.Core;
using ScopeCreep.Core.enums;
using ScopeCreep.Core.model;

namespace ScopeCreep.Game;



public partial class BattleScene : Control
{
    [ExportGroup("Top bar")]
    [Export] private Label round_label = null!;
    [Export] private Label phase_label = null!;

    [ExportGroup("Battlefield")]
    [Export] private ColorRect   boss_sprite   = null!;
    [Export] private ProgressBar boss_hp_bar   = null!;
    [Export] private Label       boss_hp_label = null!;

    [ExportGroup("Bottom panel")]
    [Export] private HeroPanel     coder_panel    = null!;
    [Export] private HeroPanel     designer_panel = null!;
    [Export] private Label         sp_label       = null!;
    [Export] private ProgressBar   sp_bar         = null!;
    [Export] private RichTextLabel log            = null!;

    [ExportGroup("Overlays")]
    [Export] private Label   target_hint    = null!;
    [Export] private Control end_screen     = null!;
    [Export] private Label   result_label   = null!;
    [Export] private Button  restart_button = null!;

    private Battle      battle      = null!;
    private HeroPanel[] hero_panels = [];

    private (Combatant.Hero user, Ability.Support ability)? pending_support;
    private bool is_boss_acting;

    private bool is_targeting => pending_support is not null;


    public override void _Ready()
    {
        var state = new BattleState();
        state.heroes.Add(Combatant.Hero.coder());
        state.heroes.Add(Combatant.Hero.designer());
        battle = new Battle(state);

        hero_panels = [coder_panel, designer_panel];
        for (int i = 0; i < hero_panels.Length; i++)
        {
            hero_panels[i].setup(state.heroes[i]);
            hero_panels[i].ability_pressed += onAbilityPressed;
            hero_panels[i].target_pressed  += onTargetPressed;
        }

        boss_hp_bar.MaxValue = state.boss.max_hp;
        sp_bar.MaxValue      = BattleState.SP_CAP;

        battle.state_changed += refresh;
        battle.damaged       += (who, amount) => writeLog($"{who.name} takes {amount} damage");
        battle.missed        += who => writeLog($"Missed {who.name}!");
        battle.phase_changed += phase => writeLog($"The Ramen Bowl enters {phase}!");

        restart_button.Pressed += () => GetTree().ReloadCurrentScene();

        refresh();
    }


    public override void _UnhandledInput(InputEvent input_event)
    {
        if (is_targeting && input_event.IsActionPressed("ui_cancel"))
        {
            pending_support = null;
            refresh();
        }
    }


    private void refresh()
    {
        var state = battle.state;
        var boss  = state.boss;

        round_label.Text   = $"Round {state.round}";
        phase_label.Text   = $"{boss.phase} · weak to {boss.weakness}";
        boss_hp_bar.Value  = boss.hp;
        boss_hp_label.Text = $"{boss.hp}/{boss.max_hp}";
        boss_sprite.Color  = boss.phase == BossPhase.PHASE_1 ? new Color("#d9a441") : new Color("#c0392b");

        sp_bar.Value  = state.spirit_points;
        sp_label.Text = $"Spirit Points {state.spirit_points}/{BattleState.SP_CAP}";

        foreach (var panel in hero_panels)
        {
            panel.refresh(battle, is_targeting, is_boss_acting);
        }

        target_hint.Visible = is_targeting;
        if (pending_support is { } pending)
        {
            target_hint.Text = $"{pending.ability.name}: choose an ally (Esc to cancel)";
        }

        end_screen.Visible = state.status != BattleStatus.IN_PROGRESS;
        result_label.Text  = state.status == BattleStatus.WON
            ? "SHIPPED IT!"
            : "THE SCOPE WON";
    }


    private async void onAbilityPressed(Combatant.Hero hero, Ability ability)
    {
        switch (ability)
        {
            case Ability.Offensive offensive:
                writeLog($"{hero.name} uses {ability.name}");
                battle.useOffensiveAbility(hero, offensive);
                break;
            case Ability.Support support:
                pending_support = (hero, support);   // wait for a Target click
                refresh();
                return;
            case Ability.Self self:
                writeLog($"{hero.name} uses {ability.name}");
                battle.useSelfAbility(hero, self);
                break;
            default:
                throw new UnreachableException();
        }

        await runBossTurnIfReady();
    }


    private async void onTargetPressed(Combatant.Hero target)
    {
        if (pending_support is not { } pending) return;

        pending_support = null;
        writeLog($"{pending.user.name} uses {pending.ability.name} on {target.name}");
        battle.useSupportAbility(pending.user, pending.ability, target);

        await runBossTurnIfReady();
    }


    private async Task runBossTurnIfReady()
    {
        if (!battle.is_hero_phase_over || battle.state.status != BattleStatus.IN_PROGRESS) return;

        is_boss_acting = true;
        refresh();
        writeLog("— The Ramen Bowl attacks —");

        // short pause so the boss turn reads as a turn; replaced by animations later
        await ToSignal(GetTree().CreateTimer(0.6), SceneTreeTimer.SignalName.Timeout);
        await battle.runBossTurn();

        is_boss_acting = false;
        refresh();
    }


    private void writeLog(string line) => log.AppendText(line + "\n");
}