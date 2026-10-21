using ScopeCreep.Core.model;

namespace ScopeCreep.Core.seams;



public class AlwaysHit : I_HitResolver
{
    public bool hits(Ability ability, Combatant user, Combatant target)
    {
        return true;
    }
}