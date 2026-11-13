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

- [ ] Run the whole test suite before starting and record the result. It must be green.
- [ ] Monster is an abstract base class. Dragon inherits from it, and its name (Smolderfang), 1d20 stats and claws (Max Damage 12, same ASCII art) are unchanged.
- [ ] Monster's `Attack` keeps the dragon's current rules in order:
  1. Roll to hit against Strength.
  2. Roll the player's defense against Agility.
  3. Send the damage through `player.ReduceDamage`.
  4. Clamp Health Points at 0.
  5. Add the armor-absorbed line when Protection > 0.
  6. Add the defeated line at 0 Health Points.
- [ ] Each Monster supplies its own keys for its attack intro, stats header, defeated narrative, taunts, damage replies and endings. The Dragon's are the existing `player_attack_intro`, `dragon_stats_header`, `dragon_defeated_narrative`, `dragon_defense_taunt_1`–`4`, `dragon_damage_reply_1`–`4`, `victory` and `defeat`.
- [ ] `Player.Attack` takes a Monster and uses that Monster's keys for its attack intro, defense taunt, damage reply and defeated narrative.
- [ ] `Combat` takes a Player and a Monster. Its public `dragon` property becomes `monster`.
- [ ] The victory and defeat endings come from the Monster that was fought. `retreat` stays shared.
- [ ] No message keys or language text change in this ticket.
- [ ] TDD:
  - Write the new tests first and watch them fail: the Dragon is a Monster, and `Combat` stores the Monster in `monster`.
  - Update `CombatTests` references from `combat.dragon` to `combat.monster` as part of the red step.
  - Make them pass, then refactor with everything green.
- [ ] All existing `DragonTests`, `CombatTests`, `PlayerTests`, `GameTests`, `MerchantTests` and dragon encounter tests still pass, apart from the `dragon` → `monster` property rename.
- [ ] Manual check: go North and fight. The output reads exactly as it did before this ticket.
