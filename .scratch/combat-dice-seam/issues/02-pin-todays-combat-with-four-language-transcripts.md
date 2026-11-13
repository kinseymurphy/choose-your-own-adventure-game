# 02: Pin today's combat with four-language fight transcripts

**What to build:** Characterization tests that capture whole fights exactly as they play today, before any combat logic moves. Each test drives `Combat.StartCombat()` with the scripted die from ticket 01 and scripted console input, and compares the complete printed output with a literal expected transcript. The transcripts cover both Monsters and all four languages, and they include today's inconsistencies. These tests must pass unchanged at the end of every later ticket. See "Characterization first" in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 01 (Script the dice through a dice seam)

**Status:** ready-for-agent

- [ ] Transcript tests run `Combat.StartCombat()` with a scripted die and redirected console input and output, restored in cleanup, under `[DoNotParallelize]`.
- [ ] Each expected transcript is a literal string. It's produced from today's code, checked by hand once against the rules in the spec's combat table, and then never regenerated.
- [ ] Together, the transcripts cover at least:
  - **A Player victory over the dragon in English.** It includes a miss, a block with a taunt, a hit with a damage reply, and a final hit that leaves the dragon's Health Points below 0.
  - **A Player defeat by Grubnik in English.** It includes a Monster miss, a block by the player, Protection 3 absorbing part of one hit and all of another, and Health Points stopping at 0 with the defeated narrative.
  - **A mid-fight retreat.** The first round happens before the prompt, then the player types `r`. It shows `retreat_combat` and no ending.
  - **A Player with a Magical Weapon.** The damage roll uses the higher Max Damage as its sides.
  - **The same victory and defeat scripts in Spanish, French and Italian.**
- [ ] Every transcript test fails if any line, its order or its wording changes.
- [ ] No production code changes in this ticket.
- [ ] The full test suite passes.

## Comments

**Done on 2026-11-13 by Claude:**

- `dotnet test CYOATests` passed 264 of 264 tests. The suite was 254 before this ticket, so it adds the 10 transcript tests in `CombatTranscriptTests`. No production code changed.
- **The 10 approved transcripts** are in `CYOATests/CombatTranscripts/`:

  | Transcript | Languages | What it pins |
  |---|---|---|
  | `dragon-victory` | en, es, fr, it | A miss from each side; a block with a taunt; a Monster hit; a hit with a damage reply; a block by the player; the final hit leaving the dragon at −6 |
  | `goblin-defeat` | en, es, fr, it | A Monster miss; a block by the player; Protection 3 absorbing all of one hit and part of three; Health Points stopped at 0 from −1; the defeated narrative |
  | `dragon-retreat` | en | The first round before the prompt; an invalid answer playing another round; `r` showing `retreat_combat` with no ending |
  | `magical-weapon-victory` | en | A damage roll of 16, possible only on the Magical Weapon's d16 |

- **How the transcripts were reviewed:**
  - I checked them by hand against the scripted rolls.
  - A script then confirmed that each one contains every expected message in its language, built from `language_data.json` with the same placeholders the combat code uses: 18/18, 18/18, 9/9 and 7/7.
  - The same script confirmed that lines which must not appear are absent and that there are no `[key]` placeholders.
  - Kinsey reviewed and approved the transcripts on 2026-11-13.
- **Existing text kept as it is:** "As a Archer" and the untranslated `<<< claws >>>` were not changed.
- **Decisions approved by Kinsey:**
  - **Expected text in files.** The transcripts are `.approved.txt` files rather than strings in the test code. On a mismatch, the test writes `<name>.received.txt` next to the copied files in `bin/.../CombatTranscripts/`. The approved files must never be regenerated to make a test pass.
  - **Temporary reflection.** The player's attack still rolls on `Player`'s private `Die`. Until ticket 04, `RollPlayerAttacksWith` uses reflection to point that die's `Random` at the same scripted die. Every roll shares one queue, so the scripts should stay valid after tickets 03–04. Ticket 04 must delete `RollPlayerAttacksWith` and `ScriptedRandom`.
- **Script check:** each test also checks that every scripted roll was used.
- **Ending messages:** the victory and defeat endings aren't in these transcripts, because `Game` prints them, not `Combat`. Ticket 05's whole-game tests cover them.
