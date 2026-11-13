# Spec: Goblin Encounter

Status: ready-for-agent

## Problem Statement

The player can only fight one Monster. At the path choice they can go North to the dragon, South to the Merchant, or exit. The only fight is the Dragon Encounter. It's dangerous, and with 1d20 stats and claws that deal 12 damage, many characters just lose. The world feels small, and the player has no other fight to try, easier or harder.

The combat code also only works with the dragon. `Combat`, `Player.Attack` and the shared combat messages all assume the enemy is the dragon, so a second Monster can't reuse them.

## Solution

Taking the new **West** path (`w` or `west`) leads into the **Murky Swamp** and starts a **Goblin Encounter**. Grubnik the goblin believes he is terrifying, but he is actually ridiculous: rude, sarcastic and full of himself. He's weaker than the dragon (1d12 stats and a rusty axe that deals 6 damage), and he fights with the same combat rules the dragon uses. Magical Armor and a Magical Weapon bought from the Merchant work against him too.

Like the Dragon Encounter, the Goblin Encounter is a **Dead End**. Win, lose or retreat, the game ends with a goblin-specific ending, or the shared retreat ending. Winning gives the player Grubnik's "treasure": three buttons and a soggy sock.

To make this possible, the dragon and the goblin become two kinds of **Monster** that share one combat system (ADR 0002). The dragon's personality, dialogue and stats don't change. The whole encounter is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want a third path to the West, so that I have more than one fight to choose.
2. As a player, I want to type `w` or `west` to take the West path, so that it works like the other path commands.
3. As a player, I want the West path command to ignore upper and lower case and surrounding spaces, as the other path commands do, so that `W` or ` west ` still works.
4. As a player, I want the path prompt to describe all three paths, so that I know West exists before I choose.
5. As a player, I want the invalid-path message to list `n`, `s`, `w` and `e`, so that I know every valid choice after a typo.
6. As a player, I want `e` to keep meaning Exit, so that the controls I already know still work.
7. As a player, I want a West narrative describing my trip into the Murky Swamp, so that the setting is clear before Grubnik appears.
8. As a player, I want Grubnik to introduce himself by name in green text, so that his entrance stands out and doesn't look like the dragon's red.
9. As a player, I want Grubnik's intro to be boastful and ridiculous, so that I can tell he's more silly than scary.
10. As a player, I want my magic ring to show me Grubnik's stats, as it does for the dragon, so that I can decide whether to fight.
11. As a player, I want Grubnik's stats under their own goblin stats header, so that they're clearly his and not the dragon's.
12. As a player, I want to choose to attack or retreat when I meet Grubnik, so that the encounter starts like the dragon's.
13. As a player, I want Grubnik's encounter prompt in his own comic voice, so that the tone stays consistent.
14. As a player, I want invalid input at the encounter prompt to show the usual "invalid choice" message and ask again, so that a typo doesn't end my adventure.
15. As a player, I want retreating before the fight to end the game with the usual retreat ending, so that the West path is a Dead End like the North path.
16. As a player, I want attacking to start the same combat I'd have with the dragon, so that the rules are familiar.
17. As a player, I want the combat stats screen to show Grubnik's name as the Monster header, so that I know who I'm fighting.
18. As a player, I want the combat stats screen to show Grubnik's Strength, Agility, Health Points, weapon and Max Damage, so that I can compare them with my own.
19. As a player, I want Grubnik to have Strength, Agility and Health Points between 1 and 12, so that he's an easier fight than the dragon.
20. As a player, I want Grubnik to fight with a rusty axe dealing up to 6 damage, so that his hits hurt less than the dragon's claws.
21. As a player, I want the rusty axe to have its own ASCII art in combat, so that his attacks look different from the dragon's.
22. As a player, I want my attack messages to name the Monster I'm hitting, so that combat text makes sense against Grubnik.
23. As a player, I want Grubnik to throw one of four rude, sarcastic taunts when he blocks my attack, so that his defenses are entertaining.
24. As a player, I want some of Grubnik's taunts to mention my race, as the dragon's do, so that the insults feel personal.
25. As a player, I want Grubnik to reply with one of four ridiculous lines when I hurt him, so that he complains without sounding threatening.
26. As a player, I want a goblin-specific line when Grubnik's Health Points reach 0, so that his defeat feels like his own.
27. As a player wearing Magical Armor, I want my Protection to reduce Grubnik's hits, so that what I bought from the Merchant helps on every path.
28. As a player wearing Magical Armor, I want to be told how much damage my armor absorbed from Grubnik, so that I can see it working.
29. As a player with a Magical Weapon, I want its higher Max Damage to apply against Grubnik, so that the upgrade helps on every path.
30. As a player, I want my health never to drop below 0 against Grubnik, so that the numbers stay sensible.
31. As a player, I want to retreat mid-combat with Grubnik as I can with the dragon, so that I'm never trapped in a fight.
32. As a player who defeats Grubnik, I want a victory ending where I search his treasure and find three buttons and a soggy sock, so that winning is funny.
33. As a player defeated by Grubnik, I want a defeat ending that's humiliating rather than tragic, so that losing to him matches his character.
34. As a player, I want the game to end after any Goblin Encounter outcome, so that West is a Dead End like North.
35. As a player who visited the Merchant first, I want to keep my Gold, Protection and Magical Weapon when I go West, so that my choices carry over.
36. As a player going North, I want the dragon to look, talk and fight exactly as before, so that adding Grubnik doesn't change Smolderfang.
37. As a player going North, I want the dragon's endings and taunts unchanged, so that Smolderfang keeps his personality.
38. As a player going North, I want the combat stats header to show Smolderfang's name, so that both Monsters are shown the same way.
39. As a player going South, I want the Merchant Encounter to work exactly as before, so that nothing I already use breaks.
40. As a Spanish, French or Italian speaker, I want the whole Goblin Encounter in my language, so that I can play the West path without English.
41. As a Spanish, French or Italian speaker, I want Grubnik's comic personality kept in translation, so that he's as ridiculous in my language as in English.
42. As a Spanish, French or Italian speaker, I want "rusty axe" translated in stats and combat, so that weapon names aren't left in English.
43. As a Spanish, French or Italian speaker, I want the new path prompt and invalid-path message in my language, so that I can find the West path.
44. As a developer, I want one combat system for every Monster, so that a combat fix applies to both fights at once.
45. As a developer, I want adding a future Monster to need only a new subclass and its message keys, so that the game is easy to extend.
46. As a developer, I want the shared combat message keys named after Monsters rather than the dragon, so that the keys describe what they're used for.
47. As a developer, I want every change written test-first (red, green, refactor), so that each behavior is covered before it exists.
48. As a developer, I want the existing dragon and combat tests to keep passing through the Monster change, so that I know the dragon wasn't broken.
49. As a developer, I want tests that fail when any language is missing a goblin or renamed key, so that `[key]` placeholders never reach players.

## Implementation Decisions

**Monster (new abstract base class, ADR 0002)**
- Holds what every Monster has:
  - Name, Strength, Agility, HealthPoints and Weapon.
  - The shared Die, passed in the constructor as Dragon does now.
  - `RollAttack`, `AttackHits` and `RollDamage`.
  - `Attack(Player, Messages)`.
  - `GetRandomDefenseTaunt(Messages)` and `GetRandomDamageReply(Messages)`.
  - `DisplayStats(Messages)`.
- `Attack` keeps the dragon's current behavior exactly:
  1. Roll to hit against the Monster's Strength.
  2. Roll the player's defense against their Agility.
  3. Roll damage, send it through `player.ReduceDamage`, and clamp Health Points at 0.
  4. Add the armor-absorbed line when Protection > 0.
  5. Add the defeated line at 0 Health Points.
- Each subclass provides the message keys that are specific to that Monster:
  - The player-attack intro.
  - The stats header.
  - The defeated narrative.
  - Four defense taunt keys and four damage reply keys.
  - The victory and defeat ending keys.

**Dragon**
- Becomes a subclass of Monster. Its name, 1d20 stats, claws (Max Damage 12) and ASCII art don't change.
- Its Monster-specific text stays exactly the same. It keeps the existing `player_attack_intro`, `dragon_stats_header`, `dragon_defeated_narrative`, `dragon_defense_taunt_1`–`4` and `dragon_damage_reply_1`–`4` text, and its endings stay `victory` and `defeat`.

**Goblin (new subclass)**
- Name: **Grubnik**.
- Rolls 1d12 for Strength, Agility and Health Points, using the shared Die.
- Weapon: **rusty axe**, Max Damage 6, with ASCII art that is clearly an axe.
- Uses its own goblin keys for every Monster-specific message.

**Player**
- `Attack` takes a Monster instead of a Dragon. The hit, defense and damage rules don't change. It uses the Monster's own keys for the attack intro, defense taunt, damage reply and defeated narrative.

**Combat**
- The constructor takes a Player and a Monster. The public `dragon` property becomes `monster`.
- The stats screen's Monster header shows the Monster's name, for example `--- Smolderfang ---` or `--- Grubnik ---`.
- The round loop and retreat behavior don't change.

**Game**
- Creates a Goblin alongside the Dragon, using the shared Die.
- The path prompt accepts `w`/`west` and returns West, and `IsValidPathChoice` accepts `w`/`west`.
- The West branch calls a new `HandleGoblinEncounter`, then ends the game, as the North branch does.
- `HandleGoblinEncounter` follows the structure of `HandleDragonEncounter`:
  1. The West narrative.
  2. Grubnik's intro in green.
  3. The magic-ring stats line and Grubnik's stats.
  4. The goblin encounter prompt, looping on invalid input.
  5. On `r`/`retreat`, the shared `retreat` ending.
  6. On `a`/`attack`, Combat with Grubnik, followed by Grubnik's victory or defeat ending, unless the player retreated mid-combat.
- The ending method chooses the victory or defeat text from the Monster that was fought. `retreat` stays shared.

**Localization**
- English, Spanish, French and Italian all get every new and renamed key. There are no accented characters, following ADR 0001.
- **Renamed** (the text becomes generic, with `{0}` for the Monster's name where needed):

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
  | `combat_dragon_header` | `combat_monster_header` (now with a `{0}` name placeholder) |

  For the dragon, the rendered text must stay the same as before, apart from the combat header now showing Smolderfang's name.
- **New goblin keys:**
  - `west_path_narrative`
  - `goblin_intro` (with `{0}` for the name)
  - `goblin_stats_intro`
  - `goblin_stats_header`
  - `goblin_encounter_prompt`
  - `goblin_player_attack_intro`
  - `goblin_defeated_narrative`
  - `goblin_defense_taunt_1`–`4` (`{0}` is the player's race where it's used)
  - `goblin_damage_reply_1`–`4`
  - `goblin_victory`
  - `goblin_defeat`
- **Rewritten keys:**
  - `path_prompt_full` describes three paths and lists `w`.
  - `path_invalid` lists `n`, `s`, `w` and `e`.
- **`displayWeaponMap`:** adds "rusty axe" in each language.
- **Grubnik's voice:**
  - He thinks he's the most fearsome creature in the swamp, but he's rude, sarcastic, petty and slightly ridiculous. Nothing he says is genuinely menacing.
  - His intro boasts about owning "his" swamp and demands a toll.
  - His taunts are insulting rather than threatening.
  - His damage replies are whiny or indignant.
  - His victory ending has the player find his treasure: three buttons and a soggy sock.
  - His defeat ending is humiliating rather than tragic.
  - Translations keep the same tone rather than translating word for word.

## Testing Decisions

- **Red-green-refactor is required.** For every behavior, write a failing test first and run it to see it fail for the right reason. Then write the least code needed to pass it, then refactor with all tests green. No production code is written before a test that needs it.
  - Introducing Monster is a refactor. Existing dragon and combat tests must be green before it and green after it.
  - Behavior changes that come with the refactor (the Monster-name combat header and the renamed keys) get their test changes first, so those tests go red before the code and JSON change.
- **Test external behavior only.** Call public methods (or `Game`'s private encounter handler through reflection, matching existing prior art) with Players built for the test. Check returned strings, public state and redirected console output for message text. Don't test private helpers or exact dice results.
- **Five test areas, all using existing test patterns:**
  1. **Goblin Encounter flow:** test `Game.HandleGoblinEncounter` through reflection with redirected console input and output, mirroring `HandleDragonEncounter_AttackPath_DisplaysEncounterFlowText`.
     - **Retreat path:** the output contains the West narrative, Grubnik's intro, the stats intro, the encounter prompt and the retreat ending.
     - **Attack path:** attack, then retreat mid-combat. The output contains the encounter text and the combat stats header with Grubnik's name.
     - **Invalid input:** the "invalid choice" message is shown and the prompt repeats.
  2. **Grubnik on his own:** a new `GoblinTests` class mirroring `DragonTests`.
     - His name is Grubnik, his stats are between 1 and 12, and he has a rusty axe with Max Damage 6 and ASCII art that isn't empty.
     - His taunts and replies are drawn from the four goblin keys.
     - `Attack` returns text that isn't empty, never raises the player's Health Points, and never takes them below 0.
     - `DisplayStats` uses the localized labels and the goblin stats header.
     - Grubnik is a Monster.
  3. **Combat with Grubnik:** add cases to `CombatTests`.
     - `GetCombatStatsDisplay` with Grubnik includes his name in the Monster header, along with his stats, weapon and Max Damage.
     - The constructor stores him in `monster`.
     - Existing dragon combat tests are updated for `monster` and the Monster header, and still check the dragon's stats.
  4. **Path input:** add cases to the `IsValidPathChoice` tests in `GameTests` for `w`, `west`, upper case and padded input. Existing `n`/`s`/`e` cases still pass.
  5. **Translations:** in `MessagesTests`, add every goblin key and renamed key to the required-key lists for English, Spanish, French and Italian, and remove the old dragon combat key names from those lists. Add weapon display tests showing "rusty axe" translates to a name other than the fallback in each language.
- **Dragon regression:** keep all existing `DragonTests` and the dragon encounter tests, updating only renamed key references. Add a check that the Dragon is a Monster and its stats stay 1d20 with claws (Max Damage 12).
- **Not unit tested:** the West branch of the main game loop. It's covered by a manual playthrough in English, plus a quick check of the West path in one other language:
  1. Create a character, go West, read the intro and stats, fight, and confirm the ending ends the game.
  2. Repeat after shopping South, to see Protection absorb damage.
  3. Go North once to confirm the dragon is unchanged.
- **Prior art:**
  - `DragonTests`.
  - The reflection-based encounter test and console redirection in `GamePbi18Tests`.
  - `CombatTests` and its test dragon helper.
  - The `IsValidPathChoice` tests in `GameTests`.
  - The required-key and weapon display tests for each language in `MessagesTests`.
  - Players and Weapons built with object initializers.

## Out of Scope

- Existing combat quirks, which both Monsters now share: the mid-combat retreat skipping the ending, and the first round running before the player is prompted.
- Earning Gold or items from Grubnik. The game ends anyway.
- Changing the dragon's stats, dialogue, endings or personality, beyond showing its name in the combat stats header.
- Merging the two encounter handlers into one generic Monster encounter handler.
- An East path, or changing `e` from Exit.
- Letting tests control the dice.
- Building the language menu from the JSON (deferred in ADR 0001).
- Further Monsters.

## Further Notes

- Work happens on `feature/goblin-encounter`, which branches from `feature/merchant-encounter` because the feature relies on Gold, Protection and the Magical Weapon.
- ADR 0002 records the decision to use a Monster base class. Its status moves from Proposed to Accepted once the change is implemented.
- Domain terms (Monster, Dead End, Dragon Encounter, Goblin Encounter, Murky Swamp, plus the existing Merchant terms) are defined in `GLOSSARY.md`.
- Grubnik's 1d12 stats against the dragon's 1d20 make him the easier fight. The average stat is 6.5 instead of 10.5, and his maximum hit is 6 instead of 12. That's before the player's Protection, which can cancel half of his maximum hit.
