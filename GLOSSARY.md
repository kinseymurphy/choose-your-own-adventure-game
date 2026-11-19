# Choose Your Own Adventure

A text adventure in which the player creates a character, chooses a path through the mountains, and may face a dragon.

## Language

### Character

**Gold**:
The player's currency, rolled once during character creation and spent at the Merchant.
_Avoid_: Money, coins, currency

**Profile**:
A saved character the player can pick again at startup. A Profile holds exactly one character: stats, Gold, Protection and Weapon. It is known by its character's name, which no other Profile shares, ignoring case. Ending a Dead End doesn't remove the Profile.
_Avoid_: Account, save slot, user

**Session**:
One play-through with a character, from choosing or creating a Profile until a Dead End or Exit. Every Session starts at the Health Points the character rolled when it was created; damage taken lasts only for that Session.
_Avoid_: Game, run, playthrough

### Paths and encounters

**Merchant Encounter**:
What happens when the player takes the South path: they meet the Merchant, may buy items, and then return to the path choice.
_Avoid_: Shop, store

**Merchant**:
The trader met on the South path who sells Magical Armor and Magical Weapons for Gold.
_Avoid_: Shopkeeper, vendor, trader

**Dead End**:
An encounter after which the Session ends, whatever the outcome: victory, defeat or retreat. The Dragon Encounter and the Goblin Encounter are Dead Ends; the Merchant Encounter is not.
_Avoid_: Final encounter, boss fight

**Dragon Encounter**:
What happens when the player takes the North path: they meet the dragon Smolderfang and may fight it. A Dead End.

**Goblin Encounter**:
What happens when the player takes the West path into the Murky Swamp: they meet the goblin Grubnik and may fight him. A Dead End.

**Murky Swamp**:
The place the West path leads to, where the Goblin Encounter happens.
_Avoid_: Bog, marsh

### Combat

**Monster**:
Any creature the player can fight. The dragon Smolderfang and the goblin Grubnik are Monsters; the Merchant is not.
_Avoid_: Enemy, foe, opponent, creature

### Equipment

**Magical Armor**:
Armor bought from the Merchant that gives the player Protection.
_Avoid_: Shield, gear

**Protection**:
A fixed amount subtracted from the damage of each hit that gets past the player's defense.
_Avoid_: Defense, armor class (defense is the Agility roll that blocks a hit entirely)

**Magical Weapon**:
The player's own occupation weapon after the Merchant upgrades it to deal more damage.
_Avoid_: Enchanted weapon, new weapon
