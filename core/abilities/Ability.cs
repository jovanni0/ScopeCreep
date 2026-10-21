namespace ScopeCreep.Core.abilities;

public abstract class Ability
{
    public abstract string Name { get; }
    public abstract int Cost { get; }

    public virtual bool TargetsAlly => false;


    public abstract void Execute(Battle battle, Hero user, Combatant target);
}