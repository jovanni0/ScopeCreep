using ScopeCreep.Core.enums;

namespace ScopeCreep.Core.model;



public class BattleState
{
    public const int SP_REGEN = 3;
    public const int SP_CAP   = 6;

    public List<Combatant.Hero> heroes { get; internal set; } = new List<Combatant.Hero>();
    public Combatant.Boss       boss   { get; internal set; } = new();
    
    public int          spirit_points { get; internal set; } = 3;
    public int          round         { get; internal set; } = 1;
    public BattleStatus status        { get; internal set; } = BattleStatus.IN_PROGRESS;
}