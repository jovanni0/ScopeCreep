# ScopeCreep GDD (Team 5)

## Core Concept

A small team of 3 devs try to build a game. What started as a simple idea quickly became overwhelming and started to actively battle the devs who try to get it back under control by removing some of the layers otherwise the game will never ship.

The characters (heroes) are the devs, each with their unique skill set. The boss they battle is the bloated game personified as the Ramen Bowl, the perfect game that has become a monster. It started as a simple broth and now is buried in toppings that nobody asked for / wants.

The party is always all 3 heroes. (Picking 2 of 3 could be added later as a harder "understaffed" mode.)

## Theme

2D pixel art, turn based.

## Character Ideas

### Heroes

**The Coder** with abilities:
  - *Refactor*:   deals damage and strips a topping.
  - *Hotfix*:     quick heal and cleans up debuffs (Spaghetti Code's stun).
  - *Git Revert*: undoes the effects of the boss's last action (time reverse): damage dealt is restored, the stun is removed, the added topping is removed, 
                  the Charge is undone. it does not undo the heroes' actions.

**The Designer** with abilities:
  - *Wireframe*:      shows the boss's next attack (the boss plans its action at the start of the round).
  - *Pixel Perfect*:  big damage attack but higher chance of the boss dodging it (miss).
  - *Negative Space*: little damage but strips 2 toppings off the boss.

**The Manager** with abilities:
  - *Meeting Time*: the boss is stunned and skips its next turn (including Charge), but the manager is exhausted and skips next round.
  - *MVP*:          1-turn shield for the whole party: blocks all damage and the stun from the next boss action.
  - *Ship It*:      damage + knockback. if the boss is stripped to the broth it acts like a finisher, otherwise it has a chance of stripping a topping. 
                    the boss cannot charge again in the next round. the knockback has a 2 round cooldown.

All heroes have the ability *Coffee Break*, which allows them to skip their turn and gain +2SP to the pool.

### Boss

**The Ramen Bowl** with abilities:
  - *Extra Toppings*: summons a topping back and get +20% HP (healing).
  - *Spaghetti Code*: stuns the character the boss is in close range of (Charge). the stunned character skips its next turn entirely (Coffee Break included).
  - *Scalding Broth*: splash area damage, with extra damage on the character the boss is in close range of.
  - *Charge*:         bonus action, can move in close range of one character (or from one character to another).

If the planned action is no longer possible when the boss acts (ex: it planned Spaghetti Code but got knocked back), it uses Scalding Broth instead.

There are 2 ways to beat the boss: deal a lot of damage or strip the toppings off of it and finish it. The boss' HP is scaled by the number of toppings it has, where each topping adds +20% of the base HP. In other words, stripping a topping deals 20% worth of damage, so it's way more effective to strip it than just keep damaging it. Once a turn, the boss can use its action to add a new topping (Extra Toppings), so just dealing damage is discouraged just because you may not be able to deal enough to keep up with the speed it heals.

## Economy

The heroes have a shared pool of **Spirit Points**. each ability of each character costs a certain amount of points, and the player needs to be careful not to empty the pool (reach Burnout). if there are no points in the pool the only ability available is Coffee Break.

The pool starts at 4SP and caps at a certain number of points (ex 8SP). at the start of each round the pool regains +4SP. Coffee Break's +2SP is extra, on top of the regen.

| Ability        | Cost       | Cooldown (rounds)                   |
| -------------- | ---------- | ----------------------------------- |
| Refactor       | 2          | –                                   |
| Hotfix         | 2          | 1                                   |
| Git Revert     | 3          | 3                                   |
| Wireframe      | 1          | –                                   |
| Pixel Perfect  | 3          | –                                   |
| Negative Space | 3          | –                                   |
| Meeting Time   | 2          | 2 (the exhaustion is an extra cost) |
| MVP            | 2          | 2                                   |
| Ship It        | 2          | 2 (knockback only)                  |
| Coffee Break   | 0, gain +2 | –                                   |


## Turn Structure

A round is all heroes acting (in any order), then the boss acting. Cooldowns count in rounds.

A hero at 0 HP is down and can't act (no revive for now). The heroes lose when all 3 are down, and win when the boss reaches 0 HP.

## Starting Numbers (placeholders, tune by playing)

- Boss: TBD base HP, TBD starting toppings, +20% base HP per topping
- Heroes: TBD HP each
- Damage / heal values: TBD
- Pixel Perfect miss chance: TBD
- Ship It strip chance (boss not stripped): TBD

## Mechanics

| #  | Mechanic                     | What it covers                                                                          | Used by                                                     |
| -- | ---------------------------- | --------------------------------------------------------------------------------------- | ----------------------------------------------------------- |
| 1  | Round & turn loop            | heroes act in any order, then the boss; downed heroes; win/lose                         | everything                                                  |
| 2  | Spirit Points pool           | start 4SP, +4SP per round, cap 8SP, Burnout, Coffee Break +2SP                          | all abilities                                               |
| 3  | Cooldowns                    | counted in rounds, block an ability until ready                                         | Hotfix, Git Revert, Meeting Time, MVP, Ship It (knockback)  |
| 4  | Damage & healing             | one damage and one heal function every ability goes through                             | every attack, Hotfix                                        |
| 5  | Topping-scaled boss HP       | damage-taken model, stripping, Extra Toppings regrowth, finisher bonus                  | Refactor, Negative Space, Ship It, Extra Toppings           |
| 6  | Status effects               | Stunned, Exhausted, Shielded, Knocked Back, with durations                              | Meeting Time, MVP, Spaghetti Code, Ship It                  |
| 7  | Cleanse                      | remove debuffs from a hero                                                              | Hotfix                                                      |
| 8  | Undo boss action             | each boss action records how to undo itself                                             | Git Revert                                                  |
| 9  | Boss AI                      | plans its action at round start, falls back to Scalding Broth if the plan is impossible | the boss turn                                               |
| 10 | Intent telegraph             | shows the boss's planned action in the UI                                               | Wireframe                                                   |
| 11 | Hit/miss & chance rolls      | miss chance, strip chance, seedable RNG                                                 | Pixel Perfect, Ship It                                      |
| 12 | Range / engagement           | which hero the boss is close to, Charge, knockback, close-range targeting               | Charge, Spaghetti Code, Scalding Broth, Ship It             |

## Task Split

The project starts with shared interfaces that each have a default (stub) implementation, so the game runs from day one and each system replaces its stub when ready. Each person works in their own folder to avoid merge conflicts.

**Dev 1 (critical path, starts now)**: mechanics 1, 2, 3, 4, 5
- project setup, shared interfaces and stubs
- placeholder UI (rectangles, HP bars, SP counter, ability buttons)
- goal: a playable fight on stubs (boss always uses Scalding Broth, everything always hits)

**Dev 2 (in parallel, pure C# logic, no scenes needed)**: mechanics 6, 7, 8, 9
- order: status effects, cleanse, undo, then the real boss AI replacing the stub
- testable on its own against the interfaces, never waits on the UI

**Designer (later, after the art)**: mechanics 11, 10, 12
- order: hit/miss (replaces the "always hit" stub), telegraph (needs 9), range (needs 6 and 9)
- none of these block the game; they add depth when they arrive

Dependencies:

```
Dev 1:    [1 turn loop] -> [2 SP] -> [3 cooldowns] -> [4 damage] -> [5 toppings]
Dev 2:          [6 status] -> [7 cleanse] -> [8 undo] -> [9 boss AI]
Designer:                          [11 hit/miss] -> [10 telegraph] -> [12 range]
                                                    (needs 9)         (needs 6 + 9)
```

If we fall behind, cut in this order: range (Spaghetti Code targets a random hero instead), then Git Revert. Everyone still keeps at least 3 mechanics.

NOTE: careful at the number of features of the game or we'll end up over engineering it.

NOTE (stretch goal, only after everything else works): implement targeted stripping. for this, the hero has to be at a certain angle from the boss to strip a certain topping.