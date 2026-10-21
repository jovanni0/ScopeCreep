namespace ScopeCreep.Core.abilities;



public class CoffeeBreak : Ability
{
    public override string Name => "Coffee Break";
    public override int Cost => 0;
    public override bool TargetsAlly => false;


    public override void Execute(Battle battle, Hero user, Combatant target)
    {
        battle.gainSpiritPoints(2);
    }
}