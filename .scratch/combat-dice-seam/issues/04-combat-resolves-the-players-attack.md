# 04: Combat resolves the player's attack with the same sequence

**What to build:** Move the player's attack on a Monster out of `Player.Attack` and into `Combat`, through the same shared sequence ticket 03 introduced. Picking a taunt or damage reply also moves into `Combat` and uses the die. `PlayerAttacksMonsterSequence()` prints exactly the same lines as today, including today's inconsistencies:
- the Monster's Health Points aren't clamped;
- ASCII art is always printed;
- the English "no weapon" text is kept.

Then remove `Player.Attack`, `RollAttack`, `AttackHits` and `RollDamage`, and `Monster.GetRandomDefenseTaunt` and `GetRandomDamageReply`. After this ticket, `Combat` is the only place an attack is resolved. See the "Player attacks Monster" column of the combat table in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 03 (Combat resolves the Monster's attack)

**Status:** ready-for-agent

- [ ] The player's attack uses the same shared sequence as the Monster's attack. The direction only supplies:
  - player Strength;
  - Monster Agility;
  - player Weapon Max Damage;
  - Protection 0;
  - no Health Points clamp.
- [ ] Picking a taunt or damage reply uses a roll from the die after the attack sequence's rolls, as today. The Monster's taunt and damage-reply key lists are readable by `Combat`.
- [ ] The printed lines and their order are unchanged:
  1. `monster.PlayerAttackIntroKey` with translated Occupation and Weapon.
  2. The ASCII art, always.
  3. The miss line, or the hit and block lines with a taunt (translated Race), or `player_damage_dealt` and `monster_health_now`.
  4. After damage, the defeated narrative at 0 or below, otherwise a damage reply.
- [ ] With no Weapon, the player's attack prints the same English "You have no weapon to attack with." text and uses no rolls.
- [ ] Removed:
  - `Player.Attack`, `Player.RollAttack`, `Player.AttackHits` and `Player.RollDamage`.
  - `Monster.GetRandomDefenseTaunt` and `Monster.GetRandomDamageReply`.
- [ ] `Player`'s private `Die` is used only by character creation.
- [ ] The ticket 02 transcripts pass unchanged.
- [ ] TDD: write each failing test through `PlayerAttacksMonsterSequence()` with the scripted die before moving the code it covers.
  - A miss leaves the Monster's Health Points unchanged and uses one roll.
  - A block shows the scripted taunt for each Monster. For example, with taunt roll 2 the dragon shows `dragon_defense_taunt_2` and Grubnik shows `goblin_defense_taunt_2`, with the translated Race filled in.
  - A damaging hit shows the scripted damage reply and reduces the Monster's Health Points by the damage roll.
  - A Magical Weapon's Max Damage is the sides of the damage roll. The scripted die rejects a roll above it.
  - A final hit can leave the Monster's Health Points below 0 (preserved), and shows its defeated narrative.
  - Each Monster's own intro key is used, checked in all four languages.
- [ ] Replace, don't layer. Delete the tests these cover, and list each deleted test under `## Comments`:
  - The `PlayerAttack_*` tests in `GoblinTests`.
  - The `GetRandomDefenseTaunt_*` and `GetRandomDamageReply_*` tests in `DragonTests` and `GoblinTests`.
- [ ] The full test suite passes.

## Comments

**Done on 2026-11-13 by Claude:**

- `dotnet test CYOATests` passed 285 of 285 tests. The suite was 267 before this ticket: 38 tests were added and 20 deleted.
- All 10 ticket 02 transcripts pass unchanged. `language_data.json` and the approved transcripts aren't touched.
- **The player's attack.** `PlayerAttacksMonsterSequence()` now goes through the same `ResolveAttack` as the Monster's attack, with Protection 0. It prints the same lines in the same order, and keeps today's inconsistencies (ticket 06):
  - the ASCII art is always printed;
  - the Monster's Health Points aren't stopped at 0;
  - with no Weapon, it prints the English "You have no weapon to attack with." and uses no rolls.
- **Taunts and replies.** `Combat.PickKey` picks a taunt or damage reply with a d(count) roll after the attack's rolls, as before. To make that possible, `Monster.DefenseTauntKeys` and `DamageReplyKeys` changed from `protected` to `public`.
- **Removed:**
  - `Player.Attack`, `RollAttack`, `AttackHits` and `RollDamage`.
  - `Monster.GetRandomDefenseTaunt` and `GetRandomDamageReply`.

  `Player`'s private `Die` is now used only by character creation, and `Combat` is the only place an attack is resolved.
- **Ticket 02's temporary reflection is deleted** (`RollPlayerAttacksWith` and `ScriptedRandom`). The transcripts pass with exactly the same roll scripts, which confirms they never depended on it.
- **Added: 38 tests in `CYOATests/CombatPlayerAttackTests.cs`.** They use scripted rolls through `Combat`'s die and check every roll is used.

  | Test | Monsters × languages | Checks |
  |---|---|---|
  | Miss | both, English | 1 roll; Monster Health Points unchanged |
  | Intro and weapon art | both × 4 languages | each Monster's own intro key, with the translated Occupation and Weapon, then the art |
  | Block | both × 4 languages | `monster_defended_player_attack` and taunt 2 with the translated Race |
  | Damaging hit | both × 4 languages | damage 6, Health Points 20 → 14, reply 3 last |
  | Final hit | both × 4 languages | Health Points 4 → −2, the defeated narrative last, no reply roll |
  | Magical Weapon | both, English | a d10 accepts 10 and rejects 11 |
  | No Weapon | both, Spanish | exactly the English text, no rolls |

  Written first, 36 failed for the right reason: the player's rolls came from `Player`'s own die, and the taunt and reply picks ran out on the Monster's die. The 2 "No Weapon" tests passed from the start, because they pin behavior that doesn't change.
- **Deleted, and what now covers each:**

  | Deleted test | Now covered by |
  |---|---|
  | `GoblinTests.PlayerAttack_OnGoblin_UsesGoblinAttackIntro` (×4 languages) | `EachMonster_UsesItsOwnAttackIntroAndTheWeaponArt` (both Monsters × 4) |
  | `GoblinTests.PlayerAttack_BlockedByGoblin_ShowsGoblinTaunt` (×4) | `Block_ShowsTheScriptedTauntWithTheTranslatedRace` |
  | `GoblinTests.PlayerAttack_HurtsGoblin_ShowsGoblinDamageReply` (×4) | `DamagingHit_ReducesMonsterHealthAndShowsTheScriptedReply` |
  | `GoblinTests.PlayerAttack_DefeatsGoblin_ShowsGoblinDefeatedNarrative` (×4) | `FinalHit_LeavesMonsterHealthBelowZeroAndShowsItsDefeatedNarrative` |
  | `DragonTests.GetRandomDefenseTaunt_ReturnsExpectedTaunt`, `GoblinTests.GetRandomDefenseTaunt_ReturnsGoblinTaunt` | the Block tests, which check the exact scripted taunt for each Monster |
  | `DragonTests.GetRandomDamageReply_ReturnsExpectedReply`, `GoblinTests.GetRandomDamageReply_ReturnsGoblinReply` | the Damaging hit tests, which check the exact scripted reply for each Monster |

- **Kept:** `using System.Reflection` in `DragonTests` and `GoblinTests` is still used by the existing `HandleDragonEncounter`/`HandleGoblinEncounter` tests. Those belong to architecture review candidate 2, not this ticket.
