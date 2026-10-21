namespace ScopeCreep.Core.abilities;



public class Firewall : Ability
{
    public override string Name => "Firewall";
    public override int Cost => 2;


    public override void Execute(Battle battle, Hero user, Combatant target)
    {
        battle.DealDamage(user, target, 10, Element.Fire);
    }
}