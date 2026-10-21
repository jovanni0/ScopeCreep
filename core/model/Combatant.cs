using System.Diagnostics;
using ScopeCreep.Core.enums;

namespace ScopeCreep.Core.model;



public abstract class Combatant
{
    public string name { get; }
    public int    hp { get; private set; }
    public int    max_hp { get; }

    public bool is_down => hp <= 0;


    private Combatant(string name, int max_hp)
    {
        this.name = name;
        this.max_hp = max_hp;
        this.hp = max_hp;
    }

    internal void takeDamage(int amount) => hp = Math.Max(0, hp - amount);
    internal void heal(int amount)       => hp = Math.Min(hp + amount, max_hp);
    private void restoreFullHp()         => hp = max_hp;
    
    
    public sealed class Boss() : Combatant("The Ramen Bowl", 60)
    {
        public BossPhase phase { get; private set; } = BossPhase.PHASE_1;
        
        public bool can_advance_phase => phase == BossPhase.PHASE_1;
        
        public Element weakness => phase switch
        {
            BossPhase.PHASE_1 => Element.ICE,
            BossPhase.PHASE_2 => Element.FIRE,
            _ => throw new UnreachableException()
        };


        public void advancePhase()
        {
            phase = BossPhase.PHASE_2;
            
            this.restoreFullHp();
        }
    }


    public sealed class Hero : Combatant
    {
        public IReadOnlyList<Ability> abilities { get; }
        public bool                   has_acted { get; internal set; }
        
        
        private Hero(string name, List<Ability> abilities, int max_hp) : base(name, max_hp)
        {
            this.abilities = abilities;
            this.has_acted = false;
        }


        public static Hero coder() => new Hero(
            "The Coder",
            [
                new Ability.Self.CoffeeBreak(),
                new Ability.Offensive.Firewall(),
                new Ability.Support.GitRevert(),
                new Ability.Support.Hotfix()
            ],
            40
        );


        public static Hero designer() => new Hero(
            "The Designer",
            [
                new Ability.Self.CoffeeBreak(),
                new Ability.Offensive.FreezeFrame(),
                new Ability.Offensive.NegativeSpace(),
                new Ability.Offensive.PixelPerfect()
            ],
            40
        );
    }
}