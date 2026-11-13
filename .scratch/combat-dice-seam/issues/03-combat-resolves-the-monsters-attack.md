# 03: Combat resolves the Monster's attack

**What to build:** Move the Monster's attack on the player out of `Monster.Attack` and into `Combat`, written as the shared attack sequence that ticket 04 will reuse for the player's attack. `MonsterAttacksPlayerSequence()` prints exactly the same lines as today, using the die from ticket 01. Then remove `Monster.Attack`, `RollAttack`, `AttackHits` and `RollDamage`, and `Player.ReduceDamage`, whose Protection rule moves into `Combat`. See the "Monster attacks Player" column of the combat table in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 02 (Pin today's combat with four-language fight transcripts)

**Status:** ready-for-agent

- [ ] `Combat` resolves the Monster's attack with one sequence:
  1. Attack d20 ≤ Monster Strength to hit.
  2. On a hit, defense d20 ≤ player Agility to block.
  3. When not blocked, damage d(Monster Weapon Max Damage), minus player Protection, not below 0.
  4. The player's Health Points are reduced and stop at 0.
- [ ] The sequence is written so the player's attack can reuse it in ticket 04: the attacker's Strength, the defender's Agility, the damage sides and the Protection to subtract are supplied to it, not hard-coded.
- [ ] The printed lines and their order are unchanged: `monster_attack_intro`, ASCII art only when not blank, then the miss, block or damage lines, `armor_absorbed_damage` when Protection > 0, `player_health_now`, and `player_defeated_narrative` at 0.
- [ ] Roll order is unchanged: one roll on a miss, two on a block, three on damage.
- [ ] Removed:
  - `Monster.Attack`, `Monster.RollAttack`, `Monster.AttackHits` and `Monster.RollDamage`.
  - `Player.ReduceDamage`.
- [ ] The ticket 02 transcripts pass unchanged.
- [ ] TDD: write each failing test through `MonsterAttacksPlayerSequence()` with the scripted die before moving the code it covers.
  - A miss (roll above Monster Strength) leaves the player's Health Points unchanged.
  - A block (hit, then defense roll ≤ player Agility) leaves them unchanged and shows `player_defended_monster_attack`.
  - A damaging hit reduces them by the damage roll.
  - Protection 3 with damage roll 7 reduces them by 4 and shows 3 absorbed. With damage roll 2, they're reduced by 0 and it shows 2 absorbed.
  - A hit for more than the player's Health Points leaves 0 and shows `player_defeated_narrative`.
- [ ] Replace, don't layer. Delete the tests these cover, and list each deleted test under `## Comments`:
  - The `ReduceDamage_*` tests in `PlayerTests`.
  - The `Attack_*` tests in `DragonTests`.
  - The `Attack_*` tests in `GoblinTests`, including `Attack_PlayerWithMagicalArmor_AbsorbsGoblinDamage`.
- [ ] The full test suite passes.
