Prepare and publish a release for the Mil.Paperwork.WriteOff project. Optionally, a version number can be passed as: $ARGUMENTS

Follow these steps in order:

## Step 1 — Verify working tree
Run `git status` to check for uncommitted changes. If any exist, stop and tell the user to commit or stash them before releasing.

## Step 2 — Run tests
Run `dotnet test Mil.Paperwork.Tests\Mil.Paperwork.Tests.csproj` and report results. If any tests fail, stop and show the failures — do not proceed to tagging.

## Step 3 — Build release
Run `dotnet build Mil.Paperwork.WriteOff.sln --configuration Release` and confirm it succeeds. If the build fails, stop and show errors.

## Step 4 — Determine version
If a version was passed as an argument, use it. Otherwise, run `git tag --sort=-version:refname | head -5` to show recent tags and ask the user: "What version should this release be tagged as? (e.g. 1.2.3.4)"

Version must follow the `MAJOR.MINOR.PATCH.BUILD` four-part format used by this project.

## Step 5 — Confirm before tagging
Show the user exactly what will happen:
```
Tag:    {version}
Branch: {current branch}
Commit: {HEAD short hash} {HEAD commit message}
```
Ask: "Proceed with tagging and pushing?"

Do NOT run `git tag` or `git push` until the user confirms.

## Step 6 — Tag and push
After confirmation:
1. `git tag {version}`
2. `git push origin --tags`

Report the tag that was pushed. Remind the user that CI/CD will now build the GitHub Release with Windows MSIX and macOS app bundles.
