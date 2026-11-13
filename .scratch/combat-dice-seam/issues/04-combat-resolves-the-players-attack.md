# 04: Combat resolves the player's attack with the same sequence

**What to build:** Move the player's attack on a Monster out of `Player.Attack` and into `Combat`, through the same shared sequence ticket 03 introduced. Picking a taunt or damage reply also moves into `Combat` and uses the die. `PlayerAttacksMonsterSequence()` prints exactly the same lines as today, including today's inconsistencies:
- the Monster's Health Points aren't clamped;
- ASCII art is always printed;
- the English "no weapon" text is kept.

Then remove `Player.Attack`, `RollAttack`, `AttackHits` and `RollDamage`, and `Monster.GetRandomDefenseTaunt` and `GetRandomDamageReply`. After this ticket, `Combat` is the only place an attack is resolved. See the "Player attacks Monster" column of the combat table in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 03 (Combat resolves the Monster's attack)

**Status:** ready-for-agent

- [ ] The player's attack uses the same shared sequence as the Monster's attack. The direction only supplies:
  - player Strength;
  - Monster Agility;
  - player Weapon Max Damage;
  - Protection 0;
  - no Health Points clamp.
- [ ] Picking a taunt or damage reply uses a roll from the die after the attack sequence's rolls, as today. The Monster's taunt and damage-reply key lists are readable by `Combat`.
- [ ] The printed lines and their order are unchanged:
  1. `monster.PlayerAttackIntroKey` with translated Occupation and Weapon.
  2. The ASCII art, always.
  3. The miss line, or the hit and block lines with a taunt (translated Race), or `player_damage_dealt` and `monster_health_now`.
  4. After damage, the defeated narrative at 0 or below, otherwise a damage reply.
- [ ] With no Weapon, the player's attack prints the same English "You have no weapon to attack with." text and uses no rolls.
- [ ] Removed:
  - `Player.Attack`, `Player.RollAttack`, `Player.AttackHits` and `Player.RollDamage`.
  - `Monster.GetRandomDefenseTaunt` and `Monster.GetRandomDamageReply`.
- [ ] `Player`'s private `Die` is used only by character creation.
- [ ] The ticket 02 transcripts pass unchanged.
- [ ] TDD: write each failing test through `PlayerAttacksMonsterSequence()` with the scripted die before moving the code it covers.
  - A miss leaves the Monster's Health Points unchanged and uses one roll.
  - A block shows the scripted taunt for each Monster. For example, with taunt roll 2 the dragon shows `dragon_defense_taunt_2` and Grubnik shows `goblin_defense_taunt_2`, with the translated Race filled in.
  - A damaging hit shows the scripted damage reply and reduces the Monster's Health Points by the damage roll.
  - A Magical Weapon's Max Damage is the sides of the damage roll. The scripted die rejects a roll above it.
  - A final hit can leave the Monster's Health Points below 0 (preserved), and shows its defeated narrative.
  - Each Monster's own intro key is used, checked in all four languages.
- [ ] Replace, don't layer. Delete the tests these cover, and list each deleted test under `## Comments`:
  - The `PlayerAttack_*` tests in `GoblinTests`.
  - The `GetRandomDefenseTaunt_*` and `GetRandomDamageReply_*` tests in `DragonTests` and `GoblinTests`.
- [ ] The full test suite passes.
