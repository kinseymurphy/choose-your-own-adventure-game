# ADR 0001: Add Italian as a supported language

- **Status:** Accepted
- **Date:** 2026-11-05
- **Deciders:** Kinsey Murphy
- **Branch / commit:** `feature/italian-language` / `0d7e233`

## Context

The game supports English, Spanish and French. All player-facing text lives in
`language_data.json`, one top-level section per language. Each section has the same
structure:

- `dictionary`: every game message, looked up by key (for example `"welcome"`)
- `raceMap` / `occupationMap`: turn what the player types into the internal English names
- `displayRaceMap` / `displayOccupationMap` / `displayWeaponMap`: turn internal names back
  into the player's language for display

`Messages.cs` loads whichever section matches `CurrentLanguage`, so it needs no changes
to support another language. The only hard-coded part is the language selection menu
in `Game.cs`, which lists the options and maps the player's number to a section name.

Italian was added to fulfill the requirements of the CIDM 4390 AI Agentic Coding
Part 2 assignment.

## Decision

Add Italian as a fourth language, following the existing pattern:

1. Add an `"Italian"` section to `language_data.json` with the same keys as English in
   every sub-map.
2. Add `4. Italiano` to the language menu in `Game.cs` and map it to `"Italian"`.
3. Add Italian tests that mirror the French tests, plus tests for Italian race and
   occupation input and display names.

The Italian text follows the file's existing conventions:

- No accented characters (for example "piu", "e stato"), so the text displays cleanly
  in the Windows console.
- The command letters (`a`, `r`, `n`, `s`, `e`, `roll`) are the same as in every other
  language. They still fit the Italian words (attaccare, ritirarsi, Nord, Sud, esci).

## Options considered

| Option | Result |
|---|---|
| **Add a JSON section and a menu entry (chosen)** | Matches how Spanish and French were added. Changes 3 lines of code. |
| Build the language menu from the JSON sections | Future languages would need no `Game.cs` change. Rejected for now because it changes more existing code than this feature needs. It can be its own decision later. |
| Translate English automatically while the game runs | Rejected. Needs a network connection or service, and the translations would be inconsistent and impossible to test. |

## Consequences

**Positive**

- Italian speakers can play the whole game in Italian, from the menus to the endings.
- No changes to `Messages.cs`, `Player.cs`, `Combat.cs` or `Dragon.cs`. Game logic still
  uses the internal English names.
- The new tests catch missing Italian keys or broken race and occupation mappings.

**Negative / risks**

- The translations haven't been reviewed by a native Italian speaker. Some wording may
  sound unnatural.
- Leaving out accents is technically incorrect Italian. It's a trade-off for console
  display, shared with the Spanish and French text.
- Every new language still needs a `Game.cs` menu change, and the menu line gets longer
  each time.
- Nothing automatically checks that every language has every key. The Italian tests
  only check a subset of keys, as the tests for the other languages do.

## Verification

- `dotnet test CYOATests`: 102 passed, 0 failed (includes 8 new Italian tests).
- Ran the game manually, chose `4`, and confirmed the menu and exit message appear in
  Italian.
