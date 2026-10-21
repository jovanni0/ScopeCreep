using System;
using Godot;
using ScopeCreep.Core;
using ScopeCreep.Core.model;

namespace ScopeCreep.Game;



public partial class HeroPanel : PanelContainer
{
    [Export] private Label         name_label = null!;
    [Export] private ProgressBar   hp_bar = null!;
    [Export] private Label         hp_label = null!;
    [Export] private Button        target_button = null!;
    [Export] private VBoxContainer abilities_container = null!;

    public Combatant.Hero hero { get; private set; } = null!;

    public Action<Combatant.Hero, Ability>? ability_pressed;
    public Action<Combatant.Hero>?          target_pressed;


    public override void _Ready()
    {
        target_button.Pressed += () => target_pressed?.Invoke(hero);
    }


    public void setup(Combatant.Hero hero)
    {
        this.hero = hero;
        name_label.Text = hero.name;
        hp_bar.MaxValue = hero.max_hp;
    }


    public void refresh(Battle battle, bool is_targeting, bool is_boss_acting)
    {
        hp_bar.Value = hero.hp;
        hp_label.Text = $"HP {hero.hp}/{hero.max_hp}";

        target_button.Visible = is_targeting && !hero.is_down;

        abilities_container.Modulate = hero.is_down || hero.has_acted
            ? new Color(1, 1, 1, 0.4f)
            : Colors.White;

        foreach (var child in abilities_container.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var ability in hero.abilities)
        {
            var button = new Button
            {
                Text = ability.cost == 0 ? $"{ability.name} (free)" : $"{ability.name} ({ability.cost} SP)",
                Disabled = is_targeting || is_boss_acting || !battle.canUse(hero, ability),
                Alignment = HorizontalAlignment.Left,
            };
            button.Pressed += () => ability_pressed?.Invoke(hero, ability);
            
            abilities_container.AddChild(button);
        }
    }
}