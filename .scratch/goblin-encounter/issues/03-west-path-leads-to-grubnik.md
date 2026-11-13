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

- [ ] Goblin is a Monster:
  - Name: Grubnik.
  - Strength, Agility and Health Points each rolled 1d12 with the shared Die.
  - Weapon: "rusty axe", Max Damage 6, with ASCII art that is clearly an axe.
  - Uses goblin keys for his stats header and the other Monster-specific messages.
- [ ] `w`, `west`, `W` and ` west ` all choose the West path. `IsValidPathChoice` accepts them. `n`, `s` and `e` still behave as before, and `e` stays Exit.
- [ ] `path_prompt_full` describes North, South and West and lists `w`. `path_invalid` lists `n`, `s`, `w` and `e`.
- [ ] The West branch runs `HandleGoblinEncounter`, then ends the game, as the North branch does.
- [ ] `HandleGoblinEncounter` follows `HandleDragonEncounter`'s structure:
  1. The West narrative.
  2. `goblin_intro` with Grubnik's name, in green.
  3. `goblin_stats_intro` and Grubnik's stats.
  4. `goblin_encounter_prompt`. Invalid input shows `invalid` and asks again. `r`/`retreat` shows `retreat` and returns.
- [ ] New keys in English, Spanish, French and Italian, with no accented characters:
  - `west_path_narrative`
  - `goblin_intro`
  - `goblin_stats_intro`
  - `goblin_stats_header`
  - `goblin_encounter_prompt`
- [ ] The text is comic, never menacing: Grubnik boasts about "his" swamp and demands a toll. Translations keep the tone rather than translating word for word.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`GoblinTests`:**
    - Grubnik is a Monster with name Grubnik, stats from 1 to 12, a rusty axe with Max Damage 6, and ASCII art that isn't empty.
    - `DisplayStats` uses the localized labels and the goblin stats header.
  - **Reflection test of `HandleGoblinEncounter`**, mirroring the dragon encounter test:
    - Retreat input shows the West narrative, intro, stats intro, prompt and retreat ending.
    - Invalid input followed by retreat shows `invalid` and the prompt again.
  - **`GameTests`:** `IsValidPathChoice` cases for `w`, `west`, upper case and padded input.
  - **`MessagesTests`:** the new keys are in the required-key lists for all four languages.
- [ ] The full test suite passes, and the dragon and Merchant paths are unchanged.
- [ ] Manual check:
  1. Create a character, go West, read the intro and stats, and retreat. The game ends.
  2. Repeat in one other language.
  3. Type an invalid path letter and see the new invalid-path message.
