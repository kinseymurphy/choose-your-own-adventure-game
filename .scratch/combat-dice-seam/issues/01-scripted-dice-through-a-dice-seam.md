# 01: Script the dice through a dice seam

**What to build:** Add a one-method dice interface, `Roll(sides)`. The existing random `Die` implements it, and a scripted die in `CYOATests` returns listed rolls in order. `Monster`, `Dragon`, `Goblin` and `Combat` take the interface. `Game` takes an optional die, defaulting to a random `Die`, and passes the same die to the dragon, the goblin and every `Combat`. No combat logic moves in this ticket, and the game plays exactly as before. See "The dice seam" and "Who gets the die" in `.scratch/combat-dice-seam/spec.md`, and ADR 0004.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] A dice interface with one method, `Roll(sides)`, returning 1 to `sides`. Name it after the existing `Die` class, for example `IDie`.
- [ ] `Die` implements it with no change in behavior.
- [ ] A scripted die exists only in `CYOATests`:
  - It returns the given rolls in order.
  - When it runs out, it fails the test with a clear message.
  - When a scripted roll is outside 1 to `sides`, it fails the test with a clear message naming the roll and the sides.
- [ ] `Monster`, `Dragon` and `Goblin` constructors take the interface. Their stat rolls go through it, in the same order as today.
- [ ] `Combat`'s constructor takes the die. In this ticket it's only stored; the attacks still use the old methods.
- [ ] `Game` takes an optional die, defaulting to a new random `Die`. The existing `Game()` and `Game(ProfileStore?)` callers and `Program.cs` keep working.
- [ ] Existing tests that build `Dragon`, `Goblin` or `Combat` are updated only as much as the new constructors need. They keep testing the same things.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **Scripted die:** returns rolls in order; fails when it runs out; fails on a roll of 0 and on a roll above `sides`.
  - **Monster stats:** a `Dragon` built with scripted rolls 15, 9, 18 has Strength 15, Agility 9 and Health Points 18. A `Goblin` built with 3, 12, 7 has 3, 12 and 7.
  - **Game:** a `Game` given a scripted die creates its dragon and goblin from it. For example, the West path shows Grubnik's scripted stats, then exits by retreating.
- [ ] The full test suite passes.

## Comments

**Done on 2026-11-13 by Claude:**

- `dotnet test CYOATests` passed 254 of 254 tests. The suite was 246 before this ticket, so it adds 8 tests:
  - 4 in `ScriptedDieTests`;
  - 1 each in `DragonTests` and `GoblinTests` for stats from scripted rolls;
  - 2 in the new `GameDiceTests` for the North and West paths showing scripted Monster stats.
- The dice interface is `IDie`, in `Die.cs` next to `Die`. The scripted die is `CYOATests/ScriptedDie.cs`.
- `Game` gained `Game(ProfileStore?, IDie)`. `Game()` and `Game(ProfileStore?)` default to a random `Die`, so `Program.cs` is unchanged. The die is now created when the `Game` is constructed instead of at the start of `StartGame`, which makes no difference to play.
- `Combat` only stores the die. Its 10 constructor calls in `CombatTests` pass `new Die()`, matching how those tests already build their Monsters, and none of them roll.
- No tests were deleted. `NewGoblin_RollsStatsFrom1To12` (200 random goblins) still passes. It could become a scripted-die test later, because the scripted die rejects a roll above 12 on a d12.
