using ScopeCreep.Core.enums;
using ScopeCreep.Core.model;

namespace ScopeCreep.Core.Tests;



public class BattleTests
{
    private static Battle newBattle()
    {
        var state = new BattleState();
        state.heroes.Add(Combatant.Hero.coder());
        state.heroes.Add(Combatant.Hero.designer());
        return new Battle(state);
    }


    [Fact]
    public void firewallDealsNormalDamageInPhase1()
    {
        var battle = newBattle();
        var coder  = battle.state.heroes[0];

        battle.useOffensiveAbility(coder, new Ability.Offensive.Firewall());

        Assert.Equal(50, battle.state.boss.hp);
    }


    [Fact]
    public void bossHealsAndEntersPhase2WhenDownInPhase1()
    {
        var battle = newBattle();
        var coder  = battle.state.heroes[0];

        battle.dealDamageToBoss(coder, new Ability.Offensive.Firewall(), 60, Element.NONE);

        Assert.Equal(BossPhase.PHASE_2, battle.state.boss.phase);
        Assert.Equal(60, battle.state.boss.hp);
        Assert.Equal(BattleStatus.IN_PROGRESS, battle.state.status);
    }


    [Fact]
    public void firewallDealsDoubleDamageInPhase2()
    {
        var battle = newBattle();
        var coder  = battle.state.heroes[0];
        battle.dealDamageToBoss(coder, new Ability.Offensive.Firewall(), 60, Element.NONE);

        battle.useOffensiveAbility(coder, new Ability.Offensive.Firewall());

        Assert.Equal(40, battle.state.boss.hp);
    }


    [Fact]
    public void healCannotExceedMaxHp()
    {
        var battle   = newBattle();
        var coder    = battle.state.heroes[0];
        var designer = battle.state.heroes[1];

        battle.useSupportAbility(coder, new Ability.Support.Hotfix(), designer);

        Assert.Equal(designer.max_hp, designer.hp);
    }


    [Fact]
    public async Task spiritPointsRegenerateAfterBossTurn()
    {
        var battle   = newBattle();
        var coder    = battle.state.heroes[0];
        var designer = battle.state.heroes[1];

        battle.useSupportAbility(coder, new Ability.Support.Hotfix(), designer);   // 3 - 3 = 0
        battle.useSelfAbility(designer, new Ability.Self.CoffeeBreak());           // 0 + 2 = 2
        await battle.runBossTurn();                                                // 2 + 3 = 5

        Assert.Equal(5, battle.state.spirit_points);
    }


    [Fact]
    public void spiritPointsAreCapped()
    {
        var battle = newBattle();

        foreach (var hero in battle.state.heroes)
        {
            battle.useSelfAbility(hero, new Ability.Self.CoffeeBreak());   // 3 + 2 + 2 = 7, capped
        }

        Assert.Equal(BattleState.SP_CAP, battle.state.spirit_points);
    }


    [Fact]
    public void heroCannotActTwiceInOneRound()
    {
        var battle = newBattle();
        var coder  = battle.state.heroes[0];
        battle.useSelfAbility(coder, new Ability.Self.CoffeeBreak());

        Assert.Throws<InvalidOperationException>(
            () => battle.useSelfAbility(coder, new Ability.Self.CoffeeBreak()));
    }
}