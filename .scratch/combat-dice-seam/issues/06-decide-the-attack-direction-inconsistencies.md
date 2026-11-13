# 06: Decide the inconsistencies between the two attack directions

**What to build:** Nothing yet. This ticket records the differences between the player's attack and the Monster's attack, which tickets 01–05 deliberately preserve. Each one needs a decision: keep it, or change it in its own small ticket. Once ticket 04 is done, every one of them lives in `Combat`, so each change can be made in one place. Any change will alter what players see, and some will alter the ticket 02 transcripts on purpose.

**Blocked by:** 04 (Combat resolves the player's attack with the same sequence)

**Status:** needs-triage

## The inconsistencies

1. **Monster Health Points can go below 0.**
   - After the player's final hit, `monster_health_now` can show a negative value, for example "Smolderfang's health points are now -7." The player's Health Points stop at 0.
   - Who wins is unaffected, because victory is checked with ≤ 0.
   - **Possible change:** stop them at 0. The `monster_health_now` line and the transcripts would change.
2. **ASCII art is printed differently.**
   - The player's Weapon art is always printed.
   - A Monster's art is printed only when it isn't blank.
   - Every Weapon has art today, so players don't see a difference.
   - **Possible change:** use one rule for both. No visible change today.
3. **"No weapon" text is hard-coded in English.**
   - A player with no Weapon gets "You have no weapon to attack with." in every language.
   - It can't happen in play: every Occupation gives a Weapon, and ADR 0003's load check requires one.
   - **Possible change:** remove the branch, or add a translated message key in all four languages.
4. **Protection only works one way.**
   - Monsters have no Protection, so the player's hits are never reduced.
   - This matches the game's design (Protection comes from the Merchant's Magical Armor), so it's listed for completeness.
   - **Likely decision:** keep.

## Related, not part of combat

- Character creation can produce Strength above 20 (d20 plus a d4 modifier). Such a character always hits.
- A Halfling can start with 0 Health Points or less. This is already a Known Limitation in `.scratch/player-profiles/spec.md`.

## Comments
