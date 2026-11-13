# 01: Create a Profile and have it saved

**What to build:** On a first run with no Profiles, choosing **1** on the main menu goes straight to character creation, which works exactly as today. When creation finishes (the player picks "proceed" on the next-action menu), the character is saved as a **Profile** in `profiles.json` in the per-user folder, and the game goes on to the path choice. Running the game, creating a character and exiting leaves that file behind. The main menu now describes option 1 as playing rather than only creating a character. See the Profile data, ProfileStore, Storage file and Game sections of `.scratch/player-profiles/spec.md`, and ADR 0003.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] A new `ProfileStore` is given its folder in its constructor and is the only class that reads or writes the profile file. In this ticket it can:
  - Load the Profiles in creation order. A missing file or folder means no Profiles, with no warning.
  - Save a character, adding it to the end of the list. It creates the folder if needed.
- [ ] `profiles.json` holds a `version` of `1` and the list of Profiles. Each Profile holds name, Race, Occupation, Strength, Agility, Health Points, Gold, Protection and the whole Weapon (type, Max Damage, ASCII art, magical). Race and Occupation are the internal English names.
- [ ] Saving writes to a temporary file in the same folder and only then replaces `profiles.json`.
- [ ] `Game` takes a `ProfileStore` in its constructor. The startup code passes one for the `ChooseYourAdventure` folder inside the local application data folder.
- [ ] Existing tests that create a `Game` without a store keep working without touching storage, through a constructor that doesn't use storage or by passing a store for a temporary folder.
- [ ] Game saves the character once, right after character creation finishes, before the path choice. Closing the game during creation saves nothing.
- [ ] Game doesn't save after a Dead End or on Exit.
- [ ] The `menu` key is rewritten in English, Spanish, French and Italian to describe option 1 as playing (choosing or creating a character), with option 2 still Exit. No accented characters.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`ProfileStore` tests**, each in its own new temporary folder, deleted in cleanup:
    - Loading from a missing folder returns no Profiles.
    - A fresh character round-trips every saved field, including its Weapon and Protection 0.
    - Two saved characters load in the order they were saved.
    - The saved file has `version` 1.
  - **Scripted `StartGame` test** (console input and output redirected and restored, `[DoNotParallelize]`, a test timeout, input ending in an exit):
    - With no Profiles, choosing **1** goes straight to character creation.
    - After creating a character and exiting with `e`, the store holds one Profile with the typed name, Race and Occupation.
  - **`MessagesTests`:** the rewritten `menu` is still in the required-key lists for all four languages.
- [ ] The full test suite passes.
