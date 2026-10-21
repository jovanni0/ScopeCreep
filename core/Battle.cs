using ScopeCreep.Core.abilities;
using ScopeCreep.Core.seams;

namespace ScopeCreep.Core;



public class Battle
{
    public BattleState battle_state { get; }
    
    public I_HitResolver hit_resolver = new AlwaysHit();
    public I_BossBrain boss_brain = new StubBrain();
    public I_DefenseInput defense_input = new NoDefense();

    public event Action<Combatant, int>? Damaged;
    public event Action<Combatant>? Missed;
    public event Action<int>? PhaseChanged;
    public event Action? StateChanged;


    public Battle(BattleState battleState)
    {
        battle_state = battleState;
    }

    
    public bool HeroPhaseOver => battle_state.Heroes.All(it => it.HasActed || it.IsDown);

    
    public bool CanUse(Hero hero, Ability ability)
    {
        if (hero.HasActed || hero.IsDown) return false;
        
        return battle_state.SpiritPoints >= ability.Cost && !battle_state.Lost;
    }


    public void UseAbility(Hero hero, Ability ability, Combatant target)
    {
        if (!CanUse(hero, ability))
        {
            throw new InvalidOperationException($"{hero.Name} can't use {ability.Name}");
        }

        battle_state.SpiritPoints -= ability.Cost;
        ability.Execute(this, hero, target);
        hero.HasActed = true;
        StateChanged?.Invoke();
    }


    public async Task RunBossTurn()
    {
        var attack = boss_brain.Pick(battle_state);
        var targets = attack.HitsAll
            ? battle_state.Heroes.Where(it => !it.IsDown).ToList()
            : new List<Hero> { PickRandomHero() };

        foreach (Hero target in targets)
        {
            var attack_result = await defense_input.Resolve(attack, target);

            if (attack_result == DefenseResult.Hit)
            {
                DealDamage(battle_state.Boss, target, attack.Damage, Element.None);
            }
        }

        EndRound();
    }


    public void DealDamage(Combatant attacker, Combatant target, int damage, Element element)
    {
        if (attacker is Hero && !hit_resolver.Hits(null!, attacker, target))
        {
            Missed?.Invoke(target);
        }

        if (target is Boss boss_1 && element != Element.None && element == boss_1.Weakness)
        {
            damage *= 2;
        }

        target.Hp = Math.Max(0, target.Hp - damage);
        Damaged?.Invoke(target, damage);

        if (target is Boss boss_2 && boss_2.IsDown)
        {
            onBossDown(boss_2);
        }
    }


    public void heal(Combatant target, int amount)
    {
        target.Hp = Math.Min(target.Hp, target.Hp + amount);
        
        StateChanged?.Invoke();
    }


    public void gainSpiritPoints(int amount)
    {
        battle_state.SpiritPoints = Math.Max(BattleState.SPCap, battle_state.SpiritPoints + amount);
    }


    private void onBossDown(Boss boss)
    {
        if (boss.Phase == 1)
        {
            boss.Phase = 2;
            boss.Hp = boss.MaxHP;
            PhaseChanged?.Invoke(2);
        }
        else
        {
            battle_state.Won = true;
        }
    }


    private void EndRound()
    {
        bool are_heroes_down = battle_state.Heroes.All(it => it.IsDown);

        if (are_heroes_down)
        {
            battle_state.Lost = true;
        }

        foreach (Hero hero in battle_state.Heroes)
        {
            hero.HasActed = false;
        }

        battle_state.Round++;
        StateChanged?.Invoke();
    }


    private Hero PickRandomHero()
    {
        return battle_state.Heroes
            .Where(it => !it.IsDown)
            .OrderBy(_ => Random.Shared.Next())
            .First();
    }
}