# 01: Introduce the Monster base class

**What to build:** A prefactor that makes the Goblin Encounter an easy change, following ADR 0002. The dragon becomes one kind of **Monster**. A new abstract Monster holds what every Monster shares:

- Name, Strength, Agility, Health Points and Weapon.
- The shared Die.
- Rolling to attack and for damage.
- `Attack(Player, Messages)`.
- Random defense taunts and damage replies.
- `DisplayStats`.

Each Monster names its own message keys for its player-attack intro, stats header, defeated narrative, four taunts, four damage replies, and victory and defeat endings. The Dragon uses exactly the keys it uses today.

`Combat` and `Player.Attack` take a Monster instead of a Dragon. `Combat`'s public `dragon` property becomes `monster`. The ending shown after combat comes from the Monster that was fought.

This is a pure refactor. The dragon looks, talks and fights exactly as before. See the Monster, Dragon, Player, Combat and Game sections of `.scratch/goblin-encounter/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [x] Run the whole test suite before starting and record the result. It must be green.
- [x] Monster is an abstract base class. Dragon inherits from it, and its name (Smolderfang), 1d20 stats and claws (Max Damage 12, same ASCII art) are unchanged.
- [x] Monster's `Attack` keeps the dragon's current rules in order:
  1. Roll to hit against Strength.
  2. Roll the player's defense against Agility.
  3. Send the damage through `player.ReduceDamage`.
  4. Clamp Health Points at 0.
  5. Add the armor-absorbed line when Protection > 0.
  6. Add the defeated line at 0 Health Points.
- [x] Each Monster supplies its own keys for its attack intro, stats header, defeated narrative, taunts, damage replies and endings. The Dragon's are the existing `player_attack_intro`, `dragon_stats_header`, `dragon_defeated_narrative`, `dragon_defense_taunt_1`–`4`, `dragon_damage_reply_1`–`4`, `victory` and `defeat`.
- [x] `Player.Attack` takes a Monster and uses that Monster's keys for its attack intro, defense taunt, damage reply and defeated narrative.
- [x] `Combat` takes a Player and a Monster. Its public `dragon` property becomes `monster`.
- [x] The victory and defeat endings come from the Monster that was fought. `retreat` stays shared.
- [x] No message keys or language text change in this ticket.
- [x] TDD:
  - Write the new tests first and watch them fail: the Dragon is a Monster, and `Combat` stores the Monster in `monster`.
  - Update `CombatTests` references from `combat.dragon` to `combat.monster` as part of the red step.
  - Make them pass, then refactor with everything green.
- [x] All existing `DragonTests`, `CombatTests`, `PlayerTests`, `GameTests`, `MerchantTests` and dragon encounter tests still pass, apart from the `dragon` → `monster` property rename.
- [x] Manual check: go North and fight. The output reads exactly as it did before this ticket.

## Comments

Implemented. New abstract `Monster` holds Name, Strength, Agility, Health Points, Weapon, the shared Die, the attack/damage rolls, `Attack`, taunts, damage replies and `DisplayStats`. Each Monster supplies its own keys through abstract properties (`PlayerAttackIntroKey`, `StatsHeaderKey`, `DefeatedNarrativeKey`, `VictoryKey`, `DefeatKey`, and the taunt and reply key lists). `Dragon` now only sets its name, claws, 1d20 stats and existing keys. `Combat` and `Player.Attack` take a Monster, and `Combat.dragon` is now `monster`.

`Game.EndGame` now takes the Monster that was fought as its first parameter, so it can show that Monster's victory or defeat ending. The 11 `EndGame` tests in `GameTests` were updated to pass a Dragon. Endings are unchanged.

TDD: `Dragon_IsAMonster` and the renamed `Combat_Constructor_InitializesPlayerAndMonster` failed to compile first, then passed. Suite: 138/138 before, 139/139 after. Scripted North fight: output identical to before.
