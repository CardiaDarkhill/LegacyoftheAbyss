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
  above. The version still reaches the package through `manifest.json` (written by MSBuild from
  `BuildTemplates/Thunderstore/manifest.template.json`) and through tcli's `--package-version`
  flag, so the committed toml is always valid as-is and a local `tcli build` behaves identically
  to CI.

## One-time setup — Thunderstore

1. On thunderstore.io: **Settings → Teams → [your team] → Service Accounts** → create a token
   (starts with `tss_`).
2. In the GitHub repo: **Settings → Secrets and variables → Actions** → add secret
   `TCLI_AUTH_TOKEN` with that value.

`thunderstore.toml` is already committed and needs no per-release edits.

## One-time setup — Nexus Mods

1. On nexusmods.com: **Settings → API Keys** (<https://www.nexusmods.com/settings/api-keys>) →
   generate a personal API key.
2. Find the existing mod's **mod ID** and **file ID** — on the mod's public Files tab use the
   "API Info" option, or check the edit view on the Manage Files page.
3. In the GitHub repo (**Settings → Secrets and variables → Actions**):
   - secret `NEXUSMODS_API_KEY` — the key from step 1. (This name matches the action's own
     README example; an earlier revision of the workflow read `NEXUS_API_KEY`, which would have
     silently resolved to an empty string and failed authentication.)
   - variable `NEXUS_MOD_ID` — numeric mod ID.
   - variable `NEXUS_FILE_ID` — numeric file ID.

The workflow passes `api_key`, `mod_id`, `file_id`, `filename`, `version` and `display_name`,
all of which are inputs the action actually defines. Note it has **no** `game_domain` input —
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

`BuildTemplates/Thunderstore/manifest.template.json` and `thunderstore.toml` both pin
`silksong_modding-BepInExPack_Silksong-1.0.3`. That listing's version drifts over time; check
<https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/BepInExPack_Silksong/>
if the dependency ever fails to resolve for someone. (The older
`BepInEx-BepInExPack_Silksong` listing is deprecated and just redirects — don't go back to it.)

## Manual fallback

The manual path still works exactly as before: build locally with
`dotnet build LegacyoftheAbyss.csproj -c Release -p:Version=x.y.z -p:CreateDistributionPackages=true`,
zip the two `obj/Release/...` folders yourself, and upload through each site's web UI. Running
`tcli build --package-version x.y.z` locally produces the exact zip CI would publish, in `./build/`
(gitignored), which is the cheapest way to eyeball a release before shipping it.

`tcli build` emits three `was added multiple times` warnings for `icon.png`, `README.md` and
`manifest.json`. That's expected: MSBuild stages them into the Thunderstore folder so the
manual-fallback zip is a valid package on its own, and tcli also places them from `[build]`
icon/readme. The copy step runs last, so the MSBuild-staged files win and the content is
equivalent.
