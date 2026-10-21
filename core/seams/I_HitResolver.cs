using ScopeCreep.Core.abilities;

namespace ScopeCreep.Core.seams;



public interface I_HitResolver
{
    bool Hits(Ability ability, Combatant user, Combatant target);
}