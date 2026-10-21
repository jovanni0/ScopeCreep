using ScopeCreep.Core.enums;
using ScopeCreep.Core.model;

namespace ScopeCreep.Core.seams;



public interface I_DefenseInput
{
    Task<DefenseResult> resolve(BossAttack attack, Combatant.Hero target);
}