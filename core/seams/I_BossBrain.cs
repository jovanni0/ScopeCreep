using ScopeCreep.Core.model;

namespace ScopeCreep.Core.seams;



public interface I_BossBrain
{
    BossAttack pickAttack(BattleState state);
}