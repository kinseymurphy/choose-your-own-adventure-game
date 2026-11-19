# 03: Protection reduces dragon damage

**What to build:** When the dragon's hit gets past the player's Agility defense, a player wearing Magical Armor takes `max(0, damage − Protection)` damage instead of the full roll. Combat shows the reduced damage and a line saying how much the armor absorbed, in all four languages. Players without armor see combat exactly as before. Hits blocked by the defense roll stay fully blocked. See the Protection section of `.scratch/merchant-encounter/spec.md`.

**Blocked by:** 02 (Merchant Encounter with Magical Armor)

**Status:** ready-for-agent

- [x] `Player.ReduceDamage(int damage)` returns `max(0, damage − Protection)`
- [x] Dragon.Attack runs the damage roll through `ReduceDamage` before subtracting Health Points; Health Points is still clamped at 0
- [x] When Protection > 0, a line saying how much the armor absorbed is added; the damage message shows the reduced amount
- [x] When Protection is 0, combat output and behavior are unchanged
- [x] The new key for the armor-absorbs message exists in all four languages with no accented characters
- [x] Unit tests for ReduceDamage: Protection 0 (unchanged); damage > Protection (reduced); damage == Protection (0); damage < Protection (0, never negative)
- [x] New key is added to the required-key tests for all four languages
- [x] All existing Dragon and Combat tests still pass
- [x] Manual check: buy armor, go North, attack, see the armor-absorbs line when a hit gets through

## Comments

Implemented: `Player.ReduceDamage` returns `Math.Max(0, damage - Protection)`. `Dragon.Attack` now sends the damage roll through it before subtracting Health Points, which are still clamped at 0. When Protection > 0, combat prints an `armor_absorbed_damage` line showing how much was absorbed (the roll minus the reduced damage, so a 2-damage hit against Protection 3 shows 2 absorbed). The damage message shows the reduced amount. Hits blocked by the defense roll return early as before. New key `armor_absorbed_damage` in English, Spanish, French and Italian (ASCII only). Automated suite: 111/111 passing.

Manual check done with scripted playthroughs, not by hand. English: bought armor, went North, saw "Your Magical Armor absorbs N damage." with the reduced damage after it, including a fully absorbed hit ("deals 0 damage", health unchanged). Italian: "La tua Armatura Magica assorbe 3 danni." No-armor run: no absorbs line, combat unchanged.

Review notes (left as is): `Protection > 0` means "has Magical Armor" in both `Dragon.Attack` and `Merchant.BuyMagicalArmor`. A `HasMagicalArmor` property could gather these if more uses appear. No unit test covers the Dragon wiring because the dice can't be controlled, as the spec intends.
