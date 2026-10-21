using System.Diagnostics;
using Godot;
using ScopeCreep.Core;
using ScopeCreep.Core.model;

namespace ScopeCreep.Game;



public partial class BattleScene : Control
{
    private Battle battle = null!;
    [Export] private Label SP_label = null!;
    [Export] private Label boss_HP = null!;
    [Export] private HBoxContainer buttons_container = null!;


    public override void _Ready()
    {
        var battle_state = new BattleState();
        battle_state.heroes.Add(Combatant.Hero.coder());
        battle_state.heroes.Add(Combatant.Hero.designer());
        
        battle = new Battle(battle_state);
        battle.state_changed += Refresh;
        battle.damaged += (who, damage) => GD.Print($"{who.name} took {damage}");

        Refresh();
    }


    private void Refresh()
    {
        SP_label.Text = $"SP {battle.state.spirit_points}/{BattleState.SP_CAP}";
        boss_HP.Text = $"Boss {battle.state.boss.hp}/{battle.state.boss.max_hp} (phase {battle.state.boss.phase})";

        RebuildButtons();
    }


    private void RebuildButtons()
    {
        foreach (var child in buttons_container.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var hero in battle.state.heroes)
        {
            foreach (var ability in hero.abilities)
            {
                var button = new Button
                {
                    Text = $"{hero.name}: {ability.name} ({ability.cost})",
                    Disabled = !battle.canUse(hero, ability)
                };
                button.Pressed += () => OnAbility(hero, ability);
                
                buttons_container.AddChild(button);
            }
        }
    }


    private async void OnAbility(Combatant.Hero hero, Ability ability)
    {
        switch (ability)
        {
            case Ability.Offensive offensive: 
                battle.useOffensiveAbility(hero, offensive); 
                break;
            case Ability.Support support:
                var ally = battle.state.heroes.Find(it => it != hero);
                battle.useSupportAbility(hero, support, ally);
                break;
            case Ability.Self self:
                battle.useSelfAbility(hero, self);
                break;
            default: 
                throw new UnreachableException();
        }

        if (battle.is_hero_phase_over)
        {
            await battle.runBossTurn();
        }
    }
}