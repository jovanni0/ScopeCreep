namespace ScopeCreep.Core.abilities;



public class Hotfix : Ability
{
    public override string Name => "Hotfix";
    public override int Cost => 3;
    public override bool TargetsAlly => true;


    public override void Execute(Battle battle, Hero user, Combatant target)
    {
        battle.heal(target, 15);
    }
}