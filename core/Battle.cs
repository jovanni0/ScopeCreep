using ScopeCreep.Core.enums;
using ScopeCreep.Core.model;
using ScopeCreep.Core.seams;

namespace ScopeCreep.Core;



public class Battle
{
    public BattleState state { get; }
    
    public I_HitResolver  hit_resolver  = new AlwaysHit();
    public I_BossBrain    boss_brain    = new StubBrain();
    public I_DefenseInput defense_input = new NoDefense();

    public event Action<Combatant, int>? damaged;
    public event Action<Combatant>?      missed;
    public event Action<BossPhase>?      phase_changed;
    public event Action?                 state_changed;

    public bool is_hero_phase_over => state.heroes.All(it => it.has_acted || it.is_down);
    

    public Battle(BattleState state)
    {
        this.state = state;
    }

    
    /**
     *  Check if the hero can use the ability.
     */
    public bool canUse(Combatant.Hero hero, Ability ability)
    {
        if (hero.has_acted || hero.is_down) return false;

        if (state.status != BattleStatus.IN_PROGRESS) return false;
        
        return state.spirit_points >= ability.cost;
    }
    
    
    public void useOffensiveAbility(Combatant.Hero user, Ability.Offensive ability)
    {
        pay(user, ability);
        ability.execute(this, user, state.boss);
        finishAction(user);
    }


    public void useSupportAbility(Combatant.Hero user, Ability.Support ability, Combatant.Hero receiver)
    {
        pay(user, ability);
        ability.execute(this, receiver);
        finishAction(user);
    }


    public void useSelfAbility(Combatant.Hero user, Ability.Self ability)
    {
        pay(user, ability);
        ability.execute(this, user);
        finishAction(user);
    }


    public async Task runBossTurn()
    {
        if (state.status != BattleStatus.IN_PROGRESS) return;
            
        var attack = boss_brain.pickAttack(state);
        var targets = attack.hits_all
            ? state.heroes.Where(it => !it.is_down).ToList()
            : new List<Combatant.Hero> { pickRandomHero() };

        foreach (Combatant.Hero target in targets)
        {
            var attack_result = await defense_input.resolve(attack, target);

            if (attack_result == DefenseResult.HIT)
            {
                dealDamageToHero(target, attack.damage);
            }
        }

        endRound();
    }


    private void dealDamageToHero(Combatant.Hero target, int damage)
    {
        target.takeDamage(damage);
        damaged?.Invoke(target, damage);
    }


    public void dealDamageToBoss(Combatant.Hero attacker, Ability.Offensive ability, int damage, Element damage_type)
    {
        var boss = state.boss;
            
        if (!hit_resolver.hits(ability, attacker, boss))
        {
            missed?.Invoke(boss);
            return;
        }

        if (damage_type == boss.weakness)
        {
            damage *= 2;
        }

        boss.takeDamage(damage);
        damaged?.Invoke(boss, damage);

        if (boss.is_down)
        {
            onBossDown(boss);
        }
    }
    
    
    public void heal(Combatant receiver, int amount)
    {
        receiver.heal(amount);
        state_changed?.Invoke();
    }


    public void gainSpiritPoints(int amount)
    {
        state.spirit_points = Math.Min(BattleState.SP_CAP, state.spirit_points + amount);
    }


    /**
     *  Pay for the usage of the hero's ability. 
     */
    private void pay(Combatant.Hero user, Ability ability)
    {
        if (!canUse(user, ability))
        {
            throw new InvalidOperationException($"{user.name} can't use {ability.name}");
        }

        state.spirit_points -= ability.cost;
    }


    /**
     *  Mark the hero as having acted and trigger state change.
     */
    private void finishAction(Combatant.Hero user)
    {
        user.has_acted = true;
        state_changed?.Invoke();
    }


    /**
     *  Decide if the boss switches phases or remains down.
     */
    private void onBossDown(Combatant.Boss boss)
    {
        if (boss.can_advance_phase)
        {
            boss.advancePhase();
            phase_changed?.Invoke(boss.phase);
        }
        else
        {
            state.status = BattleStatus.WON;
        }
    }


    private void endRound()
    {
        bool are_heroes_down = state.heroes.All(it => it.is_down);

        if (are_heroes_down)
        {
            state.status = BattleStatus.LOST;
        }

        foreach (Combatant.Hero hero in state.heroes)
        {
            hero.has_acted = false;
        }

        state.round++;
        gainSpiritPoints(BattleState.SP_REGEN);
        state_changed?.Invoke();
    }


    private Combatant.Hero pickRandomHero()
    {
        return state.heroes
            .Where(it => !it.is_down)
            .OrderBy(_ => Random.Shared.Next())
            .First();
    }
}