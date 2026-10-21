using ScopeCreep.Core.model;

namespace ScopeCreep.Core.seams;



public interface I_HitResolver
{
    bool hits(Ability ability, Combatant user, Combatant target);
}