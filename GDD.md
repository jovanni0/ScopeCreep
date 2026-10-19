# ScopeCreep GDD (Team 5)

## Core Concept

A small team of 3 devs try to build a game. What started as a simple idea quickly became overwhelming and started to actively battle the devs who try to get it back under control by removing some of the layers otherwise the game will never ship.

The characters (heroes) are the devs, each with their unique skill set. The boss they battle is the bloated game personified as the Ramen Bowl, the perfect game that has become a monster. It started as a simple broth and now is buried in toping that nobody asked for / wants.

## Theme

2D pixel art, turn based.

## Character Ideas

### Heroes

**The Coder** with abilities:
  - *Refactor*:   deals damage and strips a topping
  - *Hotfix*:     quick heal and cleans up debuffs (Spaghetti Code's stun).
  - *Git Revert*: undoes the last boss action (time reverse)

**The Designer** with abilities:
  - *Wireframe*:      shows the boss's next attack
  - *Pixel Perfect*:  big damage attack but higher chance of the boss dodging it (miss)
  - *Negative Space*: little damage but removes all/part of a topping off the boss

**The Manager** with abilities:
  - *Meeting Time*: the boss is stunned and skips the next attack, but the manager is exhausted and skips next round
  - *MVP*:          1-turn shield for them or all other players in the party
  - *Ship It*:      damage + knockback. if the boss is stripped to the broth it acts like a finisher, otherwise it has a chance of stripping a topping. 
                    the boss cannot charge again in the next round.

All heroes have the ability *Coffee Break*, which allows them to skip their turn and gain +2SP to the pool.

### Boss

**The Ramen Bowl** = with abilities:
  - *Extra Toppings*: summons a topping back and get +20% HP (healing).
  - *Spaghetti Code*: stuns a character. the stunned character cannot use any ability for it's next turn. the boss needs to be in close range (Charge) to use the attack.
  - *Scalding Broth*: splash area damage, with extra damage on stunned characters.
  - *Charge*:         bonus action, can move in close range of one character.

There are 2 ways to beat the boss: deal a lot of damage or strip the toppings off of it and finish it. The boss' HP is scaled by the number of toppings it has, where each topping adds +20% of the base HP. In other words, stripping a topping deals 20% worth of damage, so it's way more effective to strip it then just keep damaging it. Once a turn, the boss can use it's action to add a new topping (Extra Topping), so just dealing damage is discouraged just because you may not be able to deal enough to keep up with the speed it heals.

## Economy

The heroes have a shared pool of **Spirit Points**. each ability of each character costs a certain amount of points, and the player needs to be careful not to empty the pool (reach Burnout). if there are no points in the pool the characters cannot do anything and skip their turn.

The pool caps at a certain number of points (ex 8SP). each round the pool regains +4SP.

| Ability        | Cost       | Cooldown (turns)                    |
| -------------- | ---------- | ----------------------------------- |
| Refactor       | 2          | –                                   |
| Hotfix         | 2          | 1                                   |
| Git Revert     | 3          | 3                                   |
| Wireframe      | 1          | –                                   |
| Pixel Perfect  | 3          | –                                   |
| Negative Space | 2          | –                                   |
| Meeting Time   | 3          | 3 (the exhaustion is an extra cost) |
| MVP            | 2          | 2                                   |
| Ship It        | 2          | –                                   |
| Coffee Break   | 0, gain +2 | –                                   |


NOTE: careful at the number of features of the game or we'll end up over engineering it.

NOTE: implement targeted stripping. for this, the hero has to be at a certain angle from the boss to strip a certain topping. (idea if extra mechanics need to be implemented.)