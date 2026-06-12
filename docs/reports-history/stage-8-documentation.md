# Stage 8 — Documentation: README section for the History feature

**Goal:** user-facing documentation. No code changes. A new section in the repo-root `README.md` describing the Reports History feature, written in Ukrainian.

Part of the Reports History feature — see [README.md](README.md). Requires Stages 1–7 (documentation must describe the actually shipped behavior, so write it against the final implementation, not the plan).

## What to write

Add a new section to the root `README.md` (not this folder's README). Match the existing README's informal, first-person Ukrainian tone. The section covers:

1. **Що це і навіщо** — every generated report's input data is saved locally as a history entry; drafts can be saved manually; the History tab lists past reports so nothing typed into the app is ever lost with the closed tab.
2. **Як користуватися**:
   - auto-save: generating a report creates a `Generated` entry with paths to the output files;
   - the Save-draft button on report tabs creates/updates a `Draft` entry; re-saving from the same tab updates the same entry, it does not duplicate;
   - the History tab: filtering (type, status, text search by document number / asset names / serial numbers), opening the generated file, removing an entry;
   - reopening an entry into a prefilled editor tab;
   - "Створити…" cross-type conversions (which source → target pairs exist — list the ones actually implemented in Stage 6/7).
3. **Де зберігається** — `Data/History/` next to the exe: one JSON file per entry + `index.json` cache that is rebuilt automatically if missing or corrupt; entries can be backed up / transferred by copying the folder.
4. **Обмеження** — describe honestly, in the spirit of the existing "Чого тут немає?" section:
   - local only: no sync between computers, no multi-user support;
   - history stores input data, not the generated documents themselves — if the output files are deleted, "Відкрити файл" won't work;
   - editing an entry's JSON by hand is possible but unsupported;
   - schema is versioned (`SchemaVersion`), old entries are read best-effort: unreadable files are skipped, not migrated;
   - report types not (yet) covered by reopen/create-from, if any remain after Stage 7.

## Placement and style

- Insert the section after «Як користуватися?» so the README reads: intro → usage → history → limitations sections.
- Ukrainian language, same tone as the rest of the file; `> [!NOTE]`/`> [!IMPORTANT]` callouts where they genuinely help.
- Keep it practical: a user who has never seen the feature should understand it from this section alone, without the stage docs.

## Verify

- Proofread the Ukrainian text; render the markdown (GitHub preview) and check headings, lists and callouts.
- Cross-check every described behavior against the running app — no promised-but-unshipped functionality in the README.
