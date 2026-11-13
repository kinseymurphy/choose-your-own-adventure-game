# ADR 0002: Share combat between Monsters through a Monster base class

- **Status:** Proposed
- **Date:** 2026-11-13
- **Deciders:** Kinsey Murphy
- **Branch / commit:** `feature/goblin-encounter` / not yet implemented

## Context

The game is adding a second fight: the **Goblin Encounter** with Grubnik in the Murky Swamp
on the West path. It should reuse the combat the **Dragon Encounter** already has.

Combat only works with the dragon today:

- `Combat` takes a `Player` and a `Dragon`.
- `Player.Attack` takes a `Dragon`.
- The shared combat message keys mention the dragon, for example `player_missed_dragon`,
  `dragon_health_now`, `dragon_attack_intro` and `combat_dragon_header`.
- Some of the text says "the dragon" directly, for example `player_attack_intro`.

## Decision

Add an abstract **`Monster`** base class. It holds the parts every Monster has: Name,
Strength, Agility, HealthPoints, Weapon, `Attack(Player, Messages)`, the defense taunts and
damage replies, and `DisplayStats`. `Dragon` and the new `Goblin` both inherit from it.

- `Combat` and `Player.Attack` take a `Monster`, not a `Dragon`.
- The shared combat message keys get Monster names (for example `player_missed_monster`
  and `monster_health_now`) in all four languages. The text uses `{0}` for the Monster's
  name.
- Each Monster keeps its own keys for text that really is about that Monster: the intro,
  stats header, taunts, damage replies, defeated narrative and endings.
- The dragon's own text, personality and stats don't change.

## Options considered

| Option | Result |
|---|---|
| **Monster base class (chosen)** | One combat system for every Monster. Adding a third Monster means adding one subclass and its message keys. |
| `Goblin` inherits from `Dragon` | Smallest change, but it says a goblin *is a* dragon, which is false and would confuse future readers. Rejected. |
| A separate goblin combat class | No refactoring, but it doubles the combat code, so every combat fix has to be made twice. Rejected because it goes against the goal of reusing combat. |
| Keep the `dragon_*` key names and only make the text generic | Fewer changes to the language file, but goblin combat would run on keys named after the dragon. Rejected. |

## Consequences

**Positive**

- Grubnik uses the same combat, Protection and Magical Weapon rules as the dragon,
  with no extra work.
- Future Monsters only need a subclass and message keys.

**Negative / risks**

- Renaming the shared combat keys changes all four language sections and the existing
  required-key tests.
- The existing dragon and combat tests have to keep passing after the change. This
  confirms the dragon behaves exactly as before.
- Existing combat quirks are now shared by every Monster: the mid-combat retreat skips the
  ending, and the first round runs before the player is prompted. Fixing them stays out of
  scope.
