# 03: Profile names are unique

**What to build:** When creating a Profile, the player can't use a name that already belongs to another Profile. Names are compared ignoring upper and lower case and surrounding spaces, so "Bob", "bob" and " Bob " are the same name. A taken name shows a new message in the player's language and asks for the name again, keeping the Race already chosen. A blank name still shows the existing "name cannot be empty" message. The name is saved as typed (trimmed), with its case kept. See the Profile names section of `.scratch/player-profiles/spec.md`.

**Blocked by:** 02 (Pick a saved Profile and resume at the path choice). With Profiles saved, the player reaches New profile through the profile menu.

**Status:** ready-for-agent

- [ ] `ProfileStore` can say whether a name is already taken, ignoring case and surrounding spaces.
- [ ] Character creation accepts an optional name check. With one, a taken name shows `profile_name_taken` and the name prompt repeats. Without one, character creation behaves exactly as today.
- [ ] Game passes the store's name check into character creation for New profile.
- [ ] The Race chosen before the name prompt is kept. The player isn't asked for it again.
- [ ] A blank name still shows `name_invalid` and repeats the prompt.
- [ ] New key `profile_name_taken` in English, Spanish, French and Italian, with no accented characters.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`ProfileStore` tests** (temporary folder): a saved name is taken in its own case, in different case, and with surrounding spaces. An unused name isn't taken.
  - **Scripted `StartGame` test** (redirected console, `[DoNotParallelize]`, timeout, temporary folder):
    - With a Profile named "Bob" saved, New profile with the name "bob" shows `profile_name_taken`.
    - The next name is accepted without the Race prompt appearing a second time.
    - Afterwards the store holds both Profiles, with the original "Bob" unchanged.
  - **`MessagesTests`:** `profile_name_taken` is in the required-key lists for all four languages.
- [ ] The full test suite passes, including existing character creation behavior.
