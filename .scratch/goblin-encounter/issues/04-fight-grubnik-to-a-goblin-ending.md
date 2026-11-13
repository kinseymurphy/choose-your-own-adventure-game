# 04: Fight Grubnik through to a goblin ending

**What to build:** Attacking Grubnik starts the same combat the dragon uses. The player's attack line is Grubnik's own, and when Grubnik blocks a hit he throws one of four rude, sarcastic taunts. When he's hurt he whines with one of four ridiculous damage replies, and he gets his own defeated line at 0 Health Points. Combat runs as a Dead End:

- **Victory:** the player searches Grubnik's treasure and finds three buttons and a soggy sock.
- **Defeat:** a humiliating, not tragic, ending.
- **Retreat:** retreating before the fight shows the shared `retreat` ending. Retreating mid-combat keeps the existing quirk of skipping the ending.

Every outcome ends the game. Magical Armor and a Magical Weapon from the Merchant work against Grubnik. "rusty axe" is translated in stats and combat. Everything is in all four languages. See the Goblin, Game, Localization and Grubnik's voice sections of `.scratch/goblin-encounter/spec.md`.

**Blocked by:** 02 (Shared combat messages are named after Monsters), 03 (West path leads to Grubnik in the Murky Swamp)

**Status:** ready-for-agent

- [ ] At Grubnik's prompt, `a`/`attack` starts shared combat with Grubnik. After combat, the game shows `goblin_victory` or `goblin_defeat`, unless the player retreated mid-combat. Then the game ends.
- [ ] New keys in English, Spanish, French and Italian, with no accented characters:
  - `goblin_player_attack_intro`
  - `goblin_defeated_narrative`
  - `goblin_defense_taunt_1`–`4` (`{0}` is the player's race where it's used)
  - `goblin_damage_reply_1`–`4`
  - `goblin_victory`
  - `goblin_defeat`
- [ ] "rusty axe" is added to `displayWeaponMap` in all four languages.
- [ ] Grubnik's voice:
  - He thinks he's terrifying but is rude, sarcastic, petty and slightly ridiculous.
  - Taunts are insulting rather than threatening.
  - Damage replies are whiny or indignant.
  - The victory ending includes the soggy sock.
  - Translations keep the tone.
- [ ] The dragon's dialogue, endings and stats are unchanged.
- [ ] Protection reduces Grubnik's hits and the armor-absorbed line appears. A Magical Weapon's higher Max Damage applies. Health Points never go below 0.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`GoblinTests`:**
    - Taunts and damage replies come from the four goblin keys.
    - `Attack` returns text that isn't empty, never raises the player's Health Points, and never takes them below 0.
  - **`CombatTests`:** with Grubnik, the combat stats screen shows `--- Grubnik ---` and his stats, weapon and Max Damage.
  - **Reflection test of `HandleGoblinEncounter`:** attack then retreat shows the encounter text and the combat stats header with Grubnik's name.
  - **`MessagesTests`:**
    - The new goblin keys are in the required-key lists for all four languages.
    - "rusty axe" translates to a name other than the fallback in each language.
- [ ] The full test suite passes, including every existing dragon, combat and Merchant test.
- [ ] Manual playthrough:
  1. Go West and fight Grubnik to an ending in English, then spot check one other language.
  2. Shop South for armor and a weapon, then go West, and see the armor-absorbed line and the upgraded weapon.
  3. Go North once to confirm the dragon is unchanged.
- [ ] ADR 0002's status is changed from Proposed to Accepted, with the branch and commit filled in.
