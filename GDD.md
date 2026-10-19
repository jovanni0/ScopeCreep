# ScopeCreep GDD (Team 5)

## Core Concept

A small team of devs try to build a game. What started as a simple idea quickly became overwhelming and started to actively battle the devs who try to get it back under control by cutting it down layer by layer, otherwise the game will never ship.

The characters (heroes) are the devs, each with their unique skill set. The boss they battle is the bloated game personified as the Ramen Bowl, the perfect game that has become a monster. It started as a simple broth and is now buried in features nobody asked for / wants, including meatballs, which don't even belong in ramen.

The player controls 2 heroes: the Coder and the Designer. Each hero has their own dodge/parry button.

## Theme

2D pixel art, turn based.

## Turn Structure

A round is both heroes acting (in any order), then the boss acting. Debuff durations count in rounds. Abilities have no cooldowns, they are balanced only by their SP cost.

During each boss attack, the heroes can dodge or parry (see Dodge & Parry).

A hero at 0 HP is down and can't act (no revive for now). The heroes lose when both are down, and win when the boss reaches 0 HP in phase 2.

## Debuffs

The same debuffs apply to heroes and to the boss. Reapplying a debuff refreshes its duration, it does not stack.

| Debuff       | Duration                     | Effect                                                                                 |
| ------------ | ---------------------------- | -------------------------------------------------------------------------------------- |
| Defense drop | 4 rounds                     | damage taken by the character is increased (15–30%)                                    |
| Attack drop  | 4 rounds                     | damage dealt by the character is decreased (15–30%)                                    |
| Burn         | 3 rounds, damage at round end | fire type damage over time                                                             |
| Freeze       | 1 round                      | the frozen character skips its next turn and is weak to fire damage (Thermal Shock)    |

**Thermal Shock**: fire damage on a frozen character deals bonus damage. it works both in the round the freeze was applied and in the next round, while the character is still frozen.

Example: the Designer freezes the boss, the Coder uses Firewall in the same round (Thermal Shock), the boss skips its turn, and next round the heroes can use fire on it again while it is still frozen (Thermal Shock again).

## Heroes

**The Coder** (support + fire damage):
  - *Git Revert*: cleans up debuffs.
  - *Hotfix*:     heal.
  - *Firewall*:   fire damage + burn (15%).

**The Designer** (ice damage + burst + debuffs):
  - *FreezeFrame*:    ice damage + freeze (25%).
  - *Pixel Perfect*:  big damage, but higher chance of the boss dodging it (35%).
  - *Negative Space*: defense drop and attack drop on the boss.

Both heroes have *Coffee Break*: skip their turn and gain +2SP.

## Boss

**The Ramen Bowl** has 2 phases. When phase 1 reaches 0 HP, the boss heals to full and switches to phase 2.

**Phase 1** (weak to ice, x2 damage):
  - *MEATeor Shower* (Cloudy with a Chance of Meatballs): throws meatballs with forks. a hero that is hit has a chance of defense drop (TBD, ~40%).
  - *Flying Fists*: damage to one hero.

**Phase 2** (weak to fire, x2 damage):
  - *Scalding Broth*: splash damage on both heroes + burn (15%).
  - *MEATeor Hammer*: a sweeping attack across the battlefield that goes back and forth a few times. if the heroes parry 3 times in a row (counted across both heroes), the boss takes massive damage.

Weaknesses stack with Thermal Shock (ex: in phase 2, Thermal Shock on a frozen boss = fire weakness + Thermal Shock bonus).

The boss picks one of its current phase's attacks each turn.

## Dodge & Parry

During a boss attack each hero has a timing window to press their button:
  - pressed in the window = **dodge**: no damage and no debuff from the attack.
  - pressed at the perfect moment (tighter window) = **parry**: no damage, no debuff, plus a small counter (TBD: small damage or +1SP).
  - not pressed / missed = the hero takes the hit.

Attacks that hit both heroes (Scalding Broth, MEATeor Hammer) need both buttons timed.

Boss damage should be tuned assuming the player misses about half the time.

## Economy

The heroes have a shared pool of **Spirit Points**. each ability costs a certain amount of points, and the player needs to be careful not to empty the pool (reach Burnout). if there are no points in the pool the only ability available is Coffee Break.

The pool starts at 3SP and caps at 6SP. at the start of each round the pool regains +3SP. Coffee Break's +2SP is extra, on top of the regen.

| Ability        | Cost       |
| -------------- | ---------- |
| Git Revert     | 1          |
| Hotfix         | 3          |
| Firewall       | 2          |
| FreezeFrame    | 2          |
| Pixel Perfect  | 3          |
| Negative Space | 3          |
| Coffee Break   | 0, gain +2 |

Hotfix costs a full round of regen so healing every round isn't free; the heal amount should stay below the boss's average damage per round. Negative Space doesn't need a cooldown because recasting it only refreshes the 4 round duration.

## Starting Numbers (placeholders, tune by playing)

- Boss: TBD HP per phase
- Heroes: TBD HP each
- Damage / heal values: TBD (Pixel Perfect ~2x a normal attack, Hotfix heal < boss average damage per round)
- Defense / attack drop: 15–30%
- Thermal Shock bonus: TBD (ex: +50%)
- MEATeor Hammer parry counter: TBD (~15–20% of phase 2 HP)
- MEATeor Shower defense drop chance: TBD (~40%)

## Mechanics

| #  | Mechanic                | What it covers                                                           | Used by                                              |
| -- | ----------------------- | ------------------------------------------------------------------------ | ---------------------------------------------------- |
| 1  | Round & turn loop       | heroes act in any order, then the boss; downed heroes; win/lose          | everything                                           |
| 2  | Spirit Points pool      | start 3SP, +3SP per round, cap 6SP, Burnout, Coffee Break +2SP            | all abilities                                        |
| 3  | Damage & healing        | one damage and one heal function every ability goes through              | every attack, Hotfix                                 |
| 4  | Boss phases             | phase 1 -> heal to full -> phase 2, swaps attacks, weakness and sprite    | the boss                                             |
| 5  | Status effects          | burn and freeze, with durations, applied by chance                       | Firewall, FreezeFrame, Scalding Broth                |
| 6  | Stat modifiers          | defense drop and attack drop (15–30%, 4 rounds, refresh on reapply)       | Negative Space, MEATeor Shower                       |
| 7  | Cleanse                 | remove debuffs from a hero                                               | Git Revert                                           |
| 8  | Boss AI                 | picks one of the current phase's attacks each turn                       | the boss turn                                        |
| 9  | Dodge & parry           | timing windows during boss attacks, one button per hero                  | every boss attack                                    |
| 10 | Hit/miss & chance rolls | miss chance, debuff chances, seedable RNG                                | Pixel Perfect, all chance-based debuffs              |
| 11 | Elemental weakness      | fire / ice damage types, x2 against the phase's weakness                 | Firewall, FreezeFrame, Burn                          |
| 12 | Thermal Shock           | bonus fire damage on a frozen character, stacks with weakness            | Firewall, Burn + FreezeFrame                         |
| 13 | Parry chain             | 3 parries in a row across both heroes -> massive damage to the boss      | MEATeor Hammer                                       |

## Task Split

The project starts with shared interfaces that each have a default (stub) implementation, so the game runs from day one and each system replaces its stub when ready. Each person works in their own folder to avoid merge conflicts.

**Dev 1 (critical path, starts now)**: mechanics 1, 2, 3, 4
- project setup, shared interfaces and stubs
- placeholder UI (rectangles, HP bars, SP counter, ability buttons)
- goal: a playable fight on stubs (boss always uses Flying Fists, everything always hits, no dodging)

**Dev 2 (in parallel, mostly pure C# logic)**: mechanics 5, 6, 7, 8, 9
- order: status effects, stat modifiers, cleanse, boss AI (replaces the stub), then dodge & parry
- the logic parts are testable on their own against the interfaces and never wait on the UI
- dodge & parry is the only real-time part (input + animation timing), so it comes last

**Designer (later, after the art)**: mechanics 10, 11, 12, 13
- order: hit/miss (replaces the "always hit" stub), elemental weakness, Thermal Shock (needs 5), parry chain (needs 9)
- these are small rules inside the damage pipeline; none of them block the game

Dependencies:

```
Dev 1:    [1 turn loop] -> [2 SP] -> [3 damage] -> [4 phases]
Dev 2:          [5 status] -> [6 stat mods] -> [7 cleanse] -> [8 boss AI] -> [9 dodge & parry]
Designer:                     [10 hit/miss] -> [11 elements] -> [12 Thermal Shock] -> [13 parry chain]
                                                                 (needs 5)            (needs 9)
```

If we fall behind, cut in this order: parry chain, then Thermal Shock. Everyone still keeps at least 3 mechanics.

NOTE: careful at the number of features of the game or we'll end up over engineering it.
