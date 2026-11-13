# 03: West path leads to Grubnik in the Murky Swamp (retreat only)

**What to build:** The player can type `w` or `west` at the path choice. They walk into the **Murky Swamp** and meet Grubnik the goblin, a Monster who thinks he's terrifying but is actually ridiculous. The player sees:

1. The West narrative.
2. Grubnik's boastful intro in green.
3. The magic-ring stats line and Grubnik's stats under a goblin stats header.
4. Grubnik's comic attack-or-retreat prompt.

Retreating shows the shared `retreat` ending, and the game ends, because the Goblin Encounter is a **Dead End**. Attacking is wired up in ticket 04.

The path prompt now describes three paths, and the invalid-path message lists `n`, `s`, `w` and `e`. Everything is in all four languages. See the Goblin, Game, Localization and Grubnik's voice sections of `.scratch/goblin-encounter/spec.md`.

**Blocked by:** 01 (Introduce the Monster base class)

> Ticket 02 isn't a blocker. Implement this ticket after 02 anyway, so the two don't produce merge conflicts in the language file.

**Status:** ready-for-agent

- [x] Goblin is a Monster:
  - Name: Grubnik.
  - Strength, Agility and Health Points each rolled 1d12 with the shared Die.
  - Weapon: "rusty axe", Max Damage 6, with ASCII art that is clearly an axe.
  - Uses goblin keys for his stats header and the other Monster-specific messages.
- [x] `w`, `west`, `W` and ` west ` all choose the West path. `IsValidPathChoice` accepts them. `n`, `s` and `e` still behave as before, and `e` stays Exit.
- [x] `path_prompt_full` describes North, South and West and lists `w`. `path_invalid` lists `n`, `s`, `w` and `e`.
- [x] The West branch runs `HandleGoblinEncounter`, then ends the game, as the North branch does.
- [x] `HandleGoblinEncounter` follows `HandleDragonEncounter`'s structure:
  1. The West narrative.
  2. `goblin_intro` with Grubnik's name, in green.
  3. `goblin_stats_intro` and Grubnik's stats.
  4. `goblin_encounter_prompt`. Invalid input shows `invalid` and asks again. `r`/`retreat` shows `retreat` and returns.
- [x] New keys in English, Spanish, French and Italian, with no accented characters:
  - `west_path_narrative`
  - `goblin_intro`
  - `goblin_stats_intro`
  - `goblin_stats_header`
  - `goblin_encounter_prompt`
- [x] The text is comic, never menacing: Grubnik boasts about "his" swamp and demands a toll. Translations keep the tone rather than translating word for word.
- [x] TDD: write each failing test before the code that makes it pass.
  - **`GoblinTests`:**
    - Grubnik is a Monster with name Grubnik, stats from 1 to 12, a rusty axe with Max Damage 6, and ASCII art that isn't empty.
    - `DisplayStats` uses the localized labels and the goblin stats header.
  - **Reflection test of `HandleGoblinEncounter`**, mirroring the dragon encounter test:
    - Retreat input shows the West narrative, intro, stats intro, prompt and retreat ending.
    - Invalid input followed by retreat shows `invalid` and the prompt again.
  - **`GameTests`:** `IsValidPathChoice` cases for `w`, `west`, upper case and padded input.
  - **`MessagesTests`:** the new keys are in the required-key lists for all four languages.
- [x] The full test suite passes, and the dragon and Merchant paths are unchanged.
- [x] Manual check:
  1. Create a character, go West, read the intro and stats, and retreat. The game ends.
  2. Repeat in one other language.
  3. Type an invalid path letter and see the new invalid-path message.

## Comments

Implemented. New `Goblin` Monster: Grubnik, 1d12 Strength/Agility/Health Points from the shared Die, a "rusty axe" (Max Damage 6, ASCII art drawn as a curved blade on a handle, updated after code review). His combat keys (attack intro, taunts, replies, defeated line, endings) already point at `goblin_*` names, but those texts arrive in ticket 04, and nothing can reach them yet because attacking isn't wired.

`Game` creates the Goblin with the shared Die. `w`/`west` returns West from the path prompt and is accepted by `IsValidPathChoice`. The West branch runs `HandleGoblinEncounter` and then ends the game. The handler mirrors `HandleDragonEncounter`: West narrative, green intro, magic-ring line and stats, then the goblin prompt. It handles `r`/`retreat` (shared `retreat` ending) and invalid input. New keys in all four languages: `west_path_narrative`, `goblin_intro`, `goblin_stats_intro`, `goblin_stats_header`, `goblin_encounter_prompt`. `path_prompt_full` and `path_invalid` were rewritten for three paths in all four languages.

TDD, one slice at a time, each red first: Goblin construction and stats range; `DisplayStats` with the goblin header in four languages; `IsValidPathChoice` for `w`/`west`/upper case/padded; the reflection test of the retreat flow; the required keys plus a new `PathMessages_ListWestChoice` test in four languages. Two notes:
- The existing `IsValidPathChoice_ReturnsFalse_ForInvalidInput` used `"west"` as its invalid example. It now uses `"east"`, which records that there's no East path and `e` stays Exit.
- `HandleGoblinEncounter_InvalidInput_ShowsInvalidAndAsksAgain` passed on its first run, because the invalid-input branch was written together with the retreat branch to mirror the dragon handler.

The retreat test also asserts that no `[west_…]` or `[goblin_…]` placeholder is printed, so a missing key can't pass by matching its own fallback text.

Suite: 157/157. Scripted runs in English (invalid path letter, then West, invalid prompt input, retreat; the game ends) and Italian (West, retreat): text, stats and endings correct. "rusty axe" is still untranslated until ticket 04.
