namespace ScopeCreep.Core.seams;



public class NoDefense : I_DefenseInput
{
    public Task<DefenseResult> Resolve(BossAttack attack, Hero target)
    {
        return Task.FromResult(DefenseResult.Hit);
    }
}