# 03: Protection reduces dragon damage

**What to build:** When the dragon's hit gets past the player's Agility defense, a player wearing Magical Armor takes `max(0, damage − Protection)` damage instead of the full roll. Combat shows the reduced damage and a line saying how much the armor absorbed, in all four languages. Players without armor see combat exactly as before. Hits blocked by the defense roll stay fully blocked. See the Protection section of `.scratch/merchant-encounter/spec.md`.

**Blocked by:** 02 (Merchant Encounter with Magical Armor)

**Status:** ready-for-agent

- [ ] `Player.ReduceDamage(int damage)` returns `max(0, damage − Protection)`
- [ ] Dragon.Attack runs the damage roll through `ReduceDamage` before subtracting Health Points; Health Points is still clamped at 0
- [ ] When Protection > 0, a line saying how much the armor absorbed is added; the damage message shows the reduced amount
- [ ] When Protection is 0, combat output and behavior are unchanged
- [ ] The new key for the armor-absorbs message exists in all four languages with no accented characters
- [ ] Unit tests for ReduceDamage: Protection 0 (unchanged); damage > Protection (reduced); damage == Protection (0); damage < Protection (0, never negative)
- [ ] New key is added to the required-key tests for all four languages
- [ ] All existing Dragon and Combat tests still pass
- [ ] Manual check: buy armor, go North, attack, see the armor-absorbs line when a hit gets through
