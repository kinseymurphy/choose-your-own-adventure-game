# 07: Verify Player Profiles end to end and accept ADR 0003

**What to build:** Confirm the whole Player Profiles feature works together as a player would use it, record the evidence, and accept ADR 0003. This ticket adds no new behavior. It runs the automated suite and scripted playthroughs of the real game, fixes only small defects it uncovers (anything bigger is reported as a new ticket), and reports any check that still needs a person. See the Testing Decisions and Known Limitations sections of `.scratch/player-profiles/spec.md`, and ADR 0003.

**Blocked by:** 03 (Profile names are unique), 04 (Merchant purchases are saved), 05 (A damaged profile file is kept as a backup), 06 (A failed save warns and the Session continues)

**Status:** ready-for-agent

- [ ] `dotnet test CYOATests` passes. Record the pass count and how many tests were added by this feature.
- [ ] Scripted playthroughs of the real game, run with piped input against a temporary profile folder or a backed-up per-user folder so no real player data is changed:
  1. Create a character in English, shop at the Merchant, and exit. Restart, pick the Profile, and confirm the welcome back, the stats, the Gold left over and the purchases.
  2. From that Profile, go to a Dead End (North or West). Restart and confirm the Profile is still listed and starts at its rolled Health Points.
  3. Try to create a second Profile with the first name in different case, and see the name-taken message.
  4. Repeat the profile menu and welcome back in one other language, with Race and Occupation translated.
  5. Damage `profiles.json` by hand, start the game, and confirm the warning names the backup file and creation starts.
- [ ] No regressions: the Merchant, Dragon Encounter and Goblin Encounter still behave as before.
- [ ] ADR 0003's status is changed from Proposed to Accepted, with the branch and commit filled in and its Verification section completed.
- [ ] The Known Limitations in the spec (Halfling with 0 Health Points or less, two game windows at once, weapon values frozen in saves) are still accurate.
- [ ] Under `## Comments`, report what was verified, any defects found and fixed, and any manual check that still needs a person. For example: viewing the real `%LOCALAPPDATA%` file in Explorer, or how the console displays the messages in each language.

## Comments

**Verified on 2026-11-13 by Claude:**

- **Automated suite:** `dotnet test CYOATests` passed 246 of 246 tests. The baseline was 197, so the feature added 49. The scripted `ProfileGameTests` passed 8 runs in a row.
- **Scripted playthroughs:** all five ran against the real program and behaved as specified. The details are in ADR 0003's Verification section. They used `%LOCALAPPDATA%\ChooseYourAdventure`, which didn't exist beforehand and was deleted afterwards, so no player data was touched.
- **No regressions:** all existing Merchant, Dragon, Goblin and combat tests pass. The playthroughs went through the Merchant and a Dragon Dead End normally.
- **ADR 0003:** set to Accepted, with its Verification section filled in.
- **Known Limitations:** all three are still accurate (Halfling with 0 Health Points or less, two windows at once, weapon values frozen in saves).
- **Defects found in code review and fixed:**
  - The "missing required data" check now also requires a Weapon with a type, ASCII art and Max Damage above 0.
  - `Save` now catches every exception, so it never throws.
  - Tests were added for loading an empty folder and for retreating during a fight.
- **Still needs a person:**
  - Open the real `%LOCALAPPDATA%\ChooseYourAdventure\profiles.json` in Explorer after normal play.
  - Check how the Spanish, French and Italian profile messages look in a real console window. Only Spanish was viewed, and only through piped output.
  - Note that `>` in weapon ASCII art is stored as `>` in the JSON. It loads back correctly, but it's less readable when you open the file by hand.
