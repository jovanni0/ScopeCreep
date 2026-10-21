namespace ScopeCreep.Core;



public class Boss : Combatant
{
    public int Phase = 1;
    
    public Element Weakness => Phase == 1 ? Element.Ice : Element.Fire;
}