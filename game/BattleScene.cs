using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using ScopeCreep.Core;
using ScopeCreep.Core.enums;
using ScopeCreep.Core.model;
using ScopeCreep.Core.seams;

namespace ScopeCreep.Game;



public partial class BattleScene : Control, I_DefenseInput
{
	[ExportGroup("Top bar")]
	[Export] private Label round_label = null!;
	[Export] private Label phase_label = null!;

	[ExportGroup("Battlefield")]
	[Export] private ColorRect   boss_sprite   = null!;
	[Export] private ProgressBar boss_hp_bar   = null!;
	[Export] private Label       boss_hp_label = null!;
	[Export] private ColorRect   coder_sprite    = null!;
	[Export] private ColorRect   designer_sprite = null!;

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
	[Export] private Label   defense_prompt = null!;

	private Battle      battle      = null!;
	private HeroPanel[] hero_panels = [];

	private (Combatant.Hero user, Ability.Support ability)? pending_support;
	private bool is_boss_acting;

	private bool is_targeting => pending_support is not null;

	/** Defense input state — set during the boss attack window. */
	private TaskCompletionSource<DefenseResult>? defense_tcs;
	private Combatant.Hero? defense_target;

	/** Duration in seconds the player has to press a defense key. */
	private const float DEFENSE_WINDOW = 1.5f;


	public override void _Ready()
	{
		var state = new BattleState();
		state.heroes.Add(Combatant.Hero.coder());
		state.heroes.Add(Combatant.Hero.designer());
		battle = new Battle(state);

		// Wire the scene itself as the defense input resolver
		battle.defense_input = this;

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
		battle.dodged        += hero => writeLog($"{hero.name} dodged the attack!");
		battle.parried       += hero => writeLog($"{hero.name} parried the attack back!");

		defense_prompt.Visible = false;

		restart_button.Pressed += () => GetTree().ReloadCurrentScene();

		refresh();
	}


	// ─── Input handling ──────────────────────────────────────────────────

	public override void _UnhandledInput(InputEvent input_event)
	{
		if (is_targeting && input_event.IsActionPressed("ui_cancel"))
		{
			pending_support = null;
			refresh();
		}

		// Defense key handling during the boss attack window
		if (defense_tcs is not null && defense_target is not null && input_event is InputEventKey key_event && key_event.Pressed && !key_event.Echo)
		{
			char pressed = char.ToLower((char)key_event.Unicode);

			if (pressed == defense_target.parry_key)
			{
				defense_tcs.TrySetResult(DefenseResult.PARRY);
			}
			else if (pressed == defense_target.dodge_key)
			{
				defense_tcs.TrySetResult(DefenseResult.DODGE);
			}
		}
	}


	// ─── I_DefenseInput implementation ───────────────────────────────────

	public async Task<DefenseResult> resolve(BossAttack attack, Combatant.Hero target)
	{
		defense_target = target;
		defense_tcs = new TaskCompletionSource<DefenseResult>();

		// Show the telegraph: who is being targeted and which keys to press
		defense_prompt.Visible = true;
		defense_prompt.Text =
			$"» {attack.name} → {target.name}! «\n" +
			$"[{char.ToUpper(target.parry_key)}] Parry   [{char.ToUpper(target.dodge_key)}] Dodge";

		// Highlight the targeted hero sprite
		var sprite = getSpriteForHero(target);
		var original_color = sprite.Color;
		sprite.Color = new Color(1f, 0.3f, 0.3f);   // flash red

		// Start the timer — if it expires the attack lands
		var timer = GetTree().CreateTimer(DEFENSE_WINDOW);
		timer.Timeout += () => defense_tcs?.TrySetResult(DefenseResult.HIT);

		var result = await defense_tcs.Task;

		// Clean up
		defense_tcs = null;
		defense_target = null;
		defense_prompt.Visible = false;
		sprite.Color = original_color;

		// Play the appropriate animation
		await playDefenseAnimation(result, target);

		return result;
	}


	// ─── Defense animations ──────────────────────────────────────────────

	private async Task playDefenseAnimation(DefenseResult result, Combatant.Hero hero)
	{
		var sprite = getSpriteForHero(hero);

		switch (result)
		{
			case DefenseResult.DODGE:
				await animateDodge(sprite);
				break;

			case DefenseResult.PARRY:
				await animateParry(sprite);
				break;

			case DefenseResult.HIT:
				await animateHit(sprite);
				break;
		}
	}


	/**
	 *  Dodge: slide the hero sideways and back.
	 */
	private async Task animateDodge(ColorRect sprite)
	{
		var original_pos = sprite.Position;
		var dodge_offset = new Vector2(80, 0);

		var tween = CreateTween();
		tween.TweenProperty(sprite, "position", original_pos + dodge_offset, 0.15)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(sprite, "position", original_pos, 0.25)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);

		await ToSignal(tween, Tween.SignalName.Finished);
	}


	/**
	 *  Parry: flash white and lunge toward the boss briefly.
	 */
	private async Task animateParry(ColorRect sprite)
	{
		var original_pos = sprite.Position;
		var original_color = sprite.Color;
		var lunge_offset = new Vector2(40, 0);

		var tween = CreateTween();
		// Flash white and lunge forward (toward boss = rightward)
		tween.TweenProperty(sprite, "color", Colors.White, 0.08);
		tween.TweenProperty(sprite, "position", original_pos + lunge_offset, 0.1)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		// Return
		tween.TweenProperty(sprite, "position", original_pos, 0.2)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);
		tween.TweenProperty(sprite, "color", original_color, 0.15);

		await ToSignal(tween, Tween.SignalName.Finished);
	}


	/**
	 *  Hit: quick red flash.
	 */
	private async Task animateHit(ColorRect sprite)
	{
		var original_color = sprite.Color;

		var tween = CreateTween();
		tween.TweenProperty(sprite, "color", new Color(1f, 0.2f, 0.2f), 0.08);
		tween.TweenProperty(sprite, "color", original_color, 0.25);

		await ToSignal(tween, Tween.SignalName.Finished);
	}


	private ColorRect getSpriteForHero(Combatant.Hero hero)
	{
		// Match by name — coder is heroes[0], designer is heroes[1]
		return hero == battle.state.heroes[0] ? coder_sprite : designer_sprite;
	}


	// ─── UI refresh ──────────────────────────────────────────────────────

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


	// ─── Ability usage ───────────────────────────────────────────────────

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

		// short pause so the boss turn reads as a turn
		await ToSignal(GetTree().CreateTimer(0.6), SceneTreeTimer.SignalName.Timeout);
		await battle.runBossTurn();

		is_boss_acting = false;
		refresh();
	}


	private void writeLog(string line) => log.AppendText(line + "\n");
}
