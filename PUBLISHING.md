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
   via Nexus's Upload API (currently open beta).
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

1. On nexusmods.com: **Settings → API Keys** (or wherever the Upload API open-beta key
   generation currently lives — this is new enough that the exact menu location may have
   moved since I last checked) → generate a personal API key.
2. Find your existing mod's **file ID** — on your mod's Nexus page, the file-management
   view (or the "API Info" option mentioned in the upload-action's docs) shows the numeric
   ID for the file slot you're versioning.
3. In the GitHub repo, add secrets: `NEXUS_API_KEY` (the key from step 1) and repo
   *variables* (not secret, just identifiers) `NEXUS_GAME_DOMAIN` (Silksong's Nexus game
   domain slug — confirm exact slug on your mod's URL), `NEXUS_MOD_ID`, and `NEXUS_FILE_ID`.

**Before relying on this for real:** the Nexus upload action's exact `with:` input names in
the workflow below are my best read of its README, but I couldn't fully verify them against
the raw source in this session — Nexus's beta API and this action are both new enough that I
worked from paraphrased fetches, not the literal file. Check
<https://github.com/Nexus-Mods/upload-action#readme> against the workflow before your first
real run, and expect to need a small fix if an input name has changed.

## Also worth doing before the first automated publish

`BuildTemplates/Thunderstore/manifest.template.json` still lists a dependency on
`BepInEx-BepInExPack_Silksong-5.4.2303` — that's the **deprecated** Thunderstore listing
(it now just redirects to the maintained one). I've updated the namespace to
`silksong_modding` in this pass, but left the version number as a `TODO` — Thunderstore
dependency strings need an exact version match, and I didn't want to guess. Check
<https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/BepInExPack_Silksong/>
for the current version before your next release and fill it in.

## Manual fallback

Everything above is optional convenience — the manual path from before still works exactly
as it did: `dotnet build -c Release -p:Version=x.y.z` locally, zip the two `obj/Release/...`
folders yourself, upload through each site's web UI.
