using ScopeCreep.Core.abilities;

namespace ScopeCreep.Core;




public class Hero : Combatant
{
    public List<Ability> Abilities = new();
    public bool HasActed;
}