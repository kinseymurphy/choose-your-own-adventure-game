# 03: Combat resolves the Monster's attack

**What to build:** Move the Monster's attack on the player out of `Monster.Attack` and into `Combat`, written as the shared attack sequence that ticket 04 will reuse for the player's attack. `MonsterAttacksPlayerSequence()` prints exactly the same lines as today, using the die from ticket 01. Then remove `Monster.Attack`, `RollAttack`, `AttackHits` and `RollDamage`, and `Player.ReduceDamage`, whose Protection rule moves into `Combat`. See the "Monster attacks Player" column of the combat table in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 02 (Pin today's combat with four-language fight transcripts)

**Status:** ready-for-agent

- [ ] `Combat` resolves the Monster's attack with one sequence:
  1. Attack d20 ≤ Monster Strength to hit.
  2. On a hit, defense d20 ≤ player Agility to block.
  3. When not blocked, damage d(Monster Weapon Max Damage), minus player Protection, not below 0.
  4. The player's Health Points are reduced and stop at 0.
- [ ] The sequence is written so the player's attack can reuse it in ticket 04: the attacker's Strength, the defender's Agility, the damage sides and the Protection to subtract are supplied to it, not hard-coded.
- [ ] The printed lines and their order are unchanged: `monster_attack_intro`, ASCII art only when not blank, then the miss, block or damage lines, `armor_absorbed_damage` when Protection > 0, `player_health_now`, and `player_defeated_narrative` at 0.
- [ ] Roll order is unchanged: one roll on a miss, two on a block, three on damage.
- [ ] Removed:
  - `Monster.Attack`, `Monster.RollAttack`, `Monster.AttackHits` and `Monster.RollDamage`.
  - `Player.ReduceDamage`.
- [ ] The ticket 02 transcripts pass unchanged.
- [ ] TDD: write each failing test through `MonsterAttacksPlayerSequence()` with the scripted die before moving the code it covers.
  - A miss (roll above Monster Strength) leaves the player's Health Points unchanged.
  - A block (hit, then defense roll ≤ player Agility) leaves them unchanged and shows `player_defended_monster_attack`.
  - A damaging hit reduces them by the damage roll.
  - Protection 3 with damage roll 7 reduces them by 4 and shows 3 absorbed. With damage roll 2, they're reduced by 0 and it shows 2 absorbed.
  - A hit for more than the player's Health Points leaves 0 and shows `player_defeated_narrative`.
- [ ] Replace, don't layer. Delete the tests these cover, and list each deleted test under `## Comments`:
  - The `ReduceDamage_*` tests in `PlayerTests`.
  - The `Attack_*` tests in `DragonTests`.
  - The `Attack_*` tests in `GoblinTests`, including `Attack_PlayerWithMagicalArmor_AbsorbsGoblinDamage`.
- [ ] The full test suite passes.

## Comments

**Done on 2026-11-13 by Claude:**

- `dotnet test CYOATests` passed 267 of 267 tests. The suite was 264 before this ticket: 14 tests were added and 11 deleted.
- All 10 ticket 02 transcripts pass unchanged. `language_data.json` and the approved transcripts aren't touched.
- **The shared sequence.** `Combat.ResolveAttack(attackerStrength, defenderAgility, damageSides, protection)` returns an `AttackResult` (hit, blocked, damage roll, damage after Protection). It rolls attack d20, then defense d20 only on a hit, then damage only when not blocked, as today. It's private, and ticket 04 will reuse it for the player's attack.
- **The Monster's attack.** `MonsterAttacksPlayerSequence()` builds the same lines in the same order from that result:
  - ASCII art only when it isn't blank;
  - the player's Health Points stop at 0;
  - `armor_absorbed_damage` when Protection > 0.
- **Removed:**
  - `Monster.Attack`, `Monster.RollAttack`, `Monster.AttackHits` and `Monster.RollDamage`.
  - `Player.ReduceDamage`.
  - `Monster.GetRandomDefenseTaunt` and `GetRandomDamageReply` stay until ticket 04.
- **Added: 14 tests in `CYOATests/CombatMonsterAttackTests.cs`.** Each runs for both the dragon and Grubnik, with scripted rolls through `Combat`'s die, and checks every scripted roll is used:
  - a miss (1 roll);
  - a block (2 rolls);
  - an unblocked hit for 5;
  - Protection 3 against damage rolls of 6, 3 and 2, giving damage 3, 0 and 0 with 3, 3 and 2 absorbed;
  - a hit for more than the player's Health Points, which stops at 0 and shows the defeated narrative.

  Written first, they failed because `Monster.Attack` rolled the Monster's own die (it ran out), not `Combat`'s.
- **Deleted, and what now covers each:**

  | Deleted test | Now covered by |
  |---|---|
  | `PlayerTests.ReduceDamage_WithNoProtection_ReturnsDamageUnchanged` | `UnblockedHit_ReducesHealthPointsByTheDamageRoll` |
  | `PlayerTests.ReduceDamage_WithDamageGreaterThanProtection_ReturnsReducedDamage` | `Protection_IsSubtractedFromTheDamageRollButNeverBelowZero` (6 → 3) |
  | `PlayerTests.ReduceDamage_WithDamageEqualToProtection_ReturnsZero` | same test (3 → 0) |
  | `PlayerTests.ReduceDamage_WithDamageLessThanProtection_ReturnsZeroNotNegative` | same test (2 → 0) |
  | `DragonTests.Attack_ReturnsNonEmptyResultString`, `GoblinTests.Attack_ReturnsNonEmptyResultString` | every `CombatMonsterAttackTests` case, which checks exact lines, for both Monsters |
  | `DragonTests.Attack_DoesNotIncreasePlayerHealth`, `GoblinTests.Attack_DoesNotIncreasePlayerHealth` | the miss, block, hit and Protection cases, which check exact Health Points |
  | `DragonTests.Attack_KeepsPlayerHealthNonNegative`, `GoblinTests.Attack_KeepsPlayerHealthNonNegative` | `HitForMoreThanThePlayersHealthPoints_StopsAtZeroAndDefeatsThePlayer` |
  | `GoblinTests.Attack_PlayerWithMagicalArmor_AbsorbsGoblinDamage` | the Protection cases for the goblin, plus the `goblin-defeat` transcripts |
