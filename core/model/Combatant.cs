namespace ScopeCreep.Core;



public class Combatant
{
    public string Name;
    public int Hp;
    public int MaxHP;

    public bool IsDown => Hp <= 0;
}