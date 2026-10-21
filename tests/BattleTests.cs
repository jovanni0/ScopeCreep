using ScopeCreep.Core.abilities;

namespace ScopeCreep.Core.Tests;

// tests/BattleTests.cs
using ScopeCreep.Core;

public class BattleTests {
    static Battle NewBattle() {
        var s = new BattleState();
        s.Boss = new Boss { Name = "Ramen Bowl", Hp = 60, MaxHP = 60 };
        s.Heroes.Add(new Hero { Name = "Coder", Hp = 40, MaxHP = 40,
            Abilities = { new Firewall(), new Hotfix(), new CoffeeBreak() } });
        s.Heroes.Add(new Hero { Name = "Designer", Hp = 40, MaxHP = 40,
            Abilities = { new CoffeeBreak() } });
        return new Battle(s);
    }

    [Fact]
    public void Fire_does_double_damage_in_phase_2() {
        var b = NewBattle();
        b.battle_state.Boss.Phase = 2;
        var coder = b.battle_state.Heroes[0];
        b.UseAbility(coder, coder.Abilities[0], b.battle_state.Boss);
        Assert.Equal(40, b.battle_state.Boss.Hp);
    }

    [Fact]
    public async Task Sp_regens_and_caps_after_boss_turn() {
        var b = NewBattle();
        foreach (var h in b.battle_state.Heroes) b.UseAbility(h, new CoffeeBreak(), h);
        await b.RunBossTurn();
        Assert.Equal(BattleState.SPCap, b.battle_state.SpiritPoints);   // 3 + 2 + 2 + 3, capped at 6
    }
}