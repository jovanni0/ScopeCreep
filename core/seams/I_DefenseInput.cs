namespace ScopeCreep.Core.seams;



public interface I_DefenseInput
{
    Task<DefenseResult> Resolve(BossAttack attack, Hero target);
}