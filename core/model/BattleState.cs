namespace ScopeCreep.Core;



public class BattleState
{
    public const int SPRegen = 3;
    public const int SPCap = 6;
    
    public List<Hero> Heroes = new();
    public Boss Boss = new();
    
    public int SpiritPoints = 3;
    public int Round = 1;

    public bool Won;
    public bool Lost;
}