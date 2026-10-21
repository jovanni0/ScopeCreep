using ScopeCreep.Core.enums;
using ScopeCreep.Core.model;

namespace ScopeCreep.Core.seams;



public class NoDefense : I_DefenseInput
{
    public Task<DefenseResult> resolve(BossAttack attack, Combatant.Hero target)
    {
        return Task.FromResult(DefenseResult.HIT);
    }
}