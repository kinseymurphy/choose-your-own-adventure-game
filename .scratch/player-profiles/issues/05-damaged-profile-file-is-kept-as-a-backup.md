# 05: A damaged profile file is kept as a backup

**What to build:** If `profiles.json` can't be read (it doesn't parse, is missing required data, or has a `version` other than `1`), the game moves it aside instead of ever overwriting it. The player sees a warning in their language naming the backup file, then continues with no Profiles, so choosing **1** goes straight to character creation. The first backup is `profiles.unreadable.json`. If that name is taken, the first free name of `profiles.unreadable-2.json`, `-3` and so on is used, and an existing backup is never overwritten. If the damaged file can't even be moved aside, the player sees a different warning, continues with no Profiles, and the store refuses to overwrite the file for the rest of the Session. See the Error handling section of `.scratch/player-profiles/spec.md`.

**Blocked by:** 01 (Create a Profile and have it saved)

**Status:** ready-for-agent

- [ ] Loading an unreadable file moves it to the first free name of `profiles.unreadable.json`, `profiles.unreadable-2.json`, `profiles.unreadable-3.json`, … in the same folder. Loading reports that the file was unreadable and gives the backup name, and returns no Profiles.
- [ ] A `version` other than `1` is treated as unreadable.
- [ ] Existing backups are never overwritten and keep their original contents.
- [ ] Game shows `profile_file_unreadable` with the backup file name, then continues as if there were no Profiles.
- [ ] The next save after a successful backup starts a fresh `profiles.json`.
- [ ] **Backup can't be made:** if the unreadable file can't be moved aside:
  - Loading reports that it was unreadable and wasn't moved.
  - Game shows `profile_file_unreadable_not_moved` and continues with no Profiles.
  - Every save that Session reports failure and leaves the damaged file untouched. Showing the save-failure warning is ticket 06.
- [ ] New keys in English, Spanish, French and Italian, with no accented characters:
  - `profile_file_unreadable` (`{0}` backup file name)
  - `profile_file_unreadable_not_moved`
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`ProfileStore` tests** (temporary folder):
    - A file that isn't valid JSON is moved to `profiles.unreadable.json` with its contents unchanged, and loading reports that name.
    - With `profiles.unreadable.json` already present, the next damaged file goes to `profiles.unreadable-2.json`, and both keep their contents. Likewise `-3` when `-2` is also taken.
    - A file with `version` 2 is treated as unreadable.
    - A save after a backup creates a new readable `profiles.json`.
    - A damaged file that can't be moved (for example, held open with no sharing during the test) is reported as not moved. A later save reports failure, and the file's contents are unchanged.
  - **Scripted `StartGame` test** (redirected console, `[DoNotParallelize]`, timeout, temporary folder): with a damaged `profiles.json`, choosing **1** shows `profile_file_unreadable` with the backup name, then goes straight to character creation.
  - **`MessagesTests`:**
    - Both new keys are in the required-key lists for all four languages.
    - `profile_file_unreadable` fills in its placeholder in every language.
- [ ] The full test suite passes.
