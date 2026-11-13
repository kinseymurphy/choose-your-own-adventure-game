# 04: Merchant purchases are saved

**What to build:** Each time the player leaves the Merchant and is back at the path choice, the game saves the character. Its Profile is updated in place, never added a second time. After a restart, the player still has the Gold they had left, their Magical Armor (Protection 3) and their Magical Weapon (its "magical …" name, higher Max Damage and original ASCII art), and the Merchant tells them they already own those items. Combat damage never reaches the saved Profile: the saved Health Points stay the value rolled at creation after a victory, a defeat or a retreat, so every Session starts at full Health Points. See the Health Points rule, ProfileStore and Game sections of `.scratch/player-profiles/spec.md`, and ADR 0003.

**Blocked by:** 02 (Pick a saved Profile and resume at the path choice). The tests start from a saved Profile with known Gold.

**Status:** ready-for-agent

- [ ] Saving a character whose name already has a Profile replaces that Profile in place, keeping its position in the list.
- [ ] Game saves the character when the Merchant Encounter returns, before the path prompt. The Merchant doesn't know about storage.
- [ ] Game still doesn't save after a Dead End (victory, defeat or retreat) or on Exit.
- [ ] Health Points rule: Health Points are only saved at the two save points (creation finished and leaving the Merchant), both before any combat damage. Combat-damaged Health Points never overwrite the saved value.
- [ ] TDD: write each failing test before the code that makes it pass.
  - **`ProfileStore` tests** (temporary folder):
    - A character with Magical Armor and a Magical Weapon round-trips: Protection 3, the "magical …" type, the higher Max Damage, the ASCII art and magical true.
    - Saving a name that already exists replaces it in place without adding a second Profile.
  - **Scripted `StartGame` tests** (redirected console, `[DoNotParallelize]`, timeout, temporary folder, Profiles saved through the store first):
    - **Merchant save:** a saved Profile with 20 Gold and Protection 0 is picked, goes South, buys Magical Armor, leaves the Merchant and exits. Loading again shows 12 Gold, Protection 3, the same Health Points and still one Profile.
    - **Already owned after restart:** a saved Profile with Protection 3 and a magical Weapon is told both are already owned, and its Gold is unchanged.
    - **Retreat:** a saved Profile goes North and retreats before the fight. The saved Profile is unchanged.
    - **Defeat doesn't overwrite Health Points:** a saved Profile with Strength 0 (always misses), Agility 0 (never blocks), Protection 0 and Health Points 5 goes West and attacks Grubnik until he defeats it. Loading again shows Health Points 5, with Gold, Protection and Weapon unchanged.
    - **Wounded victory doesn't overwrite Health Points:** a saved Profile with Strength 20 (always hits), Agility 0 and very high Health Points goes West and defeats Grubnik. Loading again shows the original Health Points.
    - These combat tests need no dice control. Combat keeps fighting when the scripted input runs out, so they can't stall on input. The timeout still guards them.
- [ ] The full test suite passes, including every existing Merchant, combat, Dragon and Goblin test.
