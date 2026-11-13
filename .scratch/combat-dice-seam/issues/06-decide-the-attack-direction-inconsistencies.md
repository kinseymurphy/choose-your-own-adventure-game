# 06: Stop Monster Health Points at 0 and use one ASCII art rule

**What to build:** Two small changes in `Combat`, which tickets 01–05 deliberately left alone.

1. **A Monster's Health Points stop at 0**, as the player's already do. After the player's final hit, the `monster_health_now` line shows 0 instead of a negative number, for example "Smolderfang's health points are now 0." instead of "-6". Who wins doesn't change, because victory is still checked with ≤ 0.
2. **Both attack directions print a Weapon's ASCII art only when it isn't blank.** Today the player's art is always printed. Every Weapon has art, so players see no difference.

Nothing else in combat changes. Triage on 2026-11-13 set this scope and recorded the won't-fix decisions under `## Comments`.

**Blocked by:** None (tickets 01–05 are done on `feature/combat-dice-seam`)

**Status:** ready-for-agent

**Branch:** `fix/monster-health-floor`, from `feature/combat-dice-seam`

- [ ] In `Combat.PlayerAttacksMonsterSequence()`, the Monster's Health Points are reduced by the damage and stop at 0, as the player's do in `MonsterAttacksPlayerSequence()`.
- [ ] In `Combat.PlayerAttacksMonsterSequence()`, the player's Weapon art is printed only when it isn't blank, as the Monster's already is.
- [ ] No other change to combat: same rolls in the same order, same messages, same first round before the prompt and same mid-fight retreat (ADR 0002). `language_data.json` is unchanged.
- [ ] TDD: write each failing test before the code that makes it pass.
  - `CombatPlayerAttackTests` final-hit tests: Health Points 4 hit for 6 now leaves 0, and `monster_health_now` shows 0, for both Monsters in all four languages.
  - A new test: a player Weapon with blank art prints no art line, for both Monsters.
- [ ] **Transcripts.** These 5 are updated, and each must differ from its approved version only in the final `monster_health_now` line:
  - `dragon-victory` in English, Spanish, French and Italian: −6 becomes 0.
  - `magical-weapon-victory` in English: −1 becomes 0.

  The other 5 transcripts must not change.
- [ ] The full test suite passes.

## Comments

**Triage on 2026-11-13, decided by Kinsey:**

| # | Inconsistency | Decision | Reason |
|---|---|---|---|
| 1 | A Monster's Health Points can go below 0 | **Fix** (this ticket) | It's the only one players see. Who wins is unchanged, and the transcripts make the change exact and reviewable. |
| 2 | ASCII art always printed for the player, only when not blank for a Monster | **Fix** (this ticket, bundled with 1) | One line, no visible change, one rule for both directions. |
| 3 | "You have no weapon to attack with." is hard-coded in English | **Won't fix** | It can't happen in play: every Occupation gives a Weapon, and ADR 0003's load check rejects a Profile without one. Translating text no player can see would add 4 keys for nothing. `CombatPlayerAttackTests.NoWeapon_PrintsTheEnglishTextAndUsesNoRolls` still pins it, so it can't change by accident. Reopen it if a Player without a Weapon ever becomes possible. |
| 4 | Protection only reduces damage to the player | **Won't fix** | This is by design. Protection comes from Magical Armor, which only the Merchant sells, to the player (see `GLOSSARY.md`). |

**Moved out of this ticket:** Strength above 20 after character creation's d4 modifier, and a Halfling with 0 Health Points or less, aren't combat inconsistencies. The Halfling case is already a Known Limitation in `.scratch/player-profiles/spec.md`. Both belong with character-creation work (architecture review candidate 3) and will get their own needs-triage ticket when that work starts.

**Done on 2026-11-13 by Claude, on `fix/monster-health-floor`:**

- `dotnet test CYOATests` passed 289 of 289 tests. The suite was 287 before this ticket.
  - 2 tests were added: `WeaponWithBlankArt_PrintsNoArtLine` for each Monster.
  - The 8 final-hit tests were renamed `FinalHit_StopsMonsterHealthAtZeroAndShowsItsDefeatedNarrative` and now expect 0. Before the fix they failed with −2.
- **Production change:** two lines in `Combat.PlayerAttacksMonsterSequence()`.
  - The Monster's Health Points are now `Math.Max(0, HealthPoints - damage)`.
  - The player's art is printed only when it isn't blank.

  Rolls, messages, the first round before the prompt and the mid-fight retreat are unchanged.
- **Transcripts:**
  - **Updated (5):** each differs from its previous approved version in exactly one line, the final `monster_health_now`. In the 4 `dragon-victory` transcripts, −6 becomes 0. In `magical-weapon-victory` (English), −1 becomes 0.
  - **Unchanged (5):** the 4 `goblin-defeat` transcripts and `dragon-retreat` still pass with no edits.
- **No unrelated changes:** `language_data.json` is unchanged.
