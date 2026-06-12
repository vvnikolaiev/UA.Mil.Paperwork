Stage and commit the current changes. An optional commit message hint can be passed as: $ARGUMENTS

## Step 1 — Show what will be committed
Run `git status` and `git diff HEAD` in parallel. Display:
- Which files are modified / added / deleted
- A condensed diff summary (changed symbols, not full diff unless small)

## Step 2 — Suggest a commit message
Based on the diff, draft a commit message:
- Format: `type: short imperative description` (e.g. `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`)
- One line, under 72 characters
- Focus on the *why* or the *what changed*, not the implementation details
- If a hint was passed as an argument, use it to guide the message

If the changes span multiple unrelated concerns, say so and suggest splitting into separate commits.

## Step 3 — Ask for confirmation
Show the proposed commit message and ask the user:
"Commit with this message? (edit, split, or confirm)"

Do NOT run `git add` or `git commit` until the user confirms or provides their own message.

## Step 4 — Stage and commit
After confirmation:
1. Stage files using specific file names (never `git add -A` or `git add .` unless the user explicitly asks)
2. Commit with the agreed message, appended with:
   ```
   Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>
   ```
3. Run `git status` and show the result so the user can confirm the tree is clean.
