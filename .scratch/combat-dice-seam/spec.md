# Spec: Deepen the combat round behind a dice seam

Status: ready-for-agent

## Problem Statement

The rules for one attack are written twice. `Player.Attack` resolves the player's attack on a Monster, and `Monster.Attack` resolves a Monster's attack on the player. Both roll to hit, roll the defender's defense, roll damage, update Health Points and build the message lines, and the two copies have already drifted apart. `Combat` only loops over them and prints their strings.

The dice can't be controlled. `Die` wraps `new Random()`, and `Player` creates its own private `Die`. Tests can only force a fight's outcome by giving characters extreme stats (Strength 0 always misses, Health Points 1000 always survives), by repeating a check many times, or by running a suite several times to show it isn't flaky. No test can say "the attack roll is 4, the defense roll is 15, the damage roll is 7" and check exactly what happens.

## Solution

`Combat` becomes the one module that knows how an attack works. It resolves both directions with one shared sequence: attack roll, defense roll, damage roll, Protection, Health Points. It then prints the same message lines as today. `Player.Attack` and `Monster.Attack` are removed, so `Player` and `Monster` go back to holding stats and message keys.

All combat dice go through a seam with two implementations: the existing random `Die` used by the game, and a scripted die used only by tests. It returns the rolls the test lists, in order. The game plays exactly as it does today, in every language, including today's known inconsistencies, which are written up as follow-up work rather than fixed here.

## User Stories

1. As a player, I want every fight to play exactly as it does today, so that the refactor changes nothing I can see.
2. As a player, I want every combat message in English, Spanish, French and Italian to stay word for word the same, so that no translation is lost or changed.
3. As a player, I want the first round to still happen before I'm asked to attack or retreat, so that combat feels the same (ADR 0002).
4. As a player, I want retreating mid-fight to still end the Session without a victory or defeat ending, so that combat feels the same (ADR 0002).
5. As a player with Magical Armor, I want my Protection to keep reducing each Monster hit and to keep seeing how much it absorbed, so that my purchase still works.
6. As a player with a Magical Weapon, I want its higher Max Damage to keep applying, so that my upgrade still works.
7. As a player, I want the dragon and the goblin to keep their own taunts, damage replies and defeat narratives, so that each Monster keeps its personality.
8. As a player, I want my Health Points to still stop at 0 when a Monster defeats me, so that the stats screen looks the same.
9. As a player, I want a Monster's Health Points to still show the same value after my final hit (it can go below 0 today), so that nothing changes until that is decided separately.
10. As a returning player, I want my saved Profile's Health Points never to be overwritten by combat damage, so that ADR 0003 still holds.
11. As a developer, I want one place that resolves an attack, so that a combat rule is fixed once for both directions.
12. As a developer, I want to script the exact dice rolls a test uses, so that I can check a hit, a block, a miss or a defeat directly.
13. As a developer, I want a scripted die that fails the test when it runs out of rolls, so that a test can't quietly fall back to random dice.
14. As a developer, I want a scripted die that fails the test when a roll is outside 1 to the number of sides, so that a test can't script an impossible roll.
15. As a developer, I want the Monster stat rolls to go through the same seam, so that a test can create a dragon or goblin with known stats.
16. As a developer, I want to start a whole game with scripted dice, so that a full Dead End fight can be tested end to end without rigging stats.
17. As a developer, I want full-fight transcripts in all four languages captured before the refactor, so that I can prove the output didn't change.
18. As a developer, I want the old tests for the removed methods replaced by tests through `Combat`, so that the suite tests the module the game actually uses.
19. As a developer, I want today's inconsistencies between the two attack directions written up as a follow-up ticket, so that they're decided on deliberately.
20. As a developer, I want every change written test-first (red, green, refactor), so that each behavior is covered before it moves.

## Implementation Decisions

**The dice seam**
- A one-method interface for dice, `Roll(sides)`, returning a whole number from 1 to `sides`. The name follows the existing `Die` class, for example `IDie`.
- `Die` implements it and stays the random implementation used by the game. Its behavior doesn't change.
- A scripted implementation lives only in `CYOATests`. It's built with a list of rolls and returns them in order.
  - When it runs out of rolls, it fails the test with a clear message.
  - When a scripted roll is outside 1 to `sides`, it fails the test with a clear message naming the roll and the sides.
- Rolls that go through the seam:
  - Every combat roll: attack, defense, damage, and picking a taunt or damage reply.
  - The Monster stat rolls in the `Dragon` and `Goblin` constructors, which already take the shared `Die`.
- Rolls that stay outside the seam: character creation's rolls in `Player`, which keeps its own private `Die` for them. They belong with a future prompt and character-creation change (architecture review candidate 3). `Player` stays a plain record that `System.Text.Json` can save (ADR 0003).

**Who gets the die**
- `Combat` receives the die in its constructor, as `Dragon` and `Goblin` do today.
- `Monster`, `Dragon` and `Goblin` take the interface instead of `Die`.
- `Game` takes an optional die in its constructor, defaulting to a new random `Die`. It passes the same die to the dragon, the goblin and every `Combat`. The existing `Game()` and `Game(ProfileStore?)` constructors keep working.
- `Program.cs` doesn't change, except where it has to pass the default.

**Combat (the deep module)**
- `Combat` owns the whole attack sequence for both directions. It's written once and given what differs:

  | | Player attacks Monster | Monster attacks Player |
  |---|---|---|
  | Hit chance | attack roll d20 ≤ player Strength | attack roll d20 ≤ Monster Strength |
  | Block chance | defense roll d20 ≤ Monster Agility | defense roll d20 ≤ player Agility |
  | Damage | d(player Weapon Max Damage) | d(Monster Weapon Max Damage), minus player Protection, not below 0 |
  | Health Points after a hit | not clamped (can go below 0) | clamped at 0 |
  | Intro line | `monster.PlayerAttackIntroKey` with translated Occupation and Weapon | `monster_attack_intro` |
  | ASCII art | always printed | printed only when not blank |
  | On block | `monster_defended_player_attack`, then a d(taunt count) roll picks the Monster's taunt (with translated Race) | `player_defended_monster_attack` |
  | On damage | `player_damage_dealt`, `monster_health_now`, then the defeated narrative if HP ≤ 0, otherwise a d(reply count) roll picks the damage reply | `armor_absorbed_damage` if Protection > 0, `monster_damage_dealt`, `player_health_now`, then `player_defeated_narrative` if HP ≤ 0 |
  | Player has no Weapon | hard-coded English "You have no weapon to attack with." and no rolls | n/a (Monsters always have a Weapon) |

- **Roll order (must hold):** attack d20, then defense d20 only on a hit, then damage only when not blocked. For the player's attack, a taunt or reply roll comes last when one is needed. A miss uses one roll and a block uses two. This is today's order.
- **Message lines and their order (must hold):** each attack prints exactly the lines `Player.Attack` or `Monster.Attack` return today, joined the same way and in the same order.
- **Its interface stays recognisable:**
  - The constructor takes the player, Monster, messages and die.
  - `StartCombat()`, `PlayerRetreated` and `GetCombatStatsDisplay()` keep their behavior.
  - `PlayerAttacksMonsterSequence()` and `MonsterAttacksPlayerSequence()` stay as the per-attack entry points, printing the same lines as today.
  - No new outcome type is added unless a ticket shows tests need it.
- **First round and retreat (must hold, ADR 0002):** the loop isn't changed. The first exchange happens before the prompt, and a mid-fight retreat prints `retreat_combat` and returns without an ending.

**Removed from Player and Monster**
- `Player`: `Attack`, `RollAttack`, `AttackHits`, `RollDamage`, `ReduceDamage`. `GetCombatStatLines` and `DisplayStats` stay. The private `Die` stays for character creation only.
- `Monster`: `Attack`, `RollAttack`, `AttackHits`, `RollDamage`, `GetRandomDefenseTaunt`, `GetRandomDamageReply`. Its taunt and damage-reply key lists become readable by `Combat`, and it keeps its message keys and `DisplayStats`.
- Anything moved keeps its exact behavior; only where it lives changes.

**Not changed**
- `language_data.json`: no keys added, removed or reworded, in any language.
- The Health Points rule from ADR 0003. `Game` still saves only after character creation and after the Merchant.
- `Merchant`, `Messages`, `Weapon`, `ProfileStore`, character creation and the Dead End handlers in `Game`, beyond passing the die.

## Testing Decisions

- **Red-green-refactor is required.** Each behavior is pinned or tested before the code it covers moves.
- **Characterization first.** Before any combat logic moves, tests capture whole-fight transcripts driven by the scripted die and scripted console input, in all four languages. The expected text is today's output, checked by hand once, then kept as literal strings. These transcripts must pass unchanged at every later step.
- **Seams under test:**
  1. **The scripted die**, tested directly: it returns rolls in order, fails when it runs out, and fails on a roll outside 1 to `sides`.
  2. **`Combat`**, through its constructor, `PlayerAttacksMonsterSequence()`, `MonsterAttacksPlayerSequence()` and `StartCombat()`, with the scripted die and redirected console under `[DoNotParallelize]`. Tests check printed text and the Health Points left on the player and Monster.
  3. **`Game.StartGame`**, end to end with a scripted die, for at least one full Dead End fight per Monster.
- **Combat cases (each direction, using scripted rolls):**
  - A miss.
  - A hit that is blocked, which for the player's attack shows a chosen taunt.
  - A hit that deals damage, which for the player's attack shows a chosen reply.
  - A hit that defeats the defender, showing the defeated narrative.
  - For the Monster's attack: Protection absorbing part of the damage, Protection absorbing all of it, and Health Points stopping at 0.
  - For the player's attack: a Magical Weapon's Max Damage used as the damage die's sides, and the Monster's Health Points going below 0 (preserved).
- **Replace, don't layer.** Old tests of the removed methods are deleted once tests through `Combat` cover the same behavior:
  - `PlayerTests` `ReduceDamage_*`.
  - The `Attack_*` and `GetRandom*` tests in `DragonTests` and `GoblinTests`.
  - The `PlayerAttack_*` tests in `GoblinTests`.

  Every deleted test's behavior must be named in the replacing ticket.
- **Kept as they are:** the Profile tests that rig stats (`ProfileGameTests`). They still pass and still check the Health Points rule. Moving them to scripted dice is optional and not part of this work.
- **Regression:** the full suite passes at the end of every ticket.

## Out of Scope

- Changing any inconsistency between the two attack directions. Each is listed in the follow-up ticket:
  - Monster Health Points below 0.
  - ASCII art printed only conditionally for Monsters.
  - The hard-coded English "no weapon" text.
- Changing the first-round-before-prompt or mid-fight-retreat behavior (ADR 0002).
- Character creation's rolls, and Strength above 20 after the d4 modifier.
- Combining the two Dead End handlers in `Game` (architecture review candidate 2).
- Prompt parsing and input handling (architecture review candidate 3).
- Message, translation or language-data changes (architecture review candidate 4).

## Further Notes

- ADR 0004 records the dice seam and moving the attack rules into `Combat`.
- Architecture review, 2026-11-13: this was candidate 1. The review called the negative Monster Health Points something this change would fix. The decision during grilling was to preserve it and decide separately.
- Work happens on `feature/combat-dice-seam`, branched from `feature/player-profiles`, because the Profile tests depend on today's combat.
