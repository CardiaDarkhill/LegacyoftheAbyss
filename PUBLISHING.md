# Publishing — Nexus Mods & Thunderstore

One-time setup below, then every release is: **GitHub → Actions tab → "Publish Release" →
Run workflow → type the version number → Run.** Nothing publishes on a normal `git push`.

## What the workflow does

`.github/workflows/publish.yml` runs on a GitHub-hosted runner (full internet access, so the
BepInEx NuGet feed that's blocked from Cowork's sandbox isn't a problem there):

1. Checks out the repo, installs the .NET SDK.
2. `dotnet build -c Release -p:Version=<the version you typed>` — this is the same build you
   already run locally; it triggers the existing `PrepareReleasePackages` MSBuild target,
   producing `obj/Release/Nexus/LegacyoftheAbyss-<version>/` and
   `obj/Release/Thunderstore/LegacyoftheAbyss-<version>/` exactly like a local Release build
   does.
3. Zips the Nexus folder and uploads it to your existing Nexus mod page as a new file version,
   via Nexus's Upload API (currently open beta), using `Nexus-Mods/upload-action`.
4. Runs `tcli build` + `tcli publish` to zip and publish the Thunderstore folder as a new
   package version, via Thunderstore's official CLI.

Both platform pushes run in the same job — if you'd rather ship to just one at a time, comment
out the step you don't want for that run, or split them into two workflow files later.

## One-time setup — Thunderstore

1. Locally: `dotnet tool install -g tcli`, then from the repo root run `tcli init`. This
   generates a `thunderstore.toml` with placeholder fields.
2. Edit the generated `thunderstore.toml`:
   - `namespace` — your Thunderstore team name (whatever you publish under).
   - `name` — `LegacyoftheAbyss`.
   - Community — Silksong's community slug is confirmed as `hollow-knight-silksong`
     (from `https://thunderstore.io/c/hollow-knight-silksong/`).
   - Under `[build.copy]`, point the source at the folder MSBuild already assembles —
     `obj/Release/Thunderstore/LegacyoftheAbyss-<version>/` — as a single copy source
     mapped to the package root, rather than re-listing the DLL/README/icon individually.
     That way there's one place (the existing MSBuild target) that decides what's *in* the
     package, and tcli's only job is zip + upload.
3. Commit `thunderstore.toml` to the repo.
4. On thunderstore.io: **Settings → Teams → [your team] → Service Accounts** → create a
   token (starts with `tss_`).
5. In the GitHub repo: **Settings → Secrets and variables → Actions** → add secret
   `TCLI_AUTH_TOKEN` with that value.

## One-time setup — Nexus Mods

Verified directly against `Nexus-Mods/upload-action`'s `action.yml` and README (not just
paraphrased docs) this pass — the setup below reflects the action's actual inputs.

1. On nexusmods.com: **Settings → API Keys** (`https://www.nexusmods.com/settings/api-keys`)
   → generate a personal API key.
2. Find your existing mod's **file ID** — on your mod's public Files tab, use the "API Info"
   option, or check the edit view on the Manage Files page. This one ID is enough to identify
   the upload target; the action has no separate game-domain or mod-ID input.
3. In the GitHub repo (**Settings → Secrets and variables → Actions**):
   - Add secret `NEXUSMODS_API_KEY` with the key from step 1 (matches the action's own docs,
     so it's recognizable later even outside this workflow).
   - Add repo *variable* `NEXUS_FILE_ID` with the numeric file ID from step 2.

The workflow's `Upload to Nexus Mods` step passes `api_key`, `file_id`, `filename`, `version`,
and `display_name` — all inputs the action actually defines. Optional inputs it also supports
if you want them later: `description`, `category` (defaults to `main`),
`archive_existing_version`, `primary_mod_manager_download`, `allow_mod_manager_download`,
`show_requirements_pop_up`.

## Also fixed this pass

`BuildTemplates/Thunderstore/manifest.template.json` was listing a dependency on the
**deprecated** `BepInEx-BepInExPack_Silksong` listing (it just redirects to the maintained one
now) with no version pinned. Updated to the maintained namespace and its current exact version,
confirmed directly against the live Thunderstore listing:
`silksong_modding-BepInExPack_Silksong-1.0.3`. That listing's version will drift over time —
worth a quick check against
<https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/BepInExPack_Silksong/>
next time you notice the dependency failing to resolve for someone.

## Manual fallback

Everything above is optional convenience — the manual path from before still works exactly
as it did: `dotnet build -c Release -p:Version=x.y.z` locally, zip the two `obj/Release/...`
folders yourself, upload through each site's web UI.
