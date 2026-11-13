# 04: Magical Weapon upgrade

**What to build:** Merchant option 2 upgrades the player's own occupation weapon into a Magical Weapon for 8 Gold. The weapon gets 4 more Max Damage and a magical name in the player's language, and keeps its ASCII art. The upgraded name and damage show on the stats screen, the combat stats screen and in attack messages. See the Magical Weapon section of `.scratch/merchant-encounter/spec.md` and the **Magical Weapon** entry in `GLOSSARY.md`.

**Blocked by:** 02 (Merchant Encounter with Magical Armor)

**Status:** ready-for-agent

- [ ] Weapon has an `IsMagical` flag that defaults to false
- [ ] `Merchant.BuyMagicalWeapon(Player)` never writes to the console. The checks run in this order:
  - [ ] no weapon: refuse, no Gold change
  - [ ] `IsMagical`: AlreadyOwned
  - [ ] Gold < 8: NotEnoughGold
  - [ ] otherwise: subtract 8 Gold, set IsMagical, Type becomes `"magical <type>"`, MaxDamage +4, ASCII art unchanged, return Purchased
- [ ] Menu option 2 calls it and prints the matching message; the no-weapon case shows the existing `invalid` message
- [ ] `displayWeaponMap` gets four new full-name entries per language (magical long sword, magical lightning bolt spell, magical dagger, magical long bow), with correct grammatical agreement and no accented characters
- [ ] The weapon-purchased message key exists in all four languages
- [ ] Unit tests: each of the four occupation weapons upgrades to the correct magical Type and +4 MaxDamage, keeps its ASCII art and is IsMagical; exact and more than 8 Gold; less than 8 Gold refused; a second upgrade is refused as AlreadyOwned; already owned is checked before not enough Gold; no weapon is refused with Gold unchanged; buying armor then weapon with 16 Gold leaves 0 Gold and both items
- [ ] A new Weapon has `IsMagical` false
- [ ] Tests that each magical weapon type translates to a non-fallback name in all four languages; the new message key is added to the required-key tests
- [ ] Manual check: go South, buy the weapon, view stats, go North, attack, see the magical name and higher damage
