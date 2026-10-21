# ScopeCreep Coding Style

These are guidelines, not iron-clad rules. The goal is to tell classes, methods, properties and constants apart at a glance. If a rule gets in the way, talk about it and change it here.

Naming is enforced (as warnings) by the `.editorconfig` at the repo root. Rules marked *(review)* can't be checked automatically and are up to code review.

## Naming

| What | Style | Example |
| ---- | ----- | ------- |
| Compile-time constants (`const`) | `SCREAMING_SNAKE_CASE` | `SP_CAP`, `SP_REGEN` |
| Enum members | `SCREAMING_SNAKE_CASE` | `Element.FIRE`, `BattleStatus.IN_PROGRESS` |
| Variables, parameters, fields, properties, events | `snake_case` | `spirit_points`, `max_hp`, `state_changed` |
| Methods and local functions | `camelCase` | `dealDamageToBoss`, `gainSpiritPoints` |
| Classes, records, structs, enums, delegates, namespaces | `PascalCase` | `Combatant`, `BattleState`, `ScopeCreep.Core` |
| Interfaces | `I_` + `PascalCase` | `I_HitResolver`, `I_BossBrain` |
| Generic type parameters | `Type` + one letter | `TypeT`, `TypeK` |

- **Booleans** start with `is`, `has`, `can` or `should`: `is_down`, `has_acted`, `can_advance_phase`, `is_hero_phase_over`. *(review)*
- **Methods that return a boolean** follow the same rule: `canUse(...)`. *(review)*
- **Unused or ignored parameters** are named `_`. If there are two, `_` and `__`.
- **Acronyms are regular words**: lowercase at the start of a variable or method, capitalized like a word in a class name. So `UrlHttpId`, `id_name`, `idByName`, `sp_label`.
- **Lambda parameters** in short LINQ calls are named `it`: `heroes.All(it => it.is_down)`.

### Exceptions (names we don't control)

- Godot overrides keep Godot's names: `_Ready`, `_Process`, `_UnhandledInput`.
- .NET overrides keep .NET's names: `ToString`, `Equals`, `GetHashCode`.
- Godot and .NET APIs are PascalCase (`AddChild`, `QueueFree`, `GetTree`), so engine calls will look different from our methods.
- Godot `[Signal]` delegates must end in `EventHandler`. Prefer plain C# events (`event Action<...>`) so this rarely comes up.

## Formatting

- **File-scoped namespaces** (`namespace ScopeCreep.Core.model;`).
- **Three blank lines** between the `using`/`namespace` block and the first type.
- **Two blank lines** between members (methods, nested types, groups of fields).
- **Allman braces**: the opening brace goes on its own line.
- **Short guard clauses** can stay on one line: `if (hero.is_down) return false;`.
- **One-line members** use expression bodies: `public bool is_down => hp <= 0;`.
- **Align** the types, names and `=` in groups of related declarations:
  ```csharp
  public int          spirit_points = 3;
  public int          round         = 1;
  public BattleStatus status        = BattleStatus.IN_PROGRESS;
  ```
- **`this.`** is used in constructors when a parameter has the same name as the member it sets.
- **Doc comments** use `/** ... */` above types and methods, in plain sentences.

## Project layout

```
core/       game rules, no Godot references
  enums/      plain enums (Element, BossPhase, BattleStatus, ...)
  model/      domain types (Combatant, Ability, BattleState)
  seams/      interfaces + their stub implementations
  Battle.cs   the turn loop
tests/      xUnit tests for core, run with `dotnet test`
game/       the Godot project: scenes and UI scripts only
docs/       this file and other docs
GDD.md      game design document
```

- `core` must never reference Godot. Anything that needs the engine goes in `game`.
- Each person works in their own folder where possible, to avoid merge conflicts.

## Type design

The rules below follow the "make illegal states unrepresentable" approach.

- **Closed hierarchies** for things with a fixed set of variants that have different data or rules: an `abstract` base with a `private` constructor, and `sealed` nested variants. All variants live in **one file**, so the full list is visible in one place. Example: `Ability` → `Offensive` / `Support` / `Self` → `Firewall`, `Hotfix`, ...
  - Behavior goes in an `abstract` method on the branch, so every variant **must** implement it (the compiler checks this, unlike a `switch`).
  - Don't use `partial` on closed hierarchies; it would let other files add variants.
- **Stop splitting** when variants only differ in values. Use one type plus static factory methods: `Combatant.Hero.coder()`, `Combatant.Hero.designer()`.
- **Enums** when the variants carry no data (`BossPhase`, `BattleStatus`). Switch to a closed hierarchy only once a variant needs its own data.
- **Values are records** (`Ability`, `BossAttack`): equal when their data is equal, immutable.
- **Entities are classes** (`Combatant`, `Battle`): they have an identity and state that changes over time.
- **One bool or one state type, never two bools** that can contradict each other: `BattleStatus` instead of `bool won` + `bool lost`.
- **Wrap a primitive** only when it has a rule you'd otherwise check in more than one place (for example a future `SpiritPoints` type that stays between 0 and the cap).
- **Use `switch` expressions** with `_ => throw new UnreachableException()` for the impossible case, never a ternary that silently picks a default.

## Encapsulation

- The UI (`game`) may **read** the battle state but not **change** it directly; every change goes through `Battle`.
  - State the UI shouldn't set uses `internal set` or `private set` (`has_acted`, `hp`).
  - Methods only `Battle` should call are `internal` (`takeDamage`, `heal`).
  - Collections are exposed as `IReadOnlyList<...>`.
- `tests` can see `internal` members through `InternalsVisibleTo` in `core/ScopeCreep.Core.csproj`.
- Godot `[Export]` fields are `private` and initialized with `= null!`; they are assigned in the editor.

## Errors

- **Bad input from the player** (an action that isn't allowed right now): disable it in the UI, and have the rule check (`canUse`) return `false`.
- **A broken rule in our own code** (calling `useSelfAbility` when `canUse` is `false`): throw `InvalidOperationException`.
- **An impossible case** (a new hierarchy branch nobody handled): throw `UnreachableException`.
