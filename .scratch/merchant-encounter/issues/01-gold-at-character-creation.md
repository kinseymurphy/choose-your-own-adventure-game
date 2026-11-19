# 01: Gold at character creation

**What to build:** During character creation, right after rolling Health Points, the player is prompted to "type 'roll'" for Gold and receives 1d20 Gold (no Race modifier). Gold appears on the character stats screen and on the combat stats screen, in English, Spanish, French and Italian. Gold is not spent anywhere yet. See the Gold sections of `.scratch/merchant-encounter/spec.md` and the **Gold** entry in `GLOSSARY.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [x] Player has a Gold property that defaults to 0
- [x] Character creation rolls 1d20 Gold after Health Points using the existing roll prompt pattern
- [x] Gold shows on the player stats display and on the combat stats display
- [x] New keys (Gold roll prompt, Gold stat label) exist in all four languages, with no accented characters
- [x] Unit test: a new Player has 0 Gold
- [x] New keys are added to the required-key tests for all four languages
- [x] All existing tests still pass
- [x] Manual check: create a character, roll Gold, see it on the stats screen

## Comments

Implemented: `Player.Gold` (default 0), `RollGold` (1d20, no Race modifier) right after Health Points, Gold line on the character stats and combat stats screens, `roll_gold` / `stats_gold` keys in English, Spanish, French and Italian. Automated suite: 102/102 passing.

Manual acceptance test (English): the Gold prompt appeared immediately after Health Points. Invalid inputs `x`, `j` and `i` were each rejected with "Please type 'roll' to continue." and the Gold prompt repeated. The character rolled 20 Gold, shown correctly on both the character stats and combat stats screens. The dragon's stats show no Gold.

Review note for ticket 02: player stat lines are built in both `Player.DisplayStats` and `Combat.GetCombatStatsDisplay`, and `requiredKeys` is repeated four times in `MessagesTests`. Adding Protection and new keys will touch both; consider consolidating there.
