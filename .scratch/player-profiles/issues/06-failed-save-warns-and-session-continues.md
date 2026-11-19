# 06: A failed save warns and the Session continues

**What to build:** If the game can't save (the folder can't be created, the disk is full, the file is locked), the player sees a warning in their language that clearly says the latest progress was not saved. The Session carries on normally. Any existing `profiles.json` is left exactly as it was. See the Error handling section of `.scratch/player-profiles/spec.md`.

**Blocked by:** 01 (Create a Profile and have it saved)

**Status:** ready-for-agent

- [ ] Saving reports whether it worked and never throws to the caller.
- [ ] A failed save leaves an existing `profiles.json` unchanged. Any temporary file is cleaned up where possible.
- [ ] At both save points (creation finished, and leaving the Merchant once ticket 04 adds it), Game shows `profile_save_failed` when a save fails and carries on to the path prompt.
- [ ] New key `profile_save_failed` in English, Spanish, French and Italian, with no accented characters. The text clearly says the latest progress was not saved.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`ProfileStore` tests** (temporary folder):
    - A store whose folder can't be created, for example because its path goes through an existing file, reports failure without throwing.
    - When the save can't replace an existing `profiles.json` (for example, held open with no sharing during the test), the save reports failure and the file's contents are unchanged.
  - **Scripted `StartGame` test** (redirected console, `[DoNotParallelize]`, timeout): with a store whose folder can't be created, creating a character shows `profile_save_failed`, then the path prompt, and exiting works normally.
  - **`MessagesTests`:** `profile_save_failed` is in the required-key lists for all four languages.
- [ ] The full test suite passes.
