using ScopeCreep.Core.enums;

namespace ScopeCreep.Core.model;



public abstract record Ability
{
    public string name { get; }
    public int cost { get; }


    private Ability(string name, int cost)
    {
        this.name = name;
        this.cost = cost;
    }

    
    /**
     *  Offensive abilities deal damage only to the boss.
     */
    public abstract record Offensive : Ability
    {
        private Offensive(string name, int cost) : base(name, cost)
        { }
        
        public abstract void execute(Battle battle, Combatant.Hero user, Combatant.Boss target);


        /**
         *  Fire attack damage to the boss.
         *  TODO: implement burn chance.
         */
        public sealed record Firewall() : Offensive("Firewall", 2)
        {
            public override void execute(Battle battle, Combatant.Hero user, Combatant.Boss target)
            {
                battle.dealDamageToBoss(user, this, 10, Element.FIRE);
            }
        }


        /**
         *  Ice attack damage to the boss.
         *  TODO: implement freeze chance.
         */
        public sealed record FreezeFrame() : Offensive("Freeze Frame", 2)
        {
            public override void execute(Battle battle, Combatant.Hero user, Combatant.Boss target)
            {
                battle.dealDamageToBoss(user, this, 10, Element.ICE);
            }
        }


        /**
         *  Big hitter attack damage, with high change of the boss dodging it.
         */
        public sealed record PixelPerfect() : Offensive("Pixel Perfect", 3)
        {
            public override void execute(Battle battle, Combatant.Hero user, Combatant.Boss target)
            {
                battle.dealDamageToBoss(user, this, 20, Element.NONE);
            }
        }


        /**
         *  Attack and defense drop on the boss.
         *  TODO: implement stat drop on the boss.
         */
        public sealed record NegativeSpace() : Offensive("Negative Space", 3)
        {
            public override void execute(Battle battle, Combatant.Hero user, Combatant.Boss target)
            {
                
            }
        }
    }


    /**
     *  Support abilities help other heroes.
     */
    public abstract record Support : Ability
    {
        private Support(string name, int cost) : base(name, cost)
        { }

        public abstract void execute(Battle battle, Combatant.Hero receiver);


        /**
         *  Heals one teammate.
         */
        public sealed record Hotfix() : Support("Hotfix", 3)
        {
            public override void execute(Battle battle, Combatant.Hero receiver)
            {
                battle.heal(receiver, 15);
            }
        }


        /**
         *  Removes debuffs on entire team.
         *  TODO: implement.
         */
        public sealed record GitRevert() : Support("Git Revert", 1)
        {
            public override void execute(Battle battle, Combatant.Hero receiver)
            {
                
            }
        }
    }


    /**
     *  Self abilities are used by the hero on themselves.
     */
    public abstract record Self : Ability
    {
        private Self(string name, int cost) : base(name, cost)
        { }

        public abstract void execute(Battle battle, Combatant.Hero user);


        /**
         *  The hero skips their turn and gain +2SP to the pool.
         */
        public sealed record CoffeeBreak() : Self("Coffee Break", 0)
        {
            public override void execute(Battle battle, Combatant.Hero user)
            {
                battle.gainSpiritPoints(2);
            }
        }
    }
}
