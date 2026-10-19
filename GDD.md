# ScopeCreep GDD (Team 5)

## Core Concept

A small team of devs (2-3) try to build a game. What started as a simple idea quickly became overwhelming and started to actively battle the devs who try to get it back under control by removing some of the layers otherwise the game will never ship.

The characters (heroes) are the devs, each with their unique skill set. The boss they battle is the bloated game personified as the Ramen Bowl, the perfect game that has become a monster. It started as a simple broth and now is buried in toping that nobody asked for / wants.

## Theme

2D pixel art, turn based.

## Character Ideas

**The Coder** with abilities:
  - Refactor:   deals damage and strips a topping
  - Hotfix:     quick heal and cleans up debuffs
  - Git Revert: undoes the last boss action (time reverse)
  
**The Designer** with abilities:
  - Wireframe:      shows the boss's next attack / weak points (if a targeted attack in a specific region on the boss character is implemented)
  - Pixel Perfect:  big damage attach but higher chance of the boss dodging it (miss)
  - Negative Space: little damage but removes all/part of a topping off the boss
  
**The Manager** with abilities:
  - Meeting Time: the boss is stunned and skips the next attack, but the manager is exhausted and skips next round
  - MVP:          1-turn shield for them or all other players in the party
  - Ship It:      damage + knockback. if the boss is stripped to the broth it acts like a finisher, otherwise it has a chance of stripping a topping
  - Delegate:     five another action to an ally this turn
  
NOTE: the Manager should probably lose one ability so it is balanced with the other players. they can also have more abilities and have them be waker (its funny because the manager does a bit of everything and takes up credit later).
  
**The Ramen Bowl** = multi-phase boss, where each phase maps to one/two layers. Abilities:
  - Extra Toppings: summons add-ons as individual units on the battlefield. this might be too much for a 5-min game so it can be swapped to the boss adding a topping back
  - Spaghetti Code: binds or locks up abilities of the affected characters. needs to be closer to the characters to engage.
  - Scalding Broth: splash area damage, with extra damage on grappled characters
  
NOTE: because of the multi-layered boss, just dealing damage should not be the way to beat it. this probably requires a healing mechanic for the boss too.

NOTE: careful at the number of features of the game or we'll end up over engineering it.

NOTE: how do we add cost to the abilities?
  - cost: each ability requires an amount of shared resources.
  - cooldown
