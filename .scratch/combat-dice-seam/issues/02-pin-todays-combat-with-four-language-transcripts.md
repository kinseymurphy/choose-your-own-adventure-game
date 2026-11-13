# 02: Pin today's combat with four-language fight transcripts

**What to build:** Characterization tests that capture whole fights exactly as they play today, before any combat logic moves. Each test drives `Combat.StartCombat()` with the scripted die from ticket 01 and scripted console input, and compares the complete printed output with a literal expected transcript. The transcripts cover both Monsters and all four languages, and they include today's inconsistencies. These tests must pass unchanged at the end of every later ticket. See "Characterization first" in `.scratch/combat-dice-seam/spec.md`.

**Blocked by:** 01 (Script the dice through a dice seam)

**Status:** ready-for-agent

- [ ] Transcript tests run `Combat.StartCombat()` with a scripted die and redirected console input and output, restored in cleanup, under `[DoNotParallelize]`.
- [ ] Each expected transcript is a literal string. It's produced from today's code, checked by hand once against the rules in the spec's combat table, and then never regenerated.
- [ ] Together, the transcripts cover at least:
  - **A Player victory over the dragon in English.** It includes a miss, a block with a taunt, a hit with a damage reply, and a final hit that leaves the dragon's Health Points below 0.
  - **A Player defeat by Grubnik in English.** It includes a Monster miss, a block by the player, Protection 3 absorbing part of one hit and all of another, and Health Points stopping at 0 with the defeated narrative.
  - **A mid-fight retreat.** The first round happens before the prompt, then the player types `r`. It shows `retreat_combat` and no ending.
  - **A Player with a Magical Weapon.** The damage roll uses the higher Max Damage as its sides.
  - **The same victory and defeat scripts in Spanish, French and Italian.**
- [ ] Every transcript test fails if any line, its order or its wording changes.
- [ ] No production code changes in this ticket.
- [ ] The full test suite passes.
