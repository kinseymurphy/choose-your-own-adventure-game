# ADR 0003: Store Profiles locally in one JSON file

- **Status:** Accepted
- **Date:** 2026-11-13
- **Deciders:** Kinsey Murphy
- **Branch / commit:** `feature/player-profiles`, commit "Save characters as Player Profiles between Sessions". Spec: `.scratch/player-profiles/spec.md`

## Context

Players should be able to come back to their character. A **Profile** holds one character: name, Race, Occupation, Strength, Agility, Health Points, Gold, Protection and Weapon. The game starts each **Session** from a Profile or creates a new one.

Until now the game never wrote a file. It only reads `language_data.json` from the program's folder, using `System.Text.Json`. Some facts about the code shaped the choices below:

- `Player` and `Weapon` are plain data with public properties, so `System.Text.Json` can save and load them without changes.
- The Merchant records Magical Armor only as Protection above 0. It records a Magical Weapon by changing the Weapon itself: it sets it as magical, adds "magical " to its type, and adds 4 to its Max Damage.
- Health Points only drop during a Dead End fight, and a Dead End always ends the Session. Nothing in the game heals.
- Character names are free text. They may contain characters Windows doesn't allow in file names, and Windows file names ignore case.
- The program's own folder is the build output folder. Git ignores it, and it's different for Debug and Release builds.

## Decision

1. **One `profiles.json` file in the per-user local application data folder** (`ChooseYourAdventure` inside `%LOCALAPPDATA%` on Windows), written with `System.Text.Json`.
   - The file has a `version` number, starting at `1`, and holds every Profile in creation order.
   - Saves go to a temporary file first, which then replaces `profiles.json`.
   - An unreadable file is moved to a numbered backup (`profiles.unreadable.json`, `-2`, `-3` and so on) and never overwritten.
   - The folder is given to the storage class, so tests use a temporary folder.
2. **The whole Weapon is saved:** type, Max Damage, ASCII art and whether it's magical. It isn't rebuilt from Occupation on load.
3. **Health Points reset every Session.** A Profile saves only the Health Points rolled at creation. The game saves only when character creation finishes and when the player leaves the Merchant, both before any combat damage can happen. Combat-damaged Health Points must never overwrite the saved value, and no save may be added after combat without revisiting this decision.

## Options considered

**Where and how to store**

| Option | Result |
|---|---|
| **One file in the per-user folder (chosen)** | Survives rebuilds and Debug/Release switches. Names are just strings, so any name works and uniqueness is a simple list check. One write covers all Profiles, so a failed write needs care: it goes to a temporary file first, and unreadable files are backed up. |
| One file per Profile | Names would have to be turned into safe file names. "Bob" and "bob" would collide on Windows. Listing Profiles means scanning a folder. Rejected. |
| A file next to the program | Matches where `language_data.json` lives, but that's the build output folder: ignored by git, separate for Debug and Release, so saves would seem to disappear. Rejected. |
| A database (for example SQLite) | Adds a NuGet package and a lot of machinery for a handful of records. Rejected. |

**How to save the Weapon**

| Option | Result |
|---|---|
| **Save the whole Weapon (chosen)** | No changes to `Player` or `Weapon`, and what was bought comes back exactly. Weapon values are frozen in each save. |
| Save Occupation plus "is magical", and rebuild on load | Saves would pick up future weapon changes. But the occupation-to-weapon table is private to `Player`, and the Merchant's upgrade would have to be repeated on load. Rejected as more code for no current benefit. |

**What happens to Health Points between Sessions**

| Option | Result |
|---|---|
| **Reset to the rolled value every Session (chosen)** | No healing code, no extra field, and no "defeated with 0 HP" state to recover from. Relies on never saving after combat. |
| Keep damage between Sessions | A defeated character would come back at 0 HP and need a separate recovery rule. Rejected. |
| Keep damage, and let the Merchant sell healing | A new Merchant item and text in four languages. A feature in its own right. Rejected for now. |
| Reset only after certain outcomes | Adds rules per Dead End outcome for little gain. Rejected. |
| Store a separate "rolled Health Points" field | Safer if a save after combat is ever added, but it changes `Player` and isn't needed while saves only happen before combat. Rejected for now. |

## Consequences

**Positive**

- Players keep their character, Gold, Magical Armor and Magical Weapon between Sessions.
- No new NuGet packages. `System.Text.Json` is already used for the language file.
- Profiles work in every language, because Race, Occupation and weapon types are stored as the internal English names.
- Tests never touch real player data, because the folder is passed in.
- A damaged file or failed save never destroys data that was already saved.

**Negative / risks**

- Once players have save files, changing the file layout, the location or what is stored needs a migration. The `version` number makes that possible but doesn't provide it.
- Weapon values are frozen in existing saves. A future change to a weapon's Max Damage or ASCII art won't reach them.
- The Health Points reset depends on a rule rather than a field: adding a save point after combat would quietly save damaged Health Points. The spec and this ADR forbid that, but nothing in the code enforces it.
- Two game windows open at once can overwrite each other's Profiles. This is accepted as a known limitation.
- The profile file is in a hidden per-user folder, so it's harder to find when inspecting or grading the game.

## Verification

Verified on 2026-11-13.

- **`dotnet test CYOATests`:** 246 passed, 0 failed. The feature added 49 tests: 26 in `ProfileStoreTests` and 19 in `ProfileGameTests`, with the rest in `MessagesTests`. The scripted `ProfileGameTests` passed 8 runs in a row, which checks that the dice-dependent combat tests aren't flaky.
- **Health Points rule:** a temporary save was planted after the Dead Ends as a check. The defeat, wounded-victory and retreat tests all failed against it, then the save was removed.
- **Scripted playthroughs of the real program:** these used the real `%LOCALAPPDATA%\ChooseYourAdventure` folder, which didn't exist beforehand and was deleted afterwards.
  1. A character was created and Magical Armor bought with all 8 Gold. After a restart, the game welcomed the character back with Gold 0 and Protection 3.
  2. After a North Dead End and a restart, the Profile was still listed with its rolled Health Points (19).
  3. `" KIRA "` was refused for a new Profile because `Kira` already existed.
  4. In Spanish, the menu showed `1. Kira (Humano Guerrero)`, `2. Lio (Elfo Arquero)` and `3. Nuevo perfil`, and picking a Profile showed the Spanish welcome back.
  5. A hand-damaged `profiles.json` was moved to `profiles.unreadable.json` with its contents intact. The warning named that file, and character creation started.
