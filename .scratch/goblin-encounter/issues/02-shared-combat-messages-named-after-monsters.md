# 02: Shared combat messages are named after Monsters

**What to build:** The combat messages every Monster shares stop being named after the dragon. The 10 shared combat keys are renamed to `monster_*` names in English, Spanish, French and Italian, and their text uses `{0}` for the Monster's name where needed. The combat stats screen's Monster header shows the Monster's name, for example `--- Smolderfang ---` instead of `--- Dragon ---`. Apart from that header, every dragon combat line reads exactly as before. See the rename table under Localization in `.scratch/goblin-encounter/spec.md`.

**Blocked by:** 01 (Introduce the Monster base class)

**Status:** ready-for-agent

- [ ] These keys are renamed in all four languages:

  | Old key | New key |
  |---|---|
  | `player_missed_dragon` | `player_missed_monster` |
  | `player_hit_dragon` | `player_hit_monster` |
  | `dragon_defended_player_attack` | `monster_defended_player_attack` |
  | `dragon_health_now` | `monster_health_now` |
  | `dragon_attack_intro` | `monster_attack_intro` |
  | `dragon_missed_player` | `monster_missed_player` |
  | `dragon_hit_player` | `monster_hit_player` |
  | `player_defended_dragon_attack` | `player_defended_monster_attack` |
  | `dragon_damage_dealt` | `monster_damage_dealt` |
  | `combat_dragon_header` | `combat_monster_header` |

- [ ] None of the old key names remain in the language file or the code.
- [ ] `combat_monster_header` takes the Monster's name as `{0}`. The combat stats screen shows `--- Smolderfang ---` for the dragon.
- [ ] Every other dragon combat line renders the same text as before in all four languages. The dragon's own keys (attack intro, stats header, defeated narrative, taunts, replies and endings) are untouched.
- [ ] No accented characters, following ADR 0001.
- [ ] TDD: first update the required-key tests in `MessagesTests` for all four languages, and the header assertion in `CombatTests`, to use the new names. Watch them fail, then rename the keys and the code.
- [ ] A `CombatTests` case checks that the combat stats screen contains the dragon's name in the Monster header.
- [ ] The full test suite passes.
- [ ] Manual check: go North and fight in English and one other language. The only visible change is the Monster header showing Smolderfang's name.
