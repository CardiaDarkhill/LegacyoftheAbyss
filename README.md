# Legacy of the Abyss

A full co-op experience for Hollow Knight: Silksong. One player controls Hornet (controller by
default) while the other controls **the Shade** (mouse and keyboard by default) — a second
playable character with its own flight-based moveset, spells, charms and HUD.

## Documentation

**All documentation lives in the [wiki](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki).**

| | |
| --- | --- |
| **Players** | [Installation](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Installation) · [Playing the Shade](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Playing-the-Shade) · [Controls](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Controls-and-Bindings) · [Charms](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Shade-Charms) · [Skins](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Skins) · [Config reference](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Configuration-Reference) · [FAQ](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/FAQ) |
| **Contributors** | [Developer setup](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Developer-Setup) · [Building and testing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Building-and-Testing) · [Architecture](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Architecture-Overview) · [Code map](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Code-Map) · [Roadmap](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Roadmap) · [Publishing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release) · [Contributing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Contributing) |

The text shipped with the released package is
[`BuildTemplates/Thunderstore/README.md`](BuildTemplates/Thunderstore/README.md).
[`AGENTS.md`](AGENTS.md) is the working brief for coding agents.

## Downloads

- **Thunderstore:** <https://thunderstore.io/c/hollow-knight-silksong/p/CardiaDarkhill/LegacyoftheAbyss/>
- **Nexus Mods:** see the mod page linked from the Thunderstore listing.

Requires [BepInExPack for Silksong](https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/BepInExPack_Silksong/).

## Building from source

```
dotnet build LegacyoftheAbyss.csproj -c Release -p:Version=1.1.0
```

The repo is designed to be checked out directly inside `BepInEx/plugins/`, so build output is
redirected outside the game tree — see
[Developer setup](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Developer-Setup) for why
that matters, and
[Building and testing](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Building-and-Testing)
for the full list of opt-in deploy properties. The short version:

| Property | Effect |
| --- | --- |
| `-p:DeployLocalDevBuild=true` | Copies the built DLL into the adjacent `BepInEx/plugins/` folder, and into the mod manager profile too when `DevProfile.props` is present. **This is what makes a rebuild show up in-game.** |
| `-p:CreateDistributionPackages=true` | Stages the Nexus and Thunderstore release folders under `obj/Release/`. |

Tests: `dotnet test Tests/LegacyoftheAbyss.Tests.csproj`.

## Bug reports

Pressing `F8` in-game freezes the game and files a bug report: whatever you type, plus a
screenshot of the frame before the overlay drew, a full state snapshot, the captured log ring
from every BepInEx source, and roughly thirty seconds of rolling Hornet/Shade samples leading up
to the keypress. Mod-code exceptions file one automatically.

Reports land in `BepInEx/config/LegacyoftheAbyss/bug_reports/` — deliberately outside this
repository, so they survive a mod reinstall and cannot be committed by accident. `index.md` there
is the open/fixed ledger. `/bug-triage` is the slash command for working through them; the
implementation is documented on the
[Bug report system](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Bug-Report-System)
wiki page.

## Releasing

Publishing to both Nexus and Thunderstore is a single manual workflow run — see
[Publishing a release](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/Publishing-a-Release).

## Redistribution

Please ask before redistributing modified copies; see the "Can I copy/redistribute your mod"
entry in the
[FAQ](https://github.com/CardiaDarkhill/LegacyoftheAbyss/wiki/FAQ).
