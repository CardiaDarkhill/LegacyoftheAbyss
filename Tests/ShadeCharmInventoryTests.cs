#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using Xunit;

[Collection(ShadeRuntimeCollection.Name)]
public class ShadeCharmInventoryTests
{
    /// <summary>
    /// The "new" marker is only written to the save slot by the change event, so clearing it
    /// silently meant it came back on the next launch.
    /// </summary>
    [Fact]
    public void ClearingANewCharmMarkerRaisesTheChangeThatPersistsIt()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.ShamanStone);
        Assert.True(inventory.IsNewlyDiscovered(ShadeCharmId.ShamanStone));

        int raised = 0;
        inventory.StateChanged += () => raised++;

        Assert.True(inventory.MarkCharmSeen(ShadeCharmId.ShamanStone));
        Assert.Equal(1, raised);
        Assert.False(inventory.IsNewlyDiscovered(ShadeCharmId.ShamanStone));

        // Already seen: nothing changed, so nothing is announced.
        Assert.False(inventory.MarkCharmSeen(ShadeCharmId.ShamanStone));
        Assert.Equal(1, raised);
    }

    [Fact]
    public void TryEquipRequiresOvercharmAttemptsBeforeExceedingCapacity()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.ShamanStone);
        inventory.GrantCharm(ShadeCharmId.SoulCatcher);

        Assert.True(inventory.TryEquip(ShadeCharmId.ShamanStone, out _));
        Assert.Equal(3, inventory.UsedNotches);
        Assert.False(inventory.IsOvercharmed);
        Assert.Equal(inventory.OvercharmAttemptThreshold, inventory.RemainingOvercharmAttempts);

        Assert.False(inventory.TryEquip(ShadeCharmId.SoulCatcher, out var firstAttempt));
        Assert.Contains("notches", firstAttempt, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(inventory.OvercharmAttemptThreshold - 1, inventory.RemainingOvercharmAttempts);

        Assert.False(inventory.TryEquip(ShadeCharmId.SoulCatcher, out _));
        Assert.Equal(inventory.OvercharmAttemptThreshold - 2, inventory.RemainingOvercharmAttempts);

        Assert.False(inventory.TryEquip(ShadeCharmId.SoulCatcher, out _));
        Assert.Equal(1, inventory.RemainingOvercharmAttempts);

        Assert.True(inventory.TryEquip(ShadeCharmId.SoulCatcher, out var successMessage));
        Assert.True(inventory.IsOvercharmed);
        Assert.Equal(0, inventory.RemainingOvercharmAttempts);
        Assert.Equal(5, inventory.UsedNotches);
        Assert.Contains("overcharm", successMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UnequipDropsOvercharmStatusWhenCapacityRespected()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.ShamanStone);
        inventory.GrantCharm(ShadeCharmId.SoulCatcher);

        inventory.TryEquip(ShadeCharmId.ShamanStone, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);
        inventory.TryEquip(ShadeCharmId.SoulCatcher, out _);

        Assert.True(inventory.IsOvercharmed);
        Assert.True(inventory.TryUnequip(ShadeCharmId.SoulCatcher, out _));
        Assert.False(inventory.IsOvercharmed);
        Assert.Equal(inventory.OvercharmAttemptThreshold, inventory.RemainingOvercharmAttempts);
    }

    [Fact]
    public void CannotEquipWhenNoNotchesRemain()
    {
        var inventory = new ShadeCharmInventory();
        inventory.NotchCapacity = 0;
        inventory.GrantCharm(ShadeCharmId.WaywardCompass);

        Assert.False(inventory.TryEquip(ShadeCharmId.WaywardCompass, out var message));
        Assert.Contains("notch", message, StringComparison.OrdinalIgnoreCase);
        Assert.False(inventory.IsEquipped(ShadeCharmId.WaywardCompass));
        Assert.Equal(0, inventory.UsedNotches);
    }

    [Fact]
    public void VoidHeartAutomaticallyEquippedOutsideDebug()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        var equipped = inventory.GetEquipped().ToArray();
        Assert.Single(equipped);
        Assert.Equal(ShadeCharmId.VoidHeart, equipped[0]);
    }

    [Fact]
    public void VoidHeartCannotBeUnequippedOutsideDebug()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        Assert.False(inventory.TryUnequip(ShadeCharmId.VoidHeart, out var message));
        Assert.Contains("Void Heart", message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(ShadeCharmId.VoidHeart, inventory.GetEquipped());
    }

    [Fact]
    public void VoidHeartCanCoexistWithKingsoul()
    {
        var inventory = new ShadeCharmInventory();
        inventory.GrantCharm(ShadeCharmId.Kingsoul);
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        inventory.NotchCapacity = 10;
        Assert.True(inventory.TryEquip(ShadeCharmId.Kingsoul, out _));

        var equipped = inventory.GetEquipped().ToArray();
        Assert.Contains(ShadeCharmId.Kingsoul, equipped);
        Assert.Equal(ShadeCharmId.VoidHeart, equipped[0]);
    }

    [Fact]
    public void VoidHeartCanBeUnequippedDuringDebugMode()
    {
        ShadeRuntime.SaveSlots.ResetAll();
        ShadeRuntime.Clear();
        var inventory = ShadeRuntime.Charms;
        inventory.ResetLoadout();
        inventory.GrantCharm(ShadeCharmId.VoidHeart);

        bool debugEnabled = ShadeRuntime.ToggleDebugUnlockAllCharms();

        try
        {
            Assert.True(debugEnabled);
            Assert.True(inventory.TryUnequip(ShadeCharmId.VoidHeart, out _));
        }
        finally
        {
            if (ShadeRuntime.IsDebugCharmModeActive())
            {
                ShadeRuntime.ToggleDebugUnlockAllCharms();
            }

            ShadeRuntime.Clear();
        }
    }

    [Fact]
    public void VoidHeartGrantedWhenEnteringSongTowerDestroyedScene()
    {
        // The collected-charm set lives in the save-slot repository, which ShadeRuntime.Clear()
        // deliberately leaves alone - without this reset the charm is already collected from an
        // earlier test and TryCollectCharm reports "nothing new".
        ShadeRuntime.SaveSlots.ResetAll();
        ShadeRuntime.Clear();

        try
        {
            var inventory = ShadeRuntime.Charms;
            inventory.ResetLoadout();

            Assert.False(inventory.IsOwned(ShadeCharmId.VoidHeart));
            Assert.False(ShadeRuntime.IsCharmCollected(ShadeCharmId.VoidHeart));

            ShadeRuntime.HandleSceneEntered("Song_Tower_Destroyed");

            Assert.True(inventory.IsOwned(ShadeCharmId.VoidHeart));
            Assert.True(ShadeRuntime.IsCharmCollected(ShadeCharmId.VoidHeart));
            Assert.Contains(ShadeCharmId.VoidHeart, inventory.GetEquipped());
        }
        finally
        {
            ShadeRuntime.Clear();
        }
    }

    /// <summary>
    /// The save format. A slot records owned, discovered and equipped charms as plain integers -
    /// <c>QuickSlash</c> is the number 5 on disk - so an ordinal that has already shipped can never
    /// move: inserting a charm anywhere but the end renumbers everything below it and hands existing
    /// saves the wrong charms, with nothing in the diff of the reorder to show for it.
    /// <para>
    /// Adding a charm costs one line here. The count check is what forces that line: a new member
    /// left unpinned fails rather than passing silently.
    /// </para>
    /// </summary>
    [Fact]
    public void CharmIdsKeepTheOrdinalsThatAreOnDisk()
    {
        var shipped = new Dictionary<ShadeCharmId, int>
        {
            { ShadeCharmId.WaywardCompass, 0 },
            { ShadeCharmId.Sprintmaster, 1 },
            { ShadeCharmId.Dashmaster, 2 },
            { ShadeCharmId.ShamanStone, 3 },
            { ShadeCharmId.SpellTwister, 4 },
            { ShadeCharmId.QuickSlash, 5 },
            { ShadeCharmId.MarkOfPride, 6 },
            { ShadeCharmId.Longnail, 7 },
            { ShadeCharmId.SoulCatcher, 8 },
            { ShadeCharmId.FragileStrength, 9 },
            { ShadeCharmId.SoulEater, 10 },
            { ShadeCharmId.Grubsong, 11 },
            { ShadeCharmId.QuickFocus, 12 },
            { ShadeCharmId.DeepFocus, 13 },
            { ShadeCharmId.ShapeOfUnn, 14 },
            { ShadeCharmId.SteadyBody, 15 },
            { ShadeCharmId.StalwartShell, 16 },
            { ShadeCharmId.FuryOfTheFallen, 17 },
            { ShadeCharmId.NailmastersGlory, 18 },
            { ShadeCharmId.CarefreeMelody, 19 },
            { ShadeCharmId.FragileHeart, 20 },
            { ShadeCharmId.SharpShadow, 21 },
            { ShadeCharmId.GrubberflysElegy, 22 },
            { ShadeCharmId.FragileGreed, 23 },
            { ShadeCharmId.HeavyBlow, 24 },
            { ShadeCharmId.BaldurShell, 25 },
            { ShadeCharmId.LifebloodHeart, 26 },
            { ShadeCharmId.LifebloodCore, 27 },
            { ShadeCharmId.JonisBlessing, 28 },
            { ShadeCharmId.Hiveblood, 29 },
            { ShadeCharmId.Kingsoul, 30 },
            { ShadeCharmId.VoidHeart, 31 },
            { ShadeCharmId.Weaversong, 32 },
            { ShadeCharmId.DefendersCrest, 33 },
            { ShadeCharmId.Flukenest, 34 },
            { ShadeCharmId.SporeShroom, 35 },
            { ShadeCharmId.ThornsOfAgony, 36 },
            { ShadeCharmId.GlowingWomb, 37 },
            { ShadeCharmId.GatheringSwarm, 38 },
            { ShadeCharmId.Grimmchild, 39 },
            { ShadeCharmId.DreamWielder, 40 },
            { ShadeCharmId.Dreamshield, 41 },
        };

        foreach (var pair in shipped)
        {
            Assert.Equal(pair.Value, (int)pair.Key);
        }

        Assert.Equal(shipped.Count, Enum.GetValues(typeof(ShadeCharmId)).Length);
    }

    /// <summary>
    /// A charm with nothing wired still equips and still spends a notch, and from the player's side
    /// that is indistinguishable from one whose effect is simply subtle - the failure this whole
    /// codebase keeps hitting. Two charms are inert on purpose and say so in their own description;
    /// Wayward Compass is wired outside the definition table, in <c>LegacyHelper.MapMarkers</c>.
    /// Anything else reaching this list is a charm that was added and never connected.
    /// </summary>
    [Fact]
    public void EveryCharmIsWiredToSomethingOrSaysItIsNotYet()
    {
        // Wired somewhere other than a modifier, a toggle or a hook.
        var wiredElsewhere = new HashSet<ShadeCharmId> { ShadeCharmId.WaywardCompass };

        var inert = new List<string>();
        foreach (var charm in new ShadeCharmInventory().AllCharms)
        {
            if (charm.EnumId is { } id && wiredElsewhere.Contains(id))
            {
                continue;
            }

            if (HasAnyEffect(charm))
            {
                continue;
            }

            if (charm.Description != null
                && charm.Description.Contains("implemented at a later date", StringComparison.Ordinal))
            {
                continue;
            }

            inert.Add(charm.DisplayName);
        }

        Assert.True(
            inert.Count == 0,
            "These charms do nothing, and their description does not admit it: " + string.Join(", ", inert));
    }

    /// <summary>
    /// Reflected over rather than listed, so a modifier or toggle added later is covered without
    /// anyone remembering to come back here.
    /// </summary>
    private static bool HasAnyEffect(ShadeCharmDefinition charm)
    {
        var hooks = charm.Hooks;
        if (hooks.OnApplied != null
            || hooks.OnRemoved != null
            || hooks.OnUpdate != null
            || hooks.OnShadeDamaged != null)
        {
            return true;
        }

        const BindingFlags instanceProperties = BindingFlags.Public | BindingFlags.Instance;

        object toggles = charm.AbilityToggles;
        foreach (var property in typeof(ShadeCharmAbilityToggles).GetProperties(instanceProperties))
        {
            if (property.GetValue(toggles) != null)
            {
                return true;
            }
        }

        // Every modifier is a multiplier sitting at 1 or a delta sitting at 0 until a charm moves
        // it, so "equal to Identity" is exactly "this charm does not touch that stat".
        object modifiers = charm.StatModifiers;
        object identity = ShadeCharmStatModifiers.Identity;
        foreach (var property in typeof(ShadeCharmStatModifiers).GetProperties(instanceProperties))
        {
            if (!Equals(property.GetValue(modifiers), property.GetValue(identity)))
            {
                return true;
            }
        }

        return false;
    }
}
