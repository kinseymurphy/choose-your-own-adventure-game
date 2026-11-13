# 01: Gold at character creation

**What to build:** During character creation, right after rolling Health Points, the player is prompted to "type 'roll'" for Gold and receives 1d20 Gold (no Race modifier). Gold appears on the character stats screen and on the combat stats screen, in English, Spanish, French and Italian. Gold is not spent anywhere yet. See the Gold sections of `.scratch/merchant-encounter/spec.md` and the **Gold** entry in `GLOSSARY.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] Player has a Gold property that defaults to 0
- [ ] Character creation rolls 1d20 Gold after Health Points using the existing roll prompt pattern
- [ ] Gold shows on the player stats display and on the combat stats display
- [ ] New keys (Gold roll prompt, Gold stat label) exist in all four languages, with no accented characters
- [ ] Unit test: a new Player has 0 Gold
- [ ] New keys are added to the required-key tests for all four languages
- [ ] All existing tests still pass
- [ ] Manual check: create a character, roll Gold, see it on the stats screen
