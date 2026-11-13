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
