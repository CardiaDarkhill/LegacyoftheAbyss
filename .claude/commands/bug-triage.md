---
description: List, read, and close out in-game bug reports filed with the F8 hotkey.
argument-hint: "[list | latest | <report-id> | fixed <report-id>]"
---

# Bug report triage

Reports are filed from inside the game (default hotkey **F8**, see `Diagnostics/BugReportSystem.cs`)
and written outside this repository, so they survive a mod reinstall and never land in a commit.

## Locating the reports folder

Resolve `$REPORTS` in this order and use the first that exists:

1. `$LEGACYOFTHEABYSS_DATA/bug_reports` — only if that environment variable is set.
2. `../../config/LegacyoftheAbyss/bug_reports` — relative to this repository's root. This is the
   normal location: the repo is checked out inside `BepInEx/plugins/`, so this resolves to
   `BepInEx/config/LegacyoftheAbyss/bug_reports`.

If neither exists, say so and stop — it means nothing has been filed yet.

Each report is a folder named `<yyyyMMdd>-<HHmmss>-<slug>` containing:

| File | What it is |
| --- | --- |
| `report.md` | Human summary: the typed message, scene, Hornet and Shade state, and the tail of the log. **Read this first.** |
| `state.json` | Full snapshot — mod config, every loaded BepInEx plugin, all captured fields. |
| `log.txt` | The whole captured log ring, every BepInEx source, not just this mod. |
| `flight.csv` | Rolling state samples. `t_rel` is seconds relative to the capture, so the last rows are the reported moment and negative rows are the lead-up. |
| `screenshot.png` | The frame as it looked when the hotkey was pressed. Read it — visual bugs are often obvious from it and described badly in words. |

`index.md` in `$REPORTS` lists every report, oldest first. `- [ ]` is open, `- [x]` is fixed.

## What to do

The argument is `$ARGUMENTS`.

**No argument, or `list`** — list the open reports. Read `$REPORTS/index.md`, show the unchecked
entries newest-first with their id, title and scene, and note how many are already closed. Do not
open any report folder yet. End by offering to dig into one.

**`latest`** — pick the newest open entry in `index.md` and triage it as below.

**A report id** (full, or a unique prefix such as `20260822-1732`) — triage that report:

1. Read its `report.md` in full.
2. Read `screenshot.png` if present.
3. Read the tail of `flight.csv` — the last ~40 rows cover the moment reported. Look for what changed
   across them: a position discontinuity, a flag that got stuck set, HP or soul moving when it should
   not have.
4. Scan `log.txt` for warnings and exceptions near the capture. The `t=` column in each line matches
   `flight.csv`'s `realtime` column, so the two can be lined up directly.
5. Consult `state.json` for anything the summary omits — particularly the equipped charms and the
   other loaded plugins, both of which routinely turn out to be the actual variable.
6. Then find the cause in the source and explain it, citing files and line numbers. Propose a fix.
   **Do not apply it unless asked** — triage and fixing are separate decisions.

**`fixed <report-id>`** — close a report out, only after the fix is actually in the working tree:

1. In `$REPORTS/index.md`, change that report's `- [ ]` to `- [x]`.
2. In its `report.md`, change the `- **Status:** open` line to
   `- **Status:** fixed <yyyy-MM-dd> - <one-line summary of the fix and the files touched>`.
3. Leave the folder in place. A closed report is the record of why a change was made.

## Notes

- Reports with `**Trigger:** auto-exception` were filed automatically by the game, with no human
  message. Their `## Exception` block is the whole story; go straight to the stack trace.
- Several reports with the same slug and similar timestamps are usually one bug hit repeatedly.
  Diagnose them together rather than one at a time.
- Never edit anything under `$REPORTS` except the status line and the index checkbox.
