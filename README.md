# Legacy of the Abyss

A full co-op experience for Hollow Knight: Silksong. One player controls Hornet (controller by
default) while the other controls **the Shade** (mouse and keyboard by default) — a second
playable character with its own flight-based moveset, spells, charms and HUD.

Player-facing documentation — full feature list, FAQ, and known limitations — lives in
[`BuildTemplates/Thunderstore/README.md`](BuildTemplates/Thunderstore/README.md), which is the
README shipped with the released package.

## Downloads

- **Thunderstore:** <https://thunderstore.io/c/hollow-knight-silksong/p/CardiaDarkhill/LegacyoftheAbyss/>
- **Nexus Mods:** see the mod page linked from the Thunderstore listing.

Requires [BepInExPack for Silksong](https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/BepInExPack_Silksong/).

## Building from source

```
dotnet build LegacyoftheAbyss.csproj -c Release -p:Version=1.1.0
```

The repo is designed to be checked out directly inside `BepInEx/plugins/`, so build output is
redirected outside the game tree — see the comments in `Directory.Build.props` for why. Useful
opt-in properties:

| Property | Effect |
| --- | --- |
| `-p:CreateDistributionPackages=true` | Stages the Nexus and Thunderstore release folders under `obj/Release/`. |
| `-p:DeployLocalDevBuild=true` | Copies the built DLL into the adjacent `BepInEx/plugins/` folder, and into the mod manager profile too when `DevProfile.props` is present. |
| `-p:DeployDevProfile=true` | Deploys into a Thunderstore Mod Manager / r2modman profile only (needs `DevProfile.props`). Implied by `DeployLocalDevBuild`; pass `false` to opt out. |

Tests: `dotnet test Tests/LegacyoftheAbyss.Tests.csproj` — these load the game's own assemblies
at runtime, so they need a real Silksong install (see `SilksongPath.props`).

## Bug reports

Pressing `F8` in-game freezes the game and files a bug report: whatever you type, plus a
screenshot of the frame before the overlay drew, a full state snapshot, the captured log ring
from every BepInEx source, and roughly thirty seconds of rolling Hornet/Shade samples leading up
to the keypress. Mod-code exceptions file one automatically.

Reports land in `BepInEx/config/LegacyoftheAbyss/bug_reports/` — deliberately outside this
repository, so they survive a mod reinstall and cannot be committed by accident. `index.md` there
is the open/fixed ledger. See `Diagnostics/` for the implementation and `AGENTS.md` for the parts
list; `/bug-triage` is the slash command for working through them.

## Releasing

Publishing to both Nexus and Thunderstore is a single manual workflow run — see
[`PUBLISHING.md`](PUBLISHING.md).

## Redistribution

Please ask before redistributing modified copies; see the "Can I copy/redistribute your mod"
entry in the player-facing README.
