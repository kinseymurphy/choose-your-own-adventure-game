# 02: Merchant Encounter with Magical Armor

**What to build:** Taking the South path prints a rewritten narrative introducing an unnamed traveling Merchant, then opens the Merchant menu. The menu shows the player's Gold and offers `1. Magical Armor (8 Gold)`, `2. Magical Weapon (8 Gold)` and `3. Leave`, and it loops until the player chooses Leave. Buying Magical Armor costs 8 Gold and gives Protection 3. Afterwards the player returns to the path choice. The Merchant appears on every South visit. Option 2 shows the existing `invalid` message until ticket 04 adds it. Protection doesn't affect combat yet (that's ticket 03). See `.scratch/merchant-encounter/spec.md` and the Merchant, Merchant Encounter, Magical Armor and Protection entries in `GLOSSARY.md`.

**Blocked by:** 01 (Gold at character creation)

**Status:** ready-for-agent

- [ ] Player has a Protection property that defaults to 0; Protection shows on the player stats display and on the combat stats display
- [ ] New Merchant class; `BuyMagicalArmor(Player)` returns Purchased / NotEnoughGold / AlreadyOwned and never writes to the console
- [ ] Armor purchase: AlreadyOwned if Protection > 0; NotEnoughGold if Gold < 8; otherwise subtract 8 Gold, set Protection to 3, return Purchased
- [ ] `StartEncounter(Player, Messages)` shows Gold and the menu, accepts only `1`/`2`/`3`, prints a message for each purchase result, shows `invalid` for other input, and loops until `3`
- [ ] The South branch in Game runs the Merchant Encounter and then returns to the path choice; North and Exit are unchanged
- [ ] `south_path_narrative` is rewritten, and the new Merchant keys (greeting, menu, armor purchased, not enough Gold, already owned, farewell, Protection label) exist in all four languages with no accented characters
- [ ] Unit tests: armor bought with exactly 8 Gold and with more than 8 (leftover correct); refused with less than 8 (nothing changes); a second purchase is refused as AlreadyOwned (Gold unchanged); a new Player has 0 Protection
- [ ] Already owned is checked before not enough Gold (test: Protection > 0 and Gold < 8 returns AlreadyOwned)
- [ ] New keys are added to the required-key tests for all four languages
- [ ] Manual check: go South, buy armor, try again, Leave, view stats, go South again
