Add or update a Ukrainian-language documentation section in the root README.md for the feature on the current branch. Optionally, a feature name or description hint can be passed as: $ARGUMENTS

## Step 1 — Understand the feature
Run `git log master..HEAD --oneline` to list commits on this branch. Read the relevant stage docs in `docs/` (if any) and scan the changed files to understand what was built.

## Step 2 — Read existing README
Read `README.md` to understand its current structure so the new section fits naturally.

## Step 3 — Draft the section
Write a concise Ukrainian-language section covering:
- What the feature does (user-facing description, not implementation details)
- How to use it (key steps or UI actions the user takes)
- Any important notes or limitations

Write in clear Ukrainian, consistent with any existing Ukrainian text in the README. Do not describe code internals — focus on what the user sees and does.

## Step 4 — Confirm before writing
Show the user the drafted section and ask: "Does this look right? I'll append it to README.md after you confirm."

## Step 5 — Apply
After confirmation, append the section to `README.md` in the appropriate location. If the README already has a section for this feature, update it in place instead of appending.
