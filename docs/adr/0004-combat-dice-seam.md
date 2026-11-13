# ADR 0004: Resolve attacks in Combat behind a dice seam

- **Status:** Accepted
- **Date:** 2026-11-13
- **Deciders:** Kinsey Murphy
- **Branch / commit:** `feature/combat-dice-seam`:
  - `3bc9c11` adds the dice seam;
  - `0a83203` adds the transcripts;
  - `daf8ada` moves the Monster's attack into `Combat`;
  - `bb78801` moves the player's attack into `Combat`;
  - the commit "Verify the combat dice seam end to end and accept ADR 0004" records the verification.

  Spec: `.scratch/combat-dice-seam/spec.md`

## Context

ADR 0002 gave every Monster one combat system through the `Monster` base class. But the rules for a single attack still live in two places:

- `Player.Attack` resolves the player's attack on a Monster.
- `Monster.Attack` resolves a Monster's attack on the player.

Each one rolls to hit, rolls the defender's defense, rolls damage, updates Health Points and builds its message lines. The copies already differ:

- A Monster's Health Points can go below 0, but the player's stop at 0.
- Protection only applies to the player.
- ASCII art is printed unconditionally for the player but only when it isn't blank for a Monster.
- `Player.Attack` has a hard-coded English message for having no Weapon.

`Combat` only loops over the two methods and prints their strings.

The dice can't be controlled. `Die` wraps `new Random()`, and `Player` creates its own private `Die`. Tests force outcomes by rigging stats (Strength 0 always misses, Health Points 1000 always survives), by repeating checks, or by re-running the suite to show it isn't flaky. ADR 0003's verification relied on all three.

## Decision

1. **A dice seam.** Combat dice go through a one-method interface, `Roll(sides)`, with two implementations:
   - the existing random `Die`, used by the game;
   - a scripted die used only by tests, which returns listed rolls in order and fails the test if it runs out or a roll is impossible.

   `Combat`, `Monster` and its subclasses, and `Game` receive the die; `Game` defaults to a random one. Character creation's rolls stay inside `Player` for now.
2. **`Combat` owns the attack.** One shared sequence resolves both directions: attack, defense, damage, Protection and Health Points. It prints the same message lines as today. `Player.Attack`, `Monster.Attack` and their roll helpers are removed, so `Player` and `Monster` hold stats and message keys.
3. **Behavior is preserved exactly.** Every message in every language, the roll order, the first round before the prompt and the mid-fight retreat (ADR 0002) stay the same. The inconsistencies listed under Context are kept and written up as follow-up work rather than fixed here.

## Options considered

**Where the attack rules live**

| Option | Result |
|---|---|
| **`Combat` owns both directions (chosen)** | One place for the rules, and tests go through the module the game uses. `Player` and `Monster` shrink. |
| Keep `Player.Attack` and `Monster.Attack`, sharing a private helper | Less movement, but two entry points to test and keep in step. Rejected. |
| A new combatant base class or interface shared by `Player` and `Monster` | Would let one method attack either way, but it reshapes `Player`, the record ADR 0003 saves, for a difference that two explicit directions handle more simply. Rejected. |

**How tests control the dice**

| Option | Result |
|---|---|
| **An interface with a random and a scripted implementation (chosen)** | Tests list the exact rolls. Two implementations make it a real seam. |
| A seeded `Random` | No new interface, but tests would depend on whatever a seed happens to produce, and would break if the roll order changed. Rejected. |
| Keep rigging stats | No code change, but blocks, specific damage rolls and Protection edge cases stay untestable. Rejected. |

**What to do about the inconsistencies**

| Option | Result |
|---|---|
| **Preserve them and write them up (chosen)** | The refactor can be checked by output alone. Each change is decided on its own. |
| Fix them during the refactor | Mixes behavior changes into a structural change, so transcript tests can't tell an intended change from a mistake. Rejected. |

## Consequences

**Positive**

- A combat rule is changed in one place for both directions.
- Tests can script a miss, a block, a specific damage roll, Protection absorbing part or all of a hit, and a defeat.
- Whole Dead End fights can be tested end to end without rigging stats.
- Future Monsters get the same rules with no extra work.

**Negative / risks**

- Many tests move or are rewritten. The old tests for the removed methods are deleted once tests through `Combat` cover the same behavior.
- The roll order becomes part of what tests depend on. Changing the order later means updating the scripted rolls in tests, which is intended.
- Character creation still uses its own random `Die`, so it remains untestable without console scripting.
- The inconsistencies remain visible to players until the follow-up ticket is decided.

## Verification

Verified on 2026-11-13.

- **`dotnet test CYOATests`:** 287 passed, 0 failed, no warnings. The suite was 246 before this work.
  - 72 tests were added.
  - 31 tests of the removed methods were deleted. Each was replaced by a test through `Combat` with scripted rolls, and the replacements are listed in tickets 03 and 04.
- **Transcripts unchanged:**
  - All 10 transcripts pass, with no edits since they were approved in ticket 02 (`git diff 0a83203 -- CYOATests/CombatTranscripts` is empty). They cover a dragon victory and a goblin defeat in English, Spanish, French and Italian, a mid-fight retreat and a Magical Weapon victory.
  - They passed after each step: when the Monster's attack moved (ticket 03), and when the player's attack moved and the temporary reflection was deleted (ticket 04). Ticket 04 needed no change to the roll scripts.
- **Language data unchanged:** `git diff feature/player-profiles -- language_data.json` is empty.
- **Whole-game Dead Ends with scripted dice** (`GameDiceTests`, through `Game.StartGame` with Profile saving in a temporary folder):
  - **North:** a saved Profile is wounded (30 to 24 Health Points), then defeats Smolderfang. The output ends with the dragon victory ending.
  - **West:** Grubnik defeats a saved Profile. The output ends with the goblin defeat ending.
  - **Both:** every scripted roll is used, and `profiles.json` is byte-for-byte unchanged afterwards, so ADR 0003's Health Points rule still holds.
- **ADR 0002 quirks unchanged:** the `dragon-retreat` transcript still shows a full round before the first prompt, an invalid answer playing another round, and a mid-fight retreat with no ending.
- **Not run:** playthroughs of the built program. `Program.cs` always uses the real `%LOCALAPPDATA%\ChooseYourAdventure` folder, and Kinsey chose not to have it inspected or changed during verification. The whole-game tests run the same `Game.StartGame` code with scripted input. A manual playthrough is still listed as a check for a person in ticket 05.
