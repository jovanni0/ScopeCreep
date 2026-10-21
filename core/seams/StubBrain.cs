using ScopeCreep.Core.model;

namespace ScopeCreep.Core.seams;



public class StubBrain : I_BossBrain
{
    public BossAttack pickAttack(BattleState state)
    {
        return new BossAttack("Flying Fists", 8, false);
    }
}