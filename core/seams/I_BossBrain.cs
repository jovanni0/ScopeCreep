namespace ScopeCreep.Core.seams;



public interface I_BossBrain
{
    BossAttack Pick(BattleState state);
}