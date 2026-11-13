# Spec: Player Profiles

Status: ready-for-agent

## Problem Statement

Every time the player starts the game, their character is gone. They roll a new Race, Occupation, stats and Gold, and anything they bought from the Merchant (Magical Armor, a Magical Weapon) is lost when the program closes. Because the Dragon Encounter and the Goblin Encounter are Dead Ends that close the program, a character can never be used for more than one fight. A player who rolled a strong character, or spent their Gold carefully, has no way to come back to it.

## Solution

At startup, after choosing a language, the player picks **1** on the main menu as before. If any **Profiles** exist, they see a numbered list of them, showing each character's name, Race and Occupation in the current language, with **New profile** as the last option. Picking a Profile welcomes the character back, shows their stats, and goes straight to the path choice. Picking New profile runs character creation as it works today, except that the name must not already belong to another Profile.

A Profile holds exactly one character: name, Race, Occupation, Strength, Agility, the Health Points rolled at creation, Gold, Protection and Weapon. The game saves it when character creation finishes and each time the player leaves the Merchant. A Dead End still ends the **Session** and closes the program, but the Profile is kept with everything it owns. Every Session starts at the character's rolled Health Points, so damage from a fight never carries over.

All Profiles are stored in one file in the player's local application data folder (ADR 0003). If that file is damaged or can't be written, the player is warned in their language, their data is never silently overwritten, and they can keep playing. Everything new is available in English, Spanish, French and Italian.

## User Stories

1. As a returning player, I want to pick my saved character at startup, so that I can continue with it instead of starting over.
2. As a player, I want the profile menu to appear after I choose my language, so that it's in my language.
3. As a player, I want the main menu's option 1 to lead to my Profiles, so that the menu I already know still works.
4. As a player, I want option 2 on the main menu to still quit, so that leaving the game works as before.
5. As a player, I want the main menu to describe option 1 as playing rather than only creating a character, so that it's clear saved characters are reached through it.
6. As a returning player, I want each Profile listed with a number, so that I choose it the same way I choose everything else in the game.
7. As a returning player, I want each entry to show the character's name, Race and Occupation, so that I can recognise the character I want.
8. As a Spanish, French or Italian speaker, I want Race and Occupation in the profile menu shown in my language, so that the list isn't half in English.
9. As a returning player, I want Profiles listed in the order I created them, so that the list stays stable from one Session to the next.
10. As a player, I want "New profile" as the last numbered option, so that I can always create another character.
11. As a first-time player, I want to go straight to character creation when no Profiles exist, so that I don't face a menu with only one choice.
12. As a player, I want an invalid choice in the profile menu to show the usual "invalid choice" message and ask again, so that a typo doesn't start the wrong character.
13. As a player creating a Profile, I want character creation to work as it does today, so that the rules for Race, Occupation, rolls and Gold are unchanged.
14. As a player creating a Profile, I want to be told when my chosen name already belongs to another Profile, so that I understand why it wasn't accepted.
15. As a player creating a Profile, I want names compared ignoring upper and lower case and surrounding spaces, so that "Bob" and " bob " can't both appear in the menu.
16. As a player creating a Profile, I want to be asked for a different name without choosing my Race again, so that one taken name doesn't restart creation.
17. As a player creating a Profile, I want my character saved as soon as creation finishes, so that it's there next time even if I quit at the path choice.
18. As a player who closes the window during character creation, I want no half-made Profile left behind, so that the menu only lists complete characters.
19. As a returning player, I want a welcome-back message using my character's name, so that I know the right character was loaded.
20. As a returning player, I want to see my character's stats when I come back, so that I can check my Gold, Protection and Weapon before choosing a path.
21. As a returning player, I want to go straight to the path choice after my stats, so that I don't pass through the creation menu again.
22. As a returning player, I want my Strength and Agility exactly as rolled, so that my character is the one I made.
23. As a returning player, I want every Session to start at the Health Points I rolled at creation, so that a fight in a past Session doesn't leave me weakened forever.
24. As a returning player, I want my Gold as I left it, so that what I spent stays spent and what I saved is still there.
25. As a returning player who bought Magical Armor, I want to still have it, with Protection 3, so that my purchase lasts.
26. As a returning player who bought a Magical Weapon, I want it still magical, with its higher Max Damage and its name, so that my upgrade lasts.
27. As a returning player who bought Magical Armor, I want the Merchant to tell me I already own it, so that I can't pay for it twice.
28. As a returning player who bought a Magical Weapon, I want the Merchant to tell me I already own it, so that I can't upgrade it twice.
29. As a player, I want my purchases saved when I leave the Merchant, so that they're kept even if I quit at the path choice afterwards.
30. As a player who defeats a Monster, I want my Profile kept, so that I can play the character again.
31. As a player defeated by a Monster, I want my Profile kept with its Gold, Magical Armor and Magical Weapon, so that a loss ends the Session without costing me my character.
32. As a player who retreats, before or during a fight, I want my Profile kept as it was, so that retreating is never punished.
33. As a player, I want the program to close after a Dead End or Exit, as it does today, so that the end of a Session feels the same as before.
34. As a player playing in any language, I want any of my Profiles to work in that language, so that switching language never loses a character.
35. As a player, I want my Profiles kept when the game is rebuilt or updated, so that they don't vanish between versions.
36. As a player whose profile file is damaged, I want to be warned in my language, so that I know why my Profiles are missing.
37. As a player whose profile file is damaged, I want it kept as a backup and the warning to name that backup, so that my data can still be recovered.
38. As a player whose profile file is damaged more than once, I want each backup kept under its own numbered name, so that an older backup is never overwritten.
39. As a player whose profile file is from a version the game doesn't understand, I want it treated as unreadable and kept as a backup, so that it isn't overwritten by an older game.
40. As a player whose profile file is damaged, I want to keep playing with no Profiles, so that I'm not locked out of the game.
41. As a player whose progress couldn't be saved, I want a warning in my language that clearly says the latest progress wasn't saved, so that I'm not surprised later.
42. As a player whose progress couldn't be saved, I want to keep playing the Session, so that a storage problem doesn't end my adventure.
43. As a player whose progress couldn't be saved, I want my previously saved Profiles left untouched, so that a failed save never damages what was already there.
44. As a Spanish, French or Italian speaker, I want every new message in my language, so that I can use Profiles without English.
45. As a developer, I want all storage behind one small class that's given its folder, so that tests never touch the real profile file.
46. As a developer, I want Health Points only saved before any combat damage can happen, so that combat damage never overwrites the rolled Health Points.
47. As a developer, I want every change written test-first (red, green, refactor), so that each behavior is covered before it exists.
48. As a developer, I want end-to-end tests that drive the whole game through scripted console input, so that the profile flow is checked the way a player uses it.
49. As a developer, I want those end-to-end tests to time out instead of hanging, so that a broken input script fails rather than blocking the test run.
50. As a developer, I want tests that fail when any language is missing a new profile key, so that `[key]` placeholders never reach players.
51. As a developer, I want the existing Merchant, Dragon, Goblin and combat tests to keep passing, so that I know Profiles didn't break the game.

## Implementation Decisions

**Profile data**
- A Profile stores: name, Race, Occupation, Strength, Agility, Health Points, Gold, Protection and Weapon.
- Race and Occupation are stored as the internal English names ("Human", "Fighter"), as `Player` holds them now. They're translated only for display, so a Profile works in every language.
- The Weapon is stored whole: type, Max Damage, ASCII art and whether it's magical (ADR 0003). A Magical Weapon is stored as it is after the Merchant upgrades it, for example type "magical dagger" with Max Damage 10. It isn't rebuilt from Occupation on load.
- Protection is stored as a number. As today, Protection above 0 means the character owns Magical Armor. No separate purchase list is stored.
- Health Points means the value rolled at character creation (ADR 0003). `Player` gets **no** new field for it.
- Not stored: current or damaged Health Points, the language, Monster stats, the Merchant, and the Session's progress along the paths.

**Health Points rule (must hold)**
- Health Points may only be saved at the two save points below. Both happen before any combat damage can occur in that Session.
- Combat-damaged Health Points must never overwrite the saved Health Points. No save may be added after a fight, or anywhere a Monster may have lowered the character's Health Points, without first changing this design and ADR 0003.
- Because Health Points only change during a Dead End fight, and a Dead End always ends the Session, the character's Health Points at both save points still equal the rolled value. Loading a Profile therefore starts every Session at full rolled Health Points with no healing code.

**Profile names**
- A Profile is known by its character's name. There's no separate profile name.
- Names are unique ignoring upper and lower case and surrounding spaces. The name is saved as typed (trimmed), with its case kept.
- The name check happens inside character creation's existing name prompt. A blank name still shows `name_invalid`. A name already taken shows the new `profile_name_taken` message. Either way the name prompt repeats, and the Race already chosen is kept.

**ProfileStore (new class)**
- The only class that reads or writes the profile file.
- Its constructor takes the folder to use. It creates the folder when it first saves. The real game passes the per-user folder `ChooseYourAdventure` inside the local application data folder. Tests pass a temporary folder.
- It offers three operations:
  - **Load the Profiles:** returns the saved characters in the order they were created. It also reports whether the file was unreadable and, if so, the backup file name it was moved to.
  - **Check a name:** says whether a name is already taken, using the rule above.
  - **Save a character:** adds a new Profile at the end of the list, or replaces the Profile with the same name in place. It reports whether the save worked. It never throws to the caller.
- Loaded characters are ordinary `Player` objects with their saved values, ready to play.

**Storage file**
- One file, `profiles.json`, in the folder given to the store, written and read with `System.Text.Json` (already part of .NET, so no new packages).
- Its top level holds a `version` number, starting at `1`, and the list of Profiles in creation order. Each Profile holds the fields listed under Profile data, with the Weapon as a nested object.
- Saving writes the whole list to a separate temporary file in the same folder, then replaces `profiles.json` with it only once the write has finished. A failed or interrupted save leaves the existing `profiles.json` untouched.

**Error handling**
- **Missing file or folder:** treated as no Profiles. Not an error, and no warning.
- **Unreadable file:** the file can't be parsed, is missing required data, or has a `version` other than `1`.
  1. The store moves it aside to `profiles.unreadable.json` in the same folder.
  2. If that name is taken, it uses the first free name of `profiles.unreadable-2.json`, `profiles.unreadable-3.json`, and so on. An existing backup is never overwritten.
  3. Game shows `profile_file_unreadable`, naming the backup file.
  4. The game continues with no Profiles. The next save starts a fresh `profiles.json`.
- **Save failure:** for example, the folder can't be created, the disk is full, or the file is locked.
  - Game shows `profile_save_failed`, which clearly says the latest progress was not saved.
  - The Session continues normally.
  - The existing `profiles.json`, if any, is unchanged.
- If an unreadable file also can't be moved aside, Game shows `profile_file_unreadable_not_moved` instead, continues with no Profiles, and the store refuses to overwrite the file: every save that Session reports failure and leaves it untouched.

**Game**
- Takes a `ProfileStore` in its constructor. The startup code passes one for the per-user folder.
- The language choice, welcome message and main menu stay where they are. Choosing **2** still quits.
- Choosing **1**:
  1. Loads the Profiles. If the file was unreadable, it shows `profile_file_unreadable`.
  2. **If there are no Profiles:** go straight to New profile.
  3. **Otherwise:** show `profile_menu_header`, then one line per Profile using `profile_menu_entry` (number, name, translated Race, translated Occupation), then a last line with the next number and `profile_new_option`. Read a number with `enter_choice`. Anything else shows `invalid` and shows the menu again.
- **New profile:**
  1. Character creation runs as today, with the name check above.
  2. When creation finishes (after the player picks "proceed" on the next-action menu), Game saves the character.
  3. Then the path choice.
- **Existing Profile:**
  1. Show `profile_welcome_back` with the character's name.
  2. Show the character stats screen.
  3. Then the path choice. The next-action menu isn't shown.
- **Merchant:** when the Merchant Encounter returns, Game saves the character, then goes back to the path choice. The Merchant itself doesn't know about storage.
- **Dead Ends and Exit:** unchanged. The Session ends and the program closes. Game doesn't save after a Dead End or on Exit.
- **On save failure:** Game shows `profile_save_failed` and carries on.

**Player**
- Character creation needs the name check. The name prompt gets a way to ask whether a name is taken, for example an optional check passed into character creation. Without one, creation behaves exactly as today, so existing callers and tests are unaffected.
- No other change. The stats screen already shows Gold, Protection and Weapon.

**Localization**
- English, Spanish, French and Italian all get every new key. There are no accented characters, following ADR 0001.
- **New keys:**
  - `profile_menu_header`
  - `profile_menu_entry` (`{0}` number, `{1}` name, `{2}` Race, `{3}` Occupation; for example `1. Bob (Human Fighter)`)
  - `profile_new_option`
  - `profile_name_taken`
  - `profile_welcome_back` (`{0}` name)
  - `profile_file_unreadable` (`{0}` backup file name)
  - `profile_file_unreadable_not_moved`
  - `profile_save_failed`
- **Rewritten key:** `menu` describes option 1 as playing (choosing or creating a character) rather than only "Create Character", and still lists option 2 as Exit.

## Testing Decisions

- **Red-green-refactor is required.** For every behavior, write a failing test first and run it to see it fail for the right reason. Then write the least code needed to pass it, then refactor with all tests green. No production code is written before a test that needs it.
- **Test external behavior only.** Check what the store returns, what is on disk afterwards, and what the game prints. Don't test private helpers, the exact JSON layout beyond the `version` field and backup file names, or dice results.
- **Two seams, both agreed in the interview:**
  1. **`ProfileStore`, tested directly with a temporary folder.**
     - Each test makes its own new temporary folder and deletes it in cleanup.
     - No test ever uses the real per-user folder.
     - Cases:
       - Loading from an empty or missing folder returns no Profiles and no warning.
       - A fresh character round-trips: name, Race, Occupation, Strength, Agility, Health Points, Gold, Protection 0, and its Weapon.
       - A character with Magical Armor and a Magical Weapon round-trips: Protection 3, a "magical …" type, the higher Max Damage, ASCII art kept, and magical true.
       - Profiles load in creation order.
       - Saving a name that already exists replaces that Profile in place, without adding a second one.
       - The name check ignores upper and lower case and surrounding spaces.
       - A damaged file is moved to `profiles.unreadable.json`, loading reports that name, and no Profiles are returned.
       - With `profiles.unreadable.json` already present, the next damaged file goes to `profiles.unreadable-2.json`, and both backups keep their original contents.
       - A file with `version` 2 is treated as unreadable.
       - Saving when the folder can't be created reports failure without throwing. For example, the folder path points through an existing file.
       - A failed save leaves an existing `profiles.json` unchanged.
  2. **`Game.StartGame`, driven end to end by scripted console input.**
     - Uses `Console.SetIn` with a `StringReader` and `Console.SetOut` with a `StringWriter`, restored in cleanup, under `[DoNotParallelize]`.
     - `Game` is built with a `ProfileStore` for a temporary folder.
     - Each test has a timeout, so a script that runs out of input fails instead of hanging. Every script ends by exiting, at the main menu or with `e` at the path choice.
     - Dice are avoided by setting up saved Profiles with known values through the store before the test.
     - Cases:
       - With no Profiles, choosing **1** goes straight to character creation. After creating a character and exiting, the store holds one Profile with the typed name, Race and Occupation.
       - With saved Profiles, the menu lists each one as number, name, Race and Occupation in the chosen language (checked in English and one other language), with New profile last.
       - An invalid profile-menu choice shows `invalid` and the menu again.
       - Creating a Profile with a name already taken, in different case, shows `profile_name_taken`. The next name is accepted without asking for the Race again.
       - Choosing a saved Profile shows `profile_welcome_back` with its name and its stats, including its saved Health Points, then the path prompt.
       - **Merchant save:** a saved Profile with 20 Gold buys Magical Armor, leaves the Merchant and exits. Loading again shows 12 Gold and Protection 3.
       - Retreating before the fight on the North or West path leaves the saved Profile unchanged.
       - A damaged `profiles.json` shows `profile_file_unreadable` with the backup name, and goes straight to character creation.
       - A store whose folder can't be created shows `profile_save_failed` after character creation, and the Session continues to the path prompt.
- **Health Points rule:** the Merchant-save and retreat tests check that the saved Health Points equal the Profile's original value. Combat damage is also tested without controlling the dice, by picking saved stats that make the outcome certain:
  - **Defeat:** a saved Profile with Strength 0 always misses, and Agility 0 never blocks, so Grubnik eventually wins.
  - **Wounded victory:** a saved Profile with Strength 20 always hits, Agility 0, and very high Health Points beats Grubnik, whose Agility is at most 12.
  - Combat keeps fighting when scripted input runs out, so neither test can stall on input.
  - After each, loading again shows the saved Health Points unchanged.
- **Translations:** in `MessagesTests`, add every new key to the required-key lists for English, Spanish, French and Italian. Check that `profile_menu_entry`, `profile_welcome_back` and `profile_file_unreadable` format their placeholders in every language.
- **Regression:** all existing tests keep passing. Existing `new Game()` uses in tests keep working, through a constructor that doesn't touch storage, or are updated to pass a store for a temporary folder. Character creation without a name check behaves exactly as before.
- **Not unit tested:** the startup code that builds the real store for the per-user folder. It's covered by a manual playthrough:
  1. Create a character, shop at the Merchant, and exit.
  2. Restart, pick the Profile, and confirm the welcome back, the stats and the purchases.
  3. Go to a Dead End, restart, and confirm the Profile is still there with full Health Points.
  4. Repeat the menu check in one other language.
- **Prior art:**
  - Console redirection and `[DoNotParallelize]` in `GoblinTests` and `GamePbi18Tests`.
  - Players and Weapons built with object initializers in `MerchantTests` and `CombatTests`.
  - The required-key lists for each language in `MessagesTests`.
  - Constructor injection, as `Dragon` and `Goblin` receive the shared Die.

## Out of Scope

- Renaming or deleting Profiles.
- More than one character per Profile.
- Saving the language in a Profile. The language is still asked first, every time.
- Healing, or keeping damaged Health Points between Sessions.
- Any penalty for defeat, such as losing Gold or items.
- Returning to the main menu or profile menu after a Session. The program still closes.
- Saving during a fight, after a Dead End, or on Exit.
- Locking or merging the profile file between game windows (see Known Limitations).
- Changing character-creation rules, including the Halfling Health Points issue below.
- Letting tests control the dice.
- Moving profiles from an older format. Version `1` is the first.

## Known Limitations

- **Halfling with 0 Health Points or less:** a Halfling can be created with 0 Health Points or less (a 1 on the d20 with a −4 Strength modifier). That value is now saved, so every Session with that character starts in that state. Such a character still fights normally and loses on the first hit that gets through. Fixing it means changing character creation and is left for a later decision.
- **Two game windows at once:** each window loads `profiles.json` at startup and writes the whole file when it saves. Whichever window saves last wins, so a Profile created or updated in the other window can be lost. The game is normally played in one window at a time.
- **Weapon values are frozen in saves:** because the whole Weapon is saved, a future change to a weapon's Max Damage or ASCII art won't reach existing Profiles (ADR 0003).

## Further Notes

- ADR 0003 records the storage decisions: one `profiles.json` in the per-user folder, the whole Weapon saved, and Health Points reset every Session by saving only the rolled value.
- Domain terms (Profile, Session, Dead End, Gold, Merchant, Magical Armor, Protection, Magical Weapon, Monster) are defined in `GLOSSARY.md`. Profile and Session were added during this feature's interview. Dead End now ends the Session.
- Work should happen on a new `feature/player-profiles` branch. It builds on the goblin encounter work, because Profiles must work on the West path.
- This is the first feature that writes files. Until now the game only read `language_data.json`.
