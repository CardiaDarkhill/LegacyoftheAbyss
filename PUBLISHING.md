# Publishing — Nexus Mods & Thunderstore

One-time setup below, then every release is: **GitHub → Actions tab → "Publish Release" →
Run workflow → type the version number → Run.** Nothing publishes on a normal `git push`.

## What the workflow does

`.github/workflows/publish.yml` runs on a GitHub-hosted Windows runner (full internet access, so
the BepInEx NuGet feed is reachable there):

1. Checks out the repo, installs the .NET 7 SDK.
2. `dotnet build LegacyoftheAbyss.csproj -c Release -p:Version=<version> -p:CreateDistributionPackages=true`.
   Both extra flags matter:
   - the project is named explicitly so the runner doesn't also build the xunit test project,
     which needs a real game install to be useful;
   - `CreateDistributionPackages` is what enables the `PrepareReleasePackages` MSBuild target.
     It is opt-in (a plain `dotnet build -c Release` deliberately stages nothing, so that
     Release builds don't drop loadable duplicate DLLs inside the live `BepInEx/plugins` tree).
     Without it, every packaging step downstream silently has nothing to package.
   This produces `obj/Release/Nexus/LegacyoftheAbyss-<version>/` and
   `obj/Release/Thunderstore/LegacyoftheAbyss/`.
3. Verifies both staging folders actually contain `LegacyoftheAbyss.dll` before going further.
4. Zips the Nexus folder and uploads it to the existing Nexus mod page as a new file version via
   `Nexus-Mods/upload-action`.
5. Runs `tcli build --package-version <version>`, **verifies the resulting zip** contains the DLL,
   manifest, icon, README and asset entries, then `tcli publish --file <that zip>`.
6. Uploads both staging folders and the zip as workflow artifacts (30-day retention) so there's
   always a manual fallback if a platform push fails.

Each platform can be toggled off per-run with the `publish_nexus` / `publish_thunderstore`
checkboxes on the Run-workflow form.

### Why the verification steps exist

`tcli` treats a `[[build.copy]]` source path that matches nothing as a **no-op, not an error** —
it prints `Successfully built …` and exits 0 having produced a zip containing only the icon,
README and manifest. A Thunderstore version number can never be reused or deleted, only
superseded, so the zip is checked before upload rather than after.

Two related traps are now designed out rather than documented around:

- **`[[build.copy]] source` must have no trailing slash.** With one, tcli drops the first
  character of every path it writes (`plugins/Assets/…` → `lugins/Assets/…`,
  `LegacyoftheAbyss.dll` → `egacyoftheAbyss.dll`). The build log looks completely normal.
- **The Thunderstore staging folder is deliberately not version-stamped**
  (`obj/Release/Thunderstore/LegacyoftheAbyss`, no `-<version>` suffix, unlike the Nexus one).
  `thunderstore.toml` has to name that folder literally, so a versioned path would need the toml
  rewritten every release — and getting that wrong lands you in the silent-empty-package case
  above. The version reaches the package through tcli's `--package-version` flag, which is also
  what writes `manifest.json`, so the committed toml is always valid as-is and a local
  `tcli build` behaves identically to CI.

## One-time setup — Thunderstore

1. On thunderstore.io: **Settings → Teams → [your team] → Service Accounts** → create a token
   (starts with `tss_`).
2. In the GitHub repo: **Settings → Secrets and variables → Actions** → add secret
   `TCLI_AUTH_TOKEN` with that value.

`thunderstore.toml` is already committed and needs no per-release edits.

## One-time setup — Nexus Mods

1. On nexusmods.com: **Settings → API Keys** (<https://www.nexusmods.com/settings/api-keys>) →
   generate a personal API key.
2. Find the existing mod's **mod ID** and **file ID**.
   - Mod ID is the number in the mod page URL: `nexusmods.com/hollowknightsilksong/mods/166`.
   - File ID is the number the Files tab's **"API Info"** dialog shows — where Nexus's UI labels
     it **"Group ID"** (currently `812617`). The naming mismatch is confusing but the value is
     right: the action's input identifies *a file lineage that receives a new version*, not one
     individual upload, which is why every version of the mod reports the same number. Seeing it
     repeat across 1.0.5/1.0.6 confirms you have the right ID rather than the wrong one.
   - Do **not** try to verify it via `?tab=files&file_id=<id>` on the mod page. That URL parameter
     is a different identifier and returns "Object not found" for a valid Group ID.
3. In the GitHub repo (**Settings → Secrets and variables → Actions**):
   - secret `NEXUSMODS_API_KEY` — the key from step 1. (This name matches the action's own
     README example; an earlier revision of the workflow read `NEXUS_API_KEY`, which would have
     silently resolved to an empty string and failed authentication.)
   - variable `NEXUS_MOD_ID` — numeric mod ID.
   - variable `NEXUS_FILE_ID` — numeric file ID.

The action is pinned to the exact tag `v1.0.0-beta.10`. There is **no stable `v1` ref** — every
release is `v1.0.0-beta.N` — so `@v1` fails to resolve and the step never runs. Nexus's Upload API
is itself an open beta, so check <https://github.com/Nexus-Mods/upload-action/tags> before bumping,
and re-read that tag's `action.yml` rather than `main`'s.

The workflow passes `api_key`, `mod_id`, `file_id`, `filename`, `version`, `display_name` and
`update_mod_version`, all of which are inputs that tag actually defines. `mod_id` is optional
per the action's own contract — it is only *required* when `changelog` is set, and the upload
target comes from `file_id` alone — but it is set here so that adding a changelog later needs no
extra setup. `update_mod_version: true` is deliberate: it defaults to false, which uploads the
new file while leaving the mod page still advertising the previous version number. Note it has **no** `game_domain` input —
Actions only *warns* about unrecognised inputs rather than failing, so passing one looks harmless
while doing nothing. Other optional inputs available if wanted later: `description`, `changelog`,
`category` (defaults to `main`), `archive_existing_version`, `update_mod_version`,
`primary_mod_manager_download`, `allow_mod_manager_download`, `show_requirements_pop_up`.

## Package layout, and why the two platforms differ

| | Nexus package | Thunderstore package |
| --- | --- | --- |
| DLL | `BepInEx/plugins/LegacyoftheAbyss.dll` | `LegacyoftheAbyss.dll` (package root) |
| Assets | `BepInEx/plugins/Assets/` | `plugins/Assets/` |

The Nexus layout is a portable drop-in over a BepInEx install. The Thunderstore layout relies on
mod managers' handling of a top-level `plugins/` folder, which lands its contents next to the DLL
inside the installed package folder — which is what `ModPaths.Assets` (resolved relative to the
DLL) expects.

**Consequence:** the Thunderstore zip is only correct when installed through a mod manager
(Thunderstore Mod Manager, r2modman, Gale). Extracted by hand into `BepInEx/plugins/`, the assets
land at `plugins/Assets/` relative to the DLL and none of the search roots in
`ModPaths.GetAssetSearchRoots()` find them — sprites and audio silently fail to load. Manual
installers should be pointed at the Nexus archive.

## Dependency pin

`thunderstore.toml`'s `[package.dependencies]` pins
`silksong_modding-BepInExPack_Silksong-1.0.3`, and is the only place that pin lives — tcli
generates `manifest.json` from it. That listing's version drifts over time; check
<https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/BepInExPack_Silksong/>
if the dependency ever fails to resolve for someone. (The older
`BepInEx-BepInExPack_Silksong` listing is deprecated and just redirects — don't go back to it.)

## Manual fallback

The manual path still works, with one change: build locally with
`dotnet build LegacyoftheAbyss.csproj -c Release -p:Version=x.y.z -p:CreateDistributionPackages=true`.

- **Nexus** — `obj/Release/Nexus/LegacyoftheAbyss-x.y.z/` is a complete package; zip it and upload.
- **Thunderstore** — `obj/Release/Thunderstore/LegacyoftheAbyss/` holds the *payload only* (DLL,
  deps, `plugins/Assets/`). It is deliberately not a standalone package: `manifest.json`,
  `README.md` and `icon.png` come from tcli. Run `tcli build --package-version x.y.z` and upload
  the zip it writes to `./build/` (gitignored). That is also the exact zip CI publishes, so it is
  the cheapest way to eyeball a release before shipping it.

### tcli exit codes

`tcli build` **exits 1 on warnings**, including cosmetic-looking ones, while still writing a
perfectly valid zip. Staging `manifest.json`/`README.md`/`icon.png` into the Thunderstore folder
alongside tcli's own copies produced three `was added multiple times` warnings and failed the
workflow at the build step for that reason alone. Don't reintroduce a second source for any file
tcli already places, and don't paper over a non-zero exit — the whole point of that exit code is
that it is the signal something is wrong with the package.
