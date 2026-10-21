namespace ScopeCreep.Core.seams;



public class StubBrain : I_BossBrain
{
    public BossAttack Pick(BattleState state)
    {
        return new BossAttack("Flying Fists", 8, false);
    }
}