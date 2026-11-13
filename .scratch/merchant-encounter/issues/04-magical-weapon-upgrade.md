# 04: Magical Weapon upgrade

**What to build:** Merchant option 2 upgrades the player's own occupation weapon into a Magical Weapon for 8 Gold. The weapon gets 4 more Max Damage and a magical name in the player's language, and keeps its ASCII art. The upgraded name and damage show on the stats screen, the combat stats screen and in attack messages. See the Magical Weapon section of `.scratch/merchant-encounter/spec.md` and the **Magical Weapon** entry in `GLOSSARY.md`.

**Blocked by:** 02 (Merchant Encounter with Magical Armor)

**Status:** ready-for-agent

- [x] Weapon has an `IsMagical` flag that defaults to false
- [x] `Merchant.BuyMagicalWeapon(Player)` never writes to the console. The checks run in this order:
  - [x] no weapon: refuse, no Gold change
  - [x] `IsMagical`: AlreadyOwned
  - [x] Gold < 8: NotEnoughGold
  - [x] otherwise: subtract 8 Gold, set IsMagical, Type becomes `"magical <type>"`, MaxDamage +4, ASCII art unchanged, return Purchased
- [x] Menu option 2 calls it and prints the matching message; the no-weapon case shows the existing `invalid` message
- [x] `displayWeaponMap` gets four new full-name entries per language (magical long sword, magical lightning bolt spell, magical dagger, magical long bow), with correct grammatical agreement and no accented characters
- [x] The weapon-purchased message key exists in all four languages
- [x] Unit tests: each of the four occupation weapons upgrades to the correct magical Type and +4 MaxDamage, keeps its ASCII art and is IsMagical; exact and more than 8 Gold; less than 8 Gold refused; a second upgrade is refused as AlreadyOwned; already owned is checked before not enough Gold; no weapon is refused with Gold unchanged; buying armor then weapon with 16 Gold leaves 0 Gold and both items
- [x] A new Weapon has `IsMagical` false
- [x] Tests that each magical weapon type translates to a non-fallback name in all four languages; the new message key is added to the required-key tests
- [x] Manual check: go South, buy the weapon, view stats, go North, attack, see the magical name and higher damage

## Comments

Implemented: `Weapon.IsMagical` (default false). `Merchant.BuyMagicalWeapon` checks no weapon, then already magical, then Gold < 8. A purchase subtracts 8 Gold, sets `IsMagical`, prefixes Type with `"magical "` and adds `MagicalWeaponDamageBonus` (4) to MaxDamage. AsciiArt is left alone. A new `PurchaseResult.NoWeapon` covers the no-weapon case, and the menu shows the existing `invalid` message for it. Menu option 2 prints `merchant_weapon_purchased` with the translated weapon name and the new Max Damage. The message is worded so no article has to agree with the weapon's gender. Four magical names per language in `displayWeaponMap` (ASCII only), with tests that check the exact translated name for each language. Automated suite: 138/138 passing.

Manual check done with scripted playthroughs. English Fighter: bought the weapon (20 -> 12 Gold), a second purchase was refused as already owned, and the combat stats and attack line showed "magical long sword" with 16 damage and the `-)=====>` art. Italian Magician: "incantesimo del fulmine magico", 16 damage, `zap~~~~~~` art.

Note: the English magical-name test rows pass even without the map entries, because the fallback returns the English Type unchanged. Spanish, French and Italian rows do catch missing entries.

Review notes (left as is): `PurchaseResult.NoWeapon` is a fourth value beyond the spec's Purchased / NotEnoughGold / AlreadyOwned. The spec says a no-weapon purchase is refused but doesn't say how to report it. `BuyMagicalWeapon` sets three Weapon fields, so a `Weapon.MakeMagical(bonus)` method could own that. Menu options 1 and 2 have the same buy-then-print shape. With two items, both are fine. The `"magical "` Type prefix duplicates `IsMagical`, which is what the spec asked for. Fixed during review: the purchase message no longer says "enchants" (GLOSSARY avoids "enchanted weapon"), and all four weapon tests now check the result and Gold.
