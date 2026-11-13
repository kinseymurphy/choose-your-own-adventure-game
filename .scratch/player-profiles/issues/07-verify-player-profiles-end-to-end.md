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
