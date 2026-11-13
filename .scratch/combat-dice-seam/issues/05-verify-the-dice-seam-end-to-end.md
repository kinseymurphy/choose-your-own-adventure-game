# 05: Verify the dice seam end to end and accept ADR 0004

**What to build:** Confirm the refactor changed nothing a player can see, record the evidence, and accept ADR 0004. This ticket adds no new behavior. It adds whole-game tests with scripted dice, runs scripted playthroughs of the real program, fixes only small defects it uncovers (anything bigger is reported as a new ticket), and reports any check that still needs a person. See "Testing Decisions" in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 04 (Combat resolves the player's attack with the same sequence)

**Status:** ready-for-agent

- [ ] Whole-game tests with scripted dice, through `Game.StartGame` with redirected console:
  - **North:** the player defeats the dragon. The output ends with the dragon victory ending.
  - **West:** Grubnik defeats the player. The output ends with the goblin defeat ending.
  - **Both:** a saved Profile's Health Points are unchanged afterwards (ADR 0003).
- [ ] `dotnet test CYOATests` passes. Record the pass count and how many tests were added, replaced and deleted.
- [ ] The ticket 02 transcripts still pass with no changes to their expected text. Confirm with `git diff` that their expected strings are unchanged since ticket 02.
- [ ] `language_data.json` is unchanged on this branch (`git diff feature/player-profiles -- language_data.json` is empty).
- [ ] Scripted playthroughs of the real program, using a temporary or backed-up profile folder so no real player data changes:
  - A North fight and a West fight in English.
  - One fight in one other language.
- [ ] ADR 0004's status is changed from Proposed to Accepted, with the branch and commit filled in and its Verification section completed.
- [ ] ADR 0002's known combat quirks are confirmed unchanged: the first round runs before the prompt, and a mid-fight retreat skips the ending.
- [ ] Under `## Comments`, report what was verified, any defects found and fixed, and any manual check that still needs a person.

## Comments

**Verified on 2026-11-13 by Claude:**

- **`dotnet test CYOATests`:** 287 passed, 0 failed, no warnings.

  | Ticket | Added | Deleted | Suite after |
  |---|---|---|---|
  | 01 | 8 | 0 | 254 |
  | 02 | 10 | 0 | 264 |
  | 03 | 14 | 11 | 267 |
  | 04 | 38 | 20 | 285 |
  | 05 | 2 | 0 | 287 |
  | **Total** | **72** | **31** | from 246 to 287 |

  The 31 deleted tests were replaced by tests through `Combat`. Tickets 03 and 04 list what covers each one.
- **Whole-game Dead Ends with scripted dice:** two tests were added to `GameDiceTests`. They play through `Game.StartGame` with a saved Profile in a temporary folder.
  - **North:** Bob is wounded (30 to 24 Health Points), then defeats Smolderfang. The output ends with the `victory` ending.
  - **West:** Grubnik defeats Ana (Health Points stopped at 0). The output ends with the `goblin_defeat` ending.
  - **Both:** every scripted roll is used, and `profiles.json` is byte-for-byte unchanged, so ADR 0003 still holds.

  They passed as soon as they were written, as expected for a ticket that adds no behavior.
- **Transcripts unchanged:** `git diff 0a83203 -- CYOATests/CombatTranscripts` is empty. All 10 approved transcripts are exactly as approved in ticket 02, and all 10 pass.
- **Language data unchanged:** `git diff feature/player-profiles -- language_data.json` is empty.
- **ADR 0002 quirks unchanged:** the `dragon-retreat` transcript shows a full first round before the first `Enter 'a' to attack or 'r' to retreat.` prompt. It also shows an invalid answer playing another round, and `You hastily retreat licking your wounds.` with no ending.
- **ADR 0004** is now Accepted, with its branch, commits and Verification section filled in.
- **Defects found:** none. No production code changed in this ticket.
- **Not done, needs a person: playthroughs of the built program.**
  - `Program.cs` always uses the real `%LOCALAPPDATA%\ChooseYourAdventure` folder. Kinsey chose not to have that folder inspected or changed during verification, so the North, West and other-language playthroughs of the `.exe` weren't run.
  - Instead, the whole-game tests above run the same `Game.StartGame` code, and the transcripts cover all four languages.
  - To check by hand: back up the folder if you have one, then run `dotnet run` and fight once on the North path, once on the West path, and once in another language.
- **Out of scope:** ticket 06, the inconsistencies between the two attack directions, is still needs-triage and untouched.
