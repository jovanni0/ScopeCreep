using ScopeCreep.Core.abilities;

namespace ScopeCreep.Core.seams;



public class AlwaysHit : I_HitResolver
{
    public bool Hits(Ability ability, Combatant user, Combatant target)
    {
        return true;
    }
}