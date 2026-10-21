using Godot;
using ScopeCreep.Core;
using ScopeCreep.Core.abilities;

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
        battle_state.Boss = new Boss
        {
            Name = "Ramen Bowl", 
            Hp = 60, 
            MaxHP = 60
        };
        battle_state.Heroes.Add(new Hero
        {
            Name = "Coder", 
            Hp = 40, 
            MaxHP = 40, 
            Abilities = {
                new Firewall(), 
                new Hotfix(), 
                new CoffeeBreak()
            }
        });
        battle_state.Heroes.Add(new Hero
        {
            Name = "Designer", 
            Hp = 40, 
            MaxHP = 40, 
            Abilities = {
                new CoffeeBreak()
            }
        });
        
        battle = new Battle(battle_state);
        battle.StateChanged += Refresh;
        battle.Damaged += (who, damage) => GD.Print($"{who.Name} took {damage}");

        Refresh();
    }


    private void Refresh()
    {
        SP_label.Text = $"SP {battle.battle_state.SpiritPoints}/{BattleState.SPCap}";
        boss_HP.Text = $"Boss {battle.battle_state.Boss.Hp}/{battle.battle_state.Boss.MaxHP} (phase {battle.battle_state.Boss.Phase})";

        RebuildButtons();
    }


    private void RebuildButtons()
    {
        foreach (var child in buttons_container.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var hero in battle.battle_state.Heroes)
        {
            foreach (var ability in hero.Abilities)
            {
                var button = new Button
                {
                    Text = $"{hero.Name}: {ability.Name} ({ability.Cost})",
                    Disabled = !battle.CanUse(hero, ability)
                };
                button.Pressed += () => OnAbility(hero, ability);
                
                buttons_container.AddChild(button);
            }
        }
    }


    private async void OnAbility(Hero hero, Ability ability)
    {
        Combatant target = ability.TargetsAlly ? hero : battle.battle_state.Boss;
        
        battle.UseAbility(hero, ability, target);

        if (battle.HeroPhaseOver)
        {
            await battle.RunBossTurn();
        }
    }
}