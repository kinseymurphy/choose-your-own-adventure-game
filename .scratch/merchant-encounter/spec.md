# Spec: Merchant Encounter

Status: ready-for-agent

## Problem Statement

The South path is a dead end. A player who takes it reads "nothing adventurous happens… you return home, poor, and still bored", and then the game sends them back to the path choice anyway, so the text contradicts what happens. The only real content is the dragon on the North path, and the player faces it with whatever their dice gave them at character creation. Nothing the player chooses can change their odds.

## Solution

Taking the South path starts a **Merchant Encounter**. An unnamed traveling **Merchant** offers two items for **Gold**, the player's new currency, which is rolled during character creation:

- **Magical Armor** gives the player **Protection**. Every dragon hit that gets past the player's defense does less damage.
- **Magical Weapon** upgrades the player's own occupation weapon so it deals more damage.

After leaving the Merchant, the player returns to the path choice and can go North to fight the dragon with what they bought. The whole encounter is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want to roll for Gold during character creation, so that I have something to spend on my adventure.
2. As a player, I want the Gold roll to use the same "type 'roll'" prompt as my other stats, so that character creation feels consistent.
3. As a player, I want my Gold to show on my character stats, so that I know what I can afford.
4. As a player, I want my Protection to show on my character stats, so that I can see whether I'm wearing Magical Armor.
5. As a player, I want the South path to lead me to a Merchant, so that choosing South is a real choice instead of a dead end.
6. As a player, I want the South narrative to introduce the traveling Merchant, so that the story matches what happens next.
7. As a player, I want the Merchant to show me how much Gold I have, so that I can decide what to buy.
8. As a player, I want the Merchant to list Magical Armor and Magical Weapon with their prices, so that I know my options.
9. As a player, I want to buy Magical Armor for 8 Gold, so that I take less damage from the dragon.
10. As a player, I want to buy a Magical Weapon for 8 Gold, so that I deal more damage to the dragon.
11. As a player, I want to buy both items in a single visit if I have enough Gold, so that I don't have to walk South twice.
12. As a player, I want the menu to stay open after a purchase, so that I can keep shopping or choose to leave.
13. As a player, I want to choose Leave to end the encounter, so that I can go back to the path choice when I'm done.
14. As a player, I want to be told when I don't have enough Gold, so that I understand why my purchase failed.
15. As a player, I want my Gold to stay the same when a purchase fails, so that I'm never charged for something I didn't get.
16. As a player, I want to be told when I already own an item, so that I don't waste Gold buying it twice.
17. As a player, I want invalid menu input to show the usual "invalid choice" message and ask again, so that a typo doesn't kick me out of the Merchant.
18. As a player with too little Gold for anything, I want the Merchant menu to still appear, so that the encounter still happens and I can leave on my own.
19. As a player, I want to meet the Merchant every time I go South, so that I can come back if I change my mind.
20. As a player who already bought everything, I want the Merchant to refuse repeat purchases, so that going South again can't break the game.
21. As a player, I want to return to the path choice after leaving the Merchant, so that I can head North to fight the dragon.
22. As a Fighter, I want my long sword to become a magical long sword with 4 more Max Damage, so that my upgrade fits my occupation.
23. As a Magician, I want my lightning bolt spell to become a magical lightning bolt spell, so that I'm not handed a sword.
24. As a Thief, I want my dagger to become a magical dagger, so that my upgrade fits my occupation.
25. As an Archer, I want my long bow to become a magical long bow, so that my upgrade fits my occupation.
26. As a player, I want my Magical Weapon to keep its ASCII art, so that it still looks like my weapon in combat.
27. As a player, I want my weapon's magical name and new Max Damage to show on the stats and combat screens, so that I can see the upgrade worked.
28. As a player wearing Magical Armor, I want each dragon hit that gets past my defense to be reduced by my Protection, so that the armor matters in combat.
29. As a player wearing Magical Armor, I want combat to tell me how much damage my armor absorbed, so that I can see it working.
30. As a player wearing Magical Armor, I want a weak hit to do no damage at all instead of healing me, so that the rules feel fair.
31. As a player without Magical Armor, I want combat to work exactly as before, so that the game doesn't change if I skip the Merchant.
32. As a player whose defense roll blocks a hit, I want the hit fully blocked as before, so that Protection only matters for hits that get through.
33. As a Spanish, French or Italian speaker, I want the whole Merchant Encounter in my language, so that I can play the whole game without English.
34. As a Spanish, French or Italian speaker, I want magical weapon names with correct grammatical agreement, so that the text reads naturally.
35. As a developer, I want purchase rules that don't depend on the console, so that I can unit test them without simulating input.
36. As a developer, I want the damage reduction to be its own calculation, so that I can test Protection without dice.
37. As a developer, I want tests that fail when any language is missing a new key, so that untranslated `[key]` placeholders never reach players.

## Implementation Decisions

**Player**
- Add a **Gold** property. It defaults to 0 and is rolled as 1d20 during character creation, right after Health Points, using the existing roll prompt. Race doesn't modify it.
- Add a **Protection** property. It defaults to 0, and 0 means the player has no Magical Armor.
- Add `ReduceDamage(int damage)`, which returns `max(0, damage − Protection)`.
- The stats display adds Gold and Protection lines.
- The way `Player` creates its own Die doesn't change.

**Weapon**
- Add an `IsMagical` flag. It defaults to false.

**Merchant (new class)**
- Holds the prices (8 Gold for each item), the Protection value from armor (3) and the weapon damage bonus (+4).
- `BuyMagicalArmor(Player)` and `BuyMagicalWeapon(Player)` return a purchase result: **Purchased**, **NotEnoughGold** or **AlreadyOwned**. They never write to the console.
  - **Armor:** returns AlreadyOwned if Protection > 0. Returns NotEnoughGold if Gold < 8. Otherwise it subtracts 8 Gold, sets Protection to 3 and returns Purchased.
  - **Weapon:** if the player has no weapon, the purchase is refused and no Gold changes. The menu shows the existing `invalid` message for this case. Returns AlreadyOwned if the weapon `IsMagical`. Returns NotEnoughGold if Gold < 8. Otherwise it subtracts 8 Gold, sets `IsMagical`, changes `Type` from `"<type>"` to `"magical <type>"`, adds 4 to MaxDamage, leaves the ASCII art alone and returns Purchased.
  - The checks run in this order: no weapon, then already owned, then not enough Gold.
- `StartEncounter(Player, Messages)` runs the menu loop:
  - It shows the player's Gold and the options `1. Magical Armor (8 Gold)`, `2. Magical Weapon (8 Gold)` and `3. Leave`.
  - It accepts number input only.
  - It prints a message for each purchase result and keeps looping until the player chooses 3.

**Dragon**
- `Dragon.Attack` sends the damage roll through `player.ReduceDamage` before subtracting Health Points, and keeps clamping Health Points at 0.
- When the player's Protection > 0, it adds a line saying how much the armor absorbed. The damage message shows the reduced amount.

**Combat**
- The combat stats screen adds the player's Gold and Protection.

**Game**
- The South branch prints the rewritten South narrative and calls `Merchant.StartEncounter`, then goes back to the path choice.
- The North and Exit branches don't change.

**Localization (`language_data.json`)**
- **English, Spanish, French and Italian** all get every new key. The text has no accented characters, following ADR 0001's convention.
- **New dictionary keys:**
  - Gold roll prompt
  - Gold and Protection stat labels
  - Merchant greeting with Gold shown
  - Merchant menu
  - Purchase success, one per item
  - Not enough Gold
  - Already owned
  - Leave or farewell
  - Armor absorbs damage
- **Rewritten:** `south_path_narrative` now introduces an unnamed traveling Merchant.
- **`displayWeaponMap`:** gets four new entries per language: magical long sword, magical lightning bolt spell, magical dagger, magical long bow. They're full translated names, not a template, because the adjective has to agree with the noun.

## Testing Decisions

- **Test external behavior only.** Call public methods with a `Player` built for the test, then check returned results and public state (Gold, Protection, Weapon type, MaxDamage, IsMagical). Don't test private methods or console output.
- **Merchant purchase tests:**
  - Each item bought with exactly 8 Gold.
  - Each item bought with more than 8 Gold, leaving the rest.
  - Each item refused with less than 8 Gold, with nothing changed.
  - A second armor purchase is refused.
  - A second weapon upgrade is refused.
  - Buying both items in sequence.
  - All four occupation weapons upgrade to the correct magical type and +4 damage.
  - The ASCII art is kept.
  - A weapon purchase with no weapon is refused and no Gold is taken.
  - Already owned is checked before not enough Gold.
- **`Player.ReduceDamage` tests:**
  - With Protection 0, damage is unchanged.
  - Damage greater than Protection is reduced.
  - Damage equal to Protection gives 0.
  - Damage less than Protection gives 0, never a negative number.
- **New `Player` defaults:** Gold and Protection start at 0, alongside the existing default-values test.
- **New `Weapon` default:** `IsMagical` starts false.
- **Messages tests:**
  - Add the new keys to the existing required-key lists for English, Spanish, French and Italian.
  - Add tests that each magical weapon type translates to a non-fallback name in every language.
- **Not unit tested:**
  - The `StartEncounter` menu loop, `Game`'s South branch and the Gold roll. These depend on console input and on `Player`'s own Die, and existing prompt code isn't unit tested either.
  - These are covered by one manual playthrough: create a character, go South, buy items, check stats, go North and fight.
- **Prior art:**
  - MSTest classes in the test project: `PlayerTests`, `WeaponTests`, `DragonTests`.
  - The per-language required-key tests in `MessagesTests`.
  - The Italian tests added under ADR 0001.
  - Tests build `Player` and `Weapon` with object initializers.

## Out of Scope

- An inventory system, item lists, selling, or more than one of each item.
- Any way to earn Gold besides the creation roll. The dragon's hoard isn't added to Gold.
- Armor, Protection or upgrades for the dragon.
- Changing how `Player` creates its Die so tests can control it.
- A named Merchant, Merchant stats, or combat with the Merchant.
- Letter shortcuts for the Merchant menu.
- Fixing existing combat quirks unrelated to this feature, such as the mid-combat retreat skipping the ending, or the first round running before the player is prompted.
- Building the language menu from the JSON (deferred in ADR 0001).

## Further Notes

- Work happens on the `feature/merchant-encounter` branch.
- No new ADR: every decision here is easy to undo and wouldn't surprise a future reader.
- Domain terms (Gold, Merchant, Merchant Encounter, Magical Armor, Protection, Magical Weapon) are defined in `GLOSSARY.md`.
- With a d20 Gold roll and 8-Gold prices, about 35% of characters can afford nothing, 40% can afford one item and 25% can afford both.
