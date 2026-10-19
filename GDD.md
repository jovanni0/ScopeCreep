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

NOTE: careful at the number of features of the game or we'll end up over engineering it.

NOTE (stretch goal, only after everything else works): implement targeted stripping. for this, the hero has to be at a certain angle from the boss to strip a certain topping.