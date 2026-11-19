# 02: Pick a saved Profile and resume at the path choice

**What to build:** When Profiles exist, choosing **1** shows a numbered profile menu. Each Profile is listed as number, name, Race and Occupation, with Race and Occupation in the current language (for example `1. Bob (Human Fighter)` or `1. Bob (Humano Guerrero)`), in the order the Profiles were created, and **New profile** is the last number. Picking a Profile shows a welcome-back message with the character's name, then the character stats screen with the saved Health Points, then goes straight to the path choice, skipping the next-action menu. Picking New profile runs character creation and saves it, as in ticket 01. An invalid choice shows the usual "invalid choice" message and the menu again. Any Profile works in any language. See the Game and Localization sections of `.scratch/player-profiles/spec.md`.

**Blocked by:** 01 (Create a Profile and have it saved)

**Status:** ready-for-agent

- [ ] With no Profiles, choosing **1** still goes straight to character creation.
- [ ] With Profiles, the menu shows `profile_menu_header`, one `profile_menu_entry` line per Profile in creation order, and a last line with the next number and `profile_new_option`, followed by `enter_choice`.
- [ ] Anything other than a listed number shows `invalid` and the menu again.
- [ ] Picking a Profile shows `profile_welcome_back` with the name, then the character stats screen, then the path prompt. The next-action menu isn't shown.
- [ ] The loaded character has exactly its saved values, so every Session starts at its saved (rolled) Health Points.
- [ ] Picking a Profile and exiting doesn't change the saved file.
- [ ] New keys in English, Spanish, French and Italian, with no accented characters:
  - `profile_menu_header`
  - `profile_menu_entry` (`{0}` number, `{1}` name, `{2}` Race, `{3}` Occupation)
  - `profile_new_option`
  - `profile_welcome_back` (`{0}` name)
- [ ] TDD: write each failing test before the code that makes it pass.
  - **Scripted `StartGame` tests** (redirected console, `[DoNotParallelize]`, timeout, temporary folder, Profiles saved through the store first):
    - The menu lists each Profile with its name and translated Race and Occupation, in creation order, with New profile last. Checked in English and one other language.
    - An invalid choice shows `invalid` and the menu again.
    - Picking a Profile shows `profile_welcome_back` with its name, its stats including its saved Health Points, and the path prompt.
    - Picking New profile creates and saves a second Profile after the first.
  - **`MessagesTests`:**
    - The four new keys are in the required-key lists for all four languages.
    - `profile_menu_entry` and `profile_welcome_back` fill in their placeholders in every language.
- [ ] The full test suite passes.
