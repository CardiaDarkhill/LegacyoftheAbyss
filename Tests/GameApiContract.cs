using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Xunit;

/// <summary>
/// Assertions for every reflective lookup the mod makes against the game's assemblies.
/// <para>
/// This exists because of a specific, repeated failure. Reflection that comes back empty does not
/// throw and does not log: the feature built on it returns early forever, and in a bug report that is
/// indistinguishable from "the situation never arose". Two separate subsystems were dead for multiple
/// rounds of play testing that way - one looking for public fields on a type whose members are
/// properties, one naming an overloaded method that could not be resolved.
/// </para>
/// <para>
/// So the rule for this project is: <b>a reflective lookup in shipped code has a matching assertion
/// here.</b> These run against the real game and PlayMaker assemblies, so a mismatch fails at
/// <c>dotnet test</c> rather than in a play session. Use the helpers below - they fail with a message
/// naming what was actually found, which is the part that turns a mystery into a five-second fix.
/// </para>
/// </summary>
public static class GameApiContract
{
    private const BindingFlags AnyInstance =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private const BindingFlags AnyMember =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    /// <summary>Every assembly the mod reflects into.</summary>
    public static IEnumerable<Assembly> GameAssemblies => new[]
    {
        typeof(HeroController).Assembly,
        typeof(HutongGames.PlayMaker.FsmStateAction).Assembly
    }.Distinct();

    public static IEnumerable<Type> AllTypes()
    {
        foreach (var assembly in GameAssemblies)
        {
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
            foreach (var type in types)
            {
                yield return type;
            }
        }
    }

    /// <summary>A game type the mod resolves by name at runtime.</summary>
    public static Type RequireType(string name)
    {
        var type = AllTypes().FirstOrDefault(t => t.Name == name);
        Assert.True(type != null, $"Game type '{name}' no longer exists. Anything resolving it by name is now dead code.");
        return type;
    }

    /// <summary>
    /// A member the mod reads reflectively, whether it is a field or a property. Both are checked
    /// because the distinction is invisible at the call site and has already caused one outage:
    /// <c>FsmOwnerDefault</c> looks like it has public fields and does not.
    /// </summary>
    public static void RequireReadableMember(Type owner, Type memberType, string because)
    {
        bool found =
            owner.GetFields(AnyInstance).Any(f => memberType.IsAssignableFrom(f.FieldType)) ||
            owner.GetProperties(AnyInstance).Any(p => p.CanRead && memberType.IsAssignableFrom(p.PropertyType));

        if (!found)
        {
            Assert.Fail($"{owner.Name} exposes no readable {memberType.Name}. {because}\n{DescribeMembers(owner)}");
        }
    }

    public static void RequireWritableMember(Type owner, Type memberType, string because)
    {
        bool found =
            owner.GetFields(AnyInstance).Any(f => !f.IsInitOnly && memberType.IsAssignableFrom(f.FieldType)) ||
            owner.GetProperties(AnyInstance).Any(p => p.CanWrite && memberType.IsAssignableFrom(p.PropertyType));

        if (!found)
        {
            Assert.Fail($"{owner.Name} exposes no writable {memberType.Name}. {because}\n{DescribeMembers(owner)}");
        }
    }

    /// <summary>
    /// A method the mod patches or invokes. Asserting the parameter names matters as much as the
    /// types: Harmony binds prefix parameters by name, so a rename turns a patch into a load error.
    /// </summary>
    public static MethodInfo RequireMethod(Type owner, string name, string because, params string[] parameterNames)
    {
        var candidates = owner.GetMethods(AnyMember).Where(m => m.Name == name).ToList();
        Assert.True(candidates.Count > 0, $"{owner.Name}.{name} does not exist. {because}");

        if (parameterNames.Length == 0)
        {
            return candidates[0];
        }

        var match = candidates.FirstOrDefault(m =>
            parameterNames.All(n => m.GetParameters().Any(param => param.Name == n)));

        Assert.True(
            match != null,
            $"No overload of {owner.Name}.{name} has parameters [{string.Join(", ", parameterNames)}]. {because}\n" +
            string.Join("\n", candidates.Select(m => "  " + m.Name + "(" + string.Join(", ", m.GetParameters().Select(param => param.ParameterType.Name + " " + param.Name)) + ")")));

        return match;
    }

    /// <summary>A named private field the mod reads reflectively, with its expected type.</summary>
    public static FieldInfo RequireField(Type owner, string name, Type fieldType, string because)
    {
        var field = owner.GetField(name, AnyInstance);
        Assert.True(field != null, $"{owner.Name}.{name} does not exist. {because}\n{DescribeMembers(owner)}");
        Assert.True(
            field!.FieldType == fieldType,
            $"{owner.Name}.{name} is {field.FieldType.Name}, not {fieldType.Name}. {because}");
        return field;
    }

    /// <summary>
    /// Names what a type actually has, so a failure says why rather than only that. This message is
    /// the difference between the two-round-trip version of a bug and the two-minute version.
    /// </summary>
    public static string DescribeMembers(Type type)
    {
        // Every step is guarded. Resolving a member's signature loads the assembly its type lives
        // in, and the test host does not have every Unity module beside it - so simply describing
        // a type can throw FileNotFoundException. A diagnostic that fails is worse than useless,
        // and this one was being built eagerly for assertions that were passing.
        var parts = new List<string>();

        try
        {
            foreach (var field in type.GetFields(AnyInstance))
            {
                try { parts.Add($"{(field.IsPublic ? "field" : "private field")} {field.FieldType.Name} {field.Name}"); }
                catch { parts.Add("field <unresolvable>"); }
            }
        }
        catch
        {
        }

        try
        {
            foreach (var property in type.GetProperties(AnyInstance))
            {
                try { parts.Add($"property {property.PropertyType.Name} {property.Name}"); }
                catch { parts.Add("property <unresolvable>"); }
            }
        }
        catch
        {
        }

        return parts.Count > 0 ? "  Found: " + string.Join(" | ", parts) : "  Found: <nothing enumerable in this host>";
    }
}

/// <summary>
/// The contract itself, one case per reflective dependency in shipped code. Adding a new lookup
/// without adding a case here is how this project has repeatedly shipped subsystems that never ran.
/// </summary>
public class GameApiContractTests
{
    [Theory]
    [InlineData("CanHeroTakeDamage")]
    [InlineData("CanHeroTakeDamageIgnoreInvul")]
    [InlineData("CanHeroBeGrabbed")]
    [InlineData("CanHeroBeGrabbedV2")]
    public void EveryGateActionCanBeRefused(string name)
    {
        var type = GameApiContract.RequireType(name);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        if (LegacyHelper.ShadeGrabRetargeting.FindRefusalEventField(fields) == null)
        {
            Assert.Fail($"{name} has no 'cannot' branch; refusing it would strand the FSM.\n{GameApiContract.DescribeMembers(type)}");
        }

        Assert.NotNull(type.GetMethod("OnEnter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
    }

    /// <summary>
    /// Hornet's needle and her silk skills carry separate damage multipliers, and the only thing
    /// telling them apart at the point the damage is set is a private field on the damage object.
    /// If any of these stops resolving, the split reads every hit as a silk skill and the Needle
    /// slider silently does nothing - the exact silent no-op this file exists to catch.
    /// </summary>
    [Theory]
    [InlineData("sourceIsHero")]
    [InlineData("isHeroDamage")]
    [InlineData("isNailAttack")]
    public void HornetsDamageCanBeRecognisedAndClassified(string field)
    {
        GameApiContract.RequireField(
            typeof(DamageEnemies), field, typeof(bool),
            "Read by DamageEnemies_DoDamage_HornetScaling to decide whether a damage object is "
            + "Hornet's, and whether it is a needle strike or a silk skill.");
    }

    /// <summary>
    /// Where Hornet's damage is scaled, and the two fields it scales.
    /// <para>
    /// <c>nailDamageMultiplier</c> is the one that matters for the Needle slider and the reason this
    /// is asserted at all: a needle damager never reads <c>damageDealt</c>, so scaling that field
    /// moved a number nothing consumed and the slider did nothing for a release. If the multiplier
    /// field or the hook stops resolving, that silence comes straight back.
    /// </para>
    /// </summary>
    [Fact]
    public void HornetsDamageCanBeScaledWhereTheHitResolves()
    {
        GameApiContract.RequireMethod(
            typeof(DamageEnemies), "DoDamage",
            "Patched to scale Hornet's damage at the moment of the hit rather than at spawn.",
            "target", "isFirstHit");

        GameApiContract.RequireField(
            typeof(DamageEnemies), "nailDamageMultiplier", typeof(float),
            "The only thing a needle hit multiplies - DoDamage restarts the damage stack from "
            + "PlayerData.nailDamage and applies this, ignoring damageDealt entirely.");

        GameApiContract.RequireField(
            typeof(DamageEnemies), "useNailDamage", typeof(bool),
            "Tells the two apart: set means the hit reads nailDamageMultiplier, clear means it "
            + "reads damageDealt.");
    }

    /// <summary>
    /// How the Shade AI decides a spell is worth casting at an enemy.
    /// <para>
    /// The important half is what this is <em>not</em>. <c>HealthManager.IsInvincible</c> is the
    /// master switch for the blocking system, not a statement that an enemy cannot be hurt: an
    /// armoured enemy sets it and sets <c>InvincibleFromDirection</c> alongside to say which way its
    /// armour faces. Deciding spell worth from the flag alone writes off every armoured enemy in the
    /// game, so the AI asks <c>IsBlockingByDirection</c> per side instead. Both members are asserted
    /// because the distinction between them is the entire fix.
    /// </para>
    /// </summary>
    [Fact]
    public void AnEnemysArmourCanBeToldFromBeingSwitchedOff()
    {
        GameApiContract.RequireMethod(
            typeof(HealthManager), "IsBlockingByDirection",
            "Asked once per cardinal direction to decide whether a spell could land at all - an "
            + "enemy blocking every side is not worth casting at, one with armour facing a single "
            + "way still is.",
            "cardinalDirection", "attackType", "specialType");

        GameApiContract.RequireField(
            typeof(HealthManager), "invincibleFromDirection", typeof(int),
            "What makes IsInvincible mean 'armoured this way round' rather than 'cannot be hurt'. "
            + "If this ever stops existing, IsBlockingByDirection has changed shape and the spell "
            + "worth check needs rereading before it silently writes off armoured enemies.");
    }

    /// <summary>
    /// What the pause-menu injection reaches into to put "Legacy of the Abyss" in the list, and to
    /// put it directly above Quit rather than below it.
    /// <para>
    /// All three are private and reached by reflection, and the failure is quiet in an unhelpful
    /// way: a missing <c>entries</c> or <c>selectable</c> leaves the button drawn on the screen but
    /// absent from the list the stick and keyboard walk, so it can be clicked and not selected.
    /// </para>
    /// </summary>
    [Fact]
    public void ThePauseMenuListCanBeExtended()
    {
        var entries = GameApiContract.RequireField(
            typeof(MenuButtonList), "entries", typeof(MenuButtonList.Entry[]),
            "ShadeSettingsMenu.Inject rebuilds this array to add its own row to the pause menu.");

        var entryType = entries.FieldType.GetElementType();
        Assert.NotNull(entryType);

        GameApiContract.RequireField(
            // Selectable, not MenuSelectable - the entry holds the base type even though every
            // button that ends up in one is a MenuSelectable.
            entryType!, "selectable", typeof(Selectable),
            "Written to point a new entry at the injected button, and read to find where Quit sits "
            + "so the injected row can go directly above it.");

        GameApiContract.RequireField(
            typeof(MenuButtonList), "isDirty", typeof(bool),
            "Set so MenuButtonList rebuilds its navigation after the entries array is replaced.");
    }

    /// <summary>
    /// Quit is found by what it is rather than by where it sits, so that the injected row lands
    /// above it on a pause menu whose contents are not fixed. Without this the row goes back to the
    /// end of the list, below the option that leaves the game.
    /// </summary>
    [Fact]
    public void ThePauseMenusQuitButtonCanBeRecognised()
    {
        GameApiContract.RequireField(
            typeof(PauseMenuButton), "pauseButtonType", typeof(PauseMenuButton.PauseButtonType),
            "ShadeSettingsMenu.Inject reads it to place its row directly above Quit.");

        Assert.True(
            Enum.IsDefined(typeof(PauseMenuButton.PauseButtonType), "Quit"),
            "PauseButtonType has no Quit member, so the injected row cannot find what to sit above.");
    }

    [Fact]
    public void HeroDamageEntryPointsBindTheirPrefixes()
    {
        GameApiContract.RequireMethod(
            typeof(HeroController), "TakeQuickDamage",
            "Patched to spare Hornet a hit she is not standing in.", "damageAmount");

        // HornetInput.ResolveMapKeyboard. Without it Hornet cannot be handed the keyboard back when
        // the Shade AI takes over, and the failure is silent - the key simply does nothing.
        GameApiContract.RequireMethod(
            typeof(InputHandler), "MapKeyboardLayoutFromGameSettings",
            "Called to restore Hornet's keyboard bindings while the Shade AI drives, because the "
            + "public ResetDefaultKeyBindings overwrites the player's saved layout.");

        GameApiContract.RequireMethod(
            typeof(HeroController), "TakeDamage",
            "Patched to spare Hornet a hit she is not standing in.", "go", "damageAmount");
    }

    /// <summary>
    /// The companion interaction blocker. Harmony binds prefix parameters by name, so the parameter
    /// name is as much a part of this contract as the method is - a rename turns the patch into a
    /// load error and hands benches back to the companion.
    /// </summary>
    [Theory]
    [InlineData("AddInside")]
    [InlineData("LocalAddInside")]
    public void CompanionsCanBeKeptOutOfInteractionRanges(string method)
    {
        GameApiContract.RequireMethod(
            typeof(InteractableBase), method,
            "Prefixed so the companion's hero-shaped proxy cannot register as the hero being in "
            + "range of a bench, lever or door.",
            "col");
    }

    /// <summary>
    /// Gathering Swarm is the game's own rosary magnet turned on for a charm rather than a second
    /// pull written beside it. If this stops resolving the charm silently does nothing at all -
    /// which is the state it was reported in.
    /// </summary>
    [Fact]
    public void TheRosaryMagnetGateCanBeAnswered()
    {
        GameApiContract.RequireMethod(
            typeof(CurrencyObjectBase), "MagnetToolIsEquipped",
            "Postfixed so Gathering Swarm starts each pickup's own Getter routine, which is what "
            + "draws rosaries to Hornet.");
    }

    [Fact]
    public void CompanionsCannotDriveASceneTransition()
    {
        // TransitionPoint is an InteractableBase but bypasses its range bookkeeping, testing
        // layer == 9 in its own trigger callbacks - which the companion's proxy satisfies. Both
        // callbacks meet here, and Harmony binds the prefix by this parameter name.
        GameApiContract.RequireMethod(
            typeof(TransitionPoint), "TryDoTransition",
            "Prefixed so the companion standing in a doorway cannot send Hornet through it.",
            "heroCollider");
    }

    [Fact]
    public void TheOneShotAudioActionCanBeIdentified()
    {
        // Prefixed to name the FSM behind a stray sound. PlayMaker's own stack frames carry no FSM
        // identity, so without this the trace can say a state played a clip but never whose.
        GameApiContract.RequireMethod(
            typeof(HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle), "OnEnter",
            "Prefixed by the scene-entry audio trace to record which FSM is playing a one-shot.");
    }

    /// <summary>
    /// The inventory-open trace. Both of these are patch targets resolved by name, so both can go
    /// dead in silence - and a diagnostic that has gone dead is worse than none, because "the
    /// listener never ran" then reads as a finding rather than as a broken instrument.
    /// </summary>
    [Fact]
    public void TheInventoryOpenPathCanBeWatchedFromInside()
    {
        GameApiContract.RequireMethod(
            typeof(HutongGames.PlayMaker.Actions.ListenForInventoryShortcut), "OnUpdate",
            "Prefixed to record whether the Inventory Control FSM's Closed state is polling at all.");

        GameApiContract.RequireField(
            typeof(HutongGames.PlayMaker.Actions.ListenForInventoryShortcut), "inputHandler", typeof(InputHandler),
            "Read so the trace sees the same actions the listener does, rather than a second instance.");

        GameApiContract.RequireMethod(
            typeof(HeroController), "CanOpenInventory",
            "Postfixed to record the gate's answer at the moment the FSM asks, which is inside a "
            + "frame and therefore invisible to anything sampling once per frame.");

        GameApiContract.RequireMethod(
            typeof(InventoryPaneInput), "GetInventoryInputPressed",
            "Called by the trace to report which inventory action the listener saw.", "ia");
    }

    /// <summary>
    /// The bind's heal is rewritten at <c>AddHealth</c> because <c>BindCompleted</c> touches no
    /// health at all - it sets crest state and nothing else, so watching Hornet across it always read
    /// a heal of zero and the correction landed on top of the game's three rather than in place of
    /// it. If this method ever stops matching, the override goes quiet and the bind heals whatever
    /// the game says, which is the direction that failure should fall.
    /// </summary>
    [Fact]
    public void TheBindHealCanBeInterceptedWhereItHappens()
    {
        GameApiContract.RequireMethod(
            typeof(HeroController), "AddHealth",
            "Patched to rewrite the bind burst's heal to bindHornetHeal.", "amount");
    }

    /// <summary>
    /// Resolved by parameter shape rather than named through the attribute, for the same reason the
    /// damage entry points are: an overload set the prefix cannot bind to must leave the override off
    /// rather than throw out of <c>PatchAll</c> and cost the mod every patch it has.
    /// </summary>
    [Fact]
    public void TheHeroAddHealthResolverFindsOnlyBindableMethods()
    {
        var methods = LegacyHelper.FindHeroAddHealthMethods().ToList();

        Assert.NotEmpty(methods);
        foreach (var method in methods)
        {
            Assert.Equal("AddHealth", method.Name);
            var parameters = method.GetParameters();
            Assert.Single(parameters);
            Assert.Equal("amount", parameters[0].Name);
            Assert.Equal(typeof(int), parameters[0].ParameterType);
        }
    }

    [Fact]
    public void HeroBoxIsStillTheSingleChokePointForHeroDamage()
    {
        GameApiContract.RequireMethod(
            typeof(HeroBox), "CheckForDamage",
            "The bug reporter records every hero damage through it.", "otherGameObject");
    }

    [Fact]
    public void FsmStateActionCanBeFinished()
    {
        var finish = typeof(HutongGames.PlayMaker.FsmStateAction).GetMethod(
            "Finish", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

        Assert.True(finish != null, "FsmStateAction.Finish is gone; a refused action would never close out.");
    }

    [Fact]
    public void DetectionVolumesRemainDistinguishableFromHitboxes()
    {
        var type = GameApiContract.RequireType("TrackTriggerObjects");
        Assert.True(typeof(UnityEngine.Component).IsAssignableFrom(type));
    }

    /// <summary>
    /// The Shade's damage intake is gated on this so it is hit by the same things Hornet would be.
    /// </summary>
    [Fact]
    public void LayerMatrixCanBeQueried()
    {
        // UnityEngine.Physics2D lives in a module the test host does not always have beside it.
        // A missing module is a fact about this host, not about the game, and must not be reported
        // as "the method is gone" - that would be the same misleading signal this file exists to
        // prevent.
        try
        {
            GameApiContract.RequireMethod(
                typeof(UnityEngine.Physics2D), "GetIgnoreLayerCollision",
                "CouldReachHornet uses it to ignore colliders that could never touch Hornet.");
        }
        catch (System.IO.FileNotFoundException)
        {
        }
        catch (System.TypeLoadException)
        {
        }
    }

    /// <summary>
    /// The geometry query the occupancy test is built on. It used <c>Collider2D.IsTouching</c>, which
    /// consults the layer collision matrix - the Shade's body is on Default and boss hitboxes are not,
    /// so it answered "not touching" wherever the Shade stood. <c>Distance</c> has no such dependency.
    /// </summary>
    [Fact]
    public void OccupancyCanBeMeasuredGeometrically()
    {
        var distance = GameApiContract.RequireMethod(
            typeof(UnityEngine.Collider2D), "Distance",
            "IsInsideAttack asks it whether a character overlaps an attack's hitbox.");

        Assert.Equal(typeof(UnityEngine.ColliderDistance2D), distance.ReturnType);

        var overlapped = typeof(UnityEngine.ColliderDistance2D)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => p.Name == "isOverlapped");

        Assert.True(overlapped != null, "ColliderDistance2D.isOverlapped is gone; the occupancy test has no verdict to read.");
        Assert.Equal(typeof(bool), overlapped.PropertyType);
    }

    /// <summary>
    /// The co-op camera lean is a Harmony postfix on
    /// <c>CameraController.UpdateTargetDestinationDelta</c> that writes the two delta fields the
    /// camera's destination is built from, injected by name as <c>___targetDeltaX</c> and
    /// <c>___targetDeltaY</c>. Harmony resolves those by reflection: if either is renamed the patch
    /// silently stops applying the lean instead of failing, which is precisely how this feature
    /// shipped inert once already.
    /// </summary>
    [Fact]
    public void TheCameraDeltaFieldsTheCoopLeanWritesStillExist()
    {
        var controller = GameApiContract.RequireType("CameraController");

        GameApiContract.RequireField(
            controller, "targetDeltaX", typeof(float),
            "Written by the co-op camera postfix; CameraController.LateUpdate builds its destination from it.");
        GameApiContract.RequireField(
            controller, "targetDeltaY", typeof(float),
            "Written by the co-op camera postfix; CameraController.LateUpdate builds its destination from it.");

        GameApiContract.RequireMethod(
            controller, "UpdateTargetDestinationDelta",
            "The co-op camera lean postfixes it, resolved by shape in CompanionCameraBias's patch class.");
    }

    /// <summary>
    /// The Shade lights dark rooms by cloning Hornet's hero light, because scene darkness is a
    /// shader cutout fed by a camera that renders that one object. If either accessor stops
    /// resolving there is nothing to clone and the Shade goes invisible in the dark again.
    /// </summary>
    [Fact]
    public void HornetsLightCanBeFoundToCloneFromIt()
    {
        GameApiContract.RequireField(
            typeof(HeroController), "heroLight", GameApiContract.RequireType("HeroLight"),
            "Cloned onto the Shade by EnsureShadeLight so it draws into the darkness cutout pass.");

        // The renderer is NOT on the HeroLight component's own object - a GetComponent there
        // returns null, which is how the Shade went a whole session with no light and one warning.
        GameApiContract.RequireField(
            GameApiContract.RequireType("HeroLight"), "spriteRenderer", typeof(UnityEngine.SpriteRenderer),
            "ResolveHeroLight reads it to find the sprite to clone and to sample its colour each frame.");
    }

    /// <summary>
    /// A gauntlet keeps its unspawned waves in the scene as active objects with live
    /// HealthManagers, so the Shade's target scan has to ask whether the battle is running - and
    /// nothing public answers that. If this stops resolving the scan reads every wave as live and
    /// the Shade goes back to slashing invisible enemies.
    /// </summary>
    [Fact]
    public void ARunningGauntletCanBeToldFromAWaitingOne()
    {
        GameApiContract.RequireField(
            typeof(BattleScene), "started", typeof(bool),
            "ShadeAiBattleScenes.HasStarted reads it to tell a live wave from one still queued.");
    }

    /// <summary>
    /// The Shade's slash is a clone of Hornet's, rebuilt through these private members. None of them
    /// is optional: a clone that keeps Hornet's activateOnSlash chain fires her other slashes, one
    /// that keeps her scale is drawn at the wrong size, and one whose travel component still points
    /// at her re-orients every time she turns around.
    /// </summary>
    [Theory]
    [InlineData(typeof(NailAttackBase), "activateOnSlash", typeof(UnityEngine.GameObject[]))]
    [InlineData(typeof(NailAttackBase), "hc", typeof(HeroController))]
    [InlineData(typeof(NailSlashTravel), "hc", typeof(HeroController))]
    [InlineData(typeof(NailSlashTravel), "initialLocalPos", typeof(UnityEngine.Vector3))]
    [InlineData(typeof(NailSlashTravel), "initialLocalScale", typeof(UnityEngine.Vector3))]
    [InlineData(typeof(NailSlashTravel), "travelDistance", typeof(UnityEngine.Vector2))]
    public void TheShadeSlashCanBeRebuiltFromHornets(Type owner, string field, Type fieldType)
    {
        GameApiContract.RequireField(
            owner, field, fieldType,
            "ConfigureSpawnedSlash and AdoptSlashAfterFrame rewrite it so the clone belongs to the "
            + "Shade rather than to Hornet.");
    }

    /// <summary>
    /// The menu check reads the backing field because <c>UIManager.instance</c> logs an error and
    /// scans the scene when nothing is registered, and the check runs every frame.
    /// </summary>
    [Fact]
    public void TheUiManagerCanBeReadWithoutTheLoggingAccessor()
    {
        var field = typeof(UIManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);

        Assert.True(field != null, "UIManager._instance is gone; MenuStateUtility would fall back to nothing and the Shade would act during menus.");
        Assert.Equal(typeof(UIManager), field!.FieldType);
    }

    /// <summary>
    /// <c>PlayerData.instance</c> builds a blank singleton when none exists, so the menu check asks
    /// <c>HasInstance</c> first rather than creating save data as a side effect of a read.
    /// </summary>
    [Fact]
    public void PlayerDataCanBeTestedForWithoutCreatingIt()
    {
        GameApiContract.RequireMethod(
            typeof(PlayerData), "get_HasInstance",
            "TryGetPlayerData asks it before touching instance, which would otherwise deserialize a new PlayerData.");
    }

    /// <summary>
    /// Read every frame to tell a scripted hold from one of Hornet's own moves. The public

    /// <c>GameCameras.instance</c> logs an error and <c>SilentInstance</c> falls back to a scene scan
    /// when nothing is registered, so the backing field is read directly instead.
    /// </summary>
    [Fact]
    public void TheCameraRigCanBeReadWithoutTheLoggingAccessor()
    {
        var field = typeof(GameCameras).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);

        Assert.True(field != null, "GameCameras._instance is gone; the scripted-hold check would read null every frame and the Shade would never notice a cutscene.");
        Assert.Equal(typeof(GameCameras), field!.FieldType);

        GameApiContract.RequireMethod(
            typeof(GameCameras), "get_IsHudVisible",
            "IsGameHudHidden reads it as the tiebreaker for whether the game took the moment away.");
    }

    /// <summary>
    /// NailAttackBase caches the slash's scale separately from the transform and reads it back
    /// mid-animation, so orienting the clone means writing both.
    /// </summary>

    [Theory]
    [InlineData("scale")]
    [InlineData("longNeedleScale")]
    public void TheSlashScaleCachesCanBeRewritten(string field)
    {
        GameApiContract.RequireField(
            typeof(NailAttackBase), field, typeof(UnityEngine.Vector3),
            "ApplyBaseSlashOrientation writes it so the slash keeps the Shade's size once the "
            + "animation reads the cached value back.");
    }

    /// <summary>
    /// The travel component subscribes itself to Hornet's flip event in Awake. The Shade's clone has
    /// to be taken back off it, and there is no public unsubscribe - the handler has to be rebuilt
    /// from the private method to be removed.
    /// </summary>
    [Fact]
    public void TheSlashCanBeUnsubscribedFromHornetsFlip()
    {
        var handler = GameApiContract.RequireMethod(
            typeof(NailSlashTravel), "OnHeroFlipped",
            "DetachHeroFlipHandler rebuilds this delegate to remove it from HeroController.FlippedSprite.");

        Assert.Empty(handler.GetParameters());
        Assert.Equal(typeof(void), handler.ReturnType);

        var flipped = typeof(HeroController).GetEvent("FlippedSprite");
        Assert.True(flipped != null, "HeroController.FlippedSprite is gone; the Shade's slash would keep re-orienting with Hornet.");
        Assert.Equal(typeof(Action), flipped!.EventHandlerType);
    }

    /// <summary>
    /// Every field the Shade's slash rewrites on a cloned damager to stop it counting as Hornet's
    /// nail. A lookup that stops resolving here leaves the Shade generating her silk, or dealing her
    /// damage figure instead of its own.
    /// </summary>
    [Theory]
    [InlineData("direction", typeof(float))]
    [InlineData("moveDirection", typeof(bool))]
    [InlineData("flipDirectionIfBehind", typeof(bool))]
    [InlineData("forwardVector", typeof(UnityEngine.Vector2))]
    [InlineData("ignoreNailPosition", typeof(bool))]
    [InlineData("doesNotGenerateSilk", typeof(bool))]
    [InlineData("useNailDamage", typeof(bool))]
    [InlineData("damageDealt", typeof(int))]
    public void AClonedDamagerCanBeRetargetedAtEnemies(string field, Type fieldType)
    {
        GameApiContract.RequireField(
            typeof(DamageEnemies), field, fieldType,
            "RetargetDamagers writes it so the Shade's slash damages enemies without reading as "
            + "one of Hornet's nail strikes.");
    }

    [Theory]
    [InlineData("TakeQuickDamage", false)]
    [InlineData("TakeDamage", true)]
    public void TheHeroDamageResolverFindsOnlyBindableMethods(string name, bool requireSource)

    {
        var methods = LegacyHelper.FindHeroDamageMethods(name, requireSource).ToList();

        Assert.NotEmpty(methods);
        foreach (var method in methods)
        {
            var names = method.GetParameters().Select(p => p.Name).ToList();
            Assert.Contains("damageAmount", names);
            if (requireSource)
            {
                Assert.Contains("go", names);
            }
        }
    }


    /// <summary>
    /// Every <c>[HarmonyPatch]</c> that names a method by name alone resolves to exactly one of
    /// them.
    /// <para>
    /// <c>AccessTools</c> resolves such a patch with <c>Type.GetMethod(name, flags)</c>, which
    /// throws <c>AmbiguousMatchException</c> when the game has more than one overload - and the
    /// shipped assembly carries overloads the decompiles do not. That throw took out every other
    /// patch in the mod once already, which is why patch classes are now applied one at a time. A
    /// name that resolves to nothing is the quieter half of the same problem: the patch binds
    /// nothing and the feature simply never runs.
    /// </para>
    /// <para>
    /// The rule this enforces is the one in AGENTS.md: an overloaded method is patched through a
    /// <c>TargetMethods()</c> that filters by parameter shape, or by naming
    /// <c>argumentTypes</c> - never by name alone.
    /// </para>
    /// </summary>
    [Fact]
    public void NoPatchNamesAMethodThatCannotBeResolved()
    {
        const BindingFlags anyMethod =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        var problems = new List<string>();
        int checkedNames = 0;

        Type[] types;
        try
        {
            types = typeof(LegacyHelper).Assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // The test host has no TextMeshPro beside it, so a handful of menu types will not load.
            // None of them carries a patch attribute; the count assertion below is what keeps that
            // from quietly becoming "checked nothing".
            types = ex.Types.Where(t => t != null).ToArray()!;
        }

        foreach (var type in types)
        {
            var attributes = new List<HarmonyPatch>(type.GetCustomAttributes<HarmonyPatch>(inherit: false));
            try
            {
                foreach (var method in type.GetMethods(anyMethod | BindingFlags.DeclaredOnly))
                {
                    attributes.AddRange(method.GetCustomAttributes<HarmonyPatch>(inherit: false));
                }
            }
            catch (FileNotFoundException)
            {
                // A signature naming a Unity module this host does not have. The class attribute
                // above is still readable, which is where every patch in this mod declares itself.
            }

            foreach (var attribute in attributes)
            {
                var info = attribute.info;
                if (info?.declaringType == null || string.IsNullOrEmpty(info.methodName))
                {
                    continue;
                }

                // A property accessor is named unambiguously, and an explicit argument list is
                // exactly the disambiguation this is asking for.
                if (info.methodType == MethodType.Getter || info.methodType == MethodType.Setter)
                {
                    continue;
                }

                if (info.argumentTypes != null && info.argumentTypes.Length > 0)
                {
                    continue;
                }

                checkedNames++;
                string where = $"{type.FullName} -> {info.declaringType.Name}.{info.methodName}";
                try
                {
                    if (info.declaringType.GetMethod(info.methodName, anyMethod) == null)
                    {
                        problems.Add($"{where} resolves to nothing, so the patch binds nothing.");
                    }
                }
                catch (AmbiguousMatchException)
                {
                    problems.Add(
                        $"{where} is overloaded, so resolving it throws and takes this patch class down. "
                        + "Use a TargetMethods() that filters by parameter shape, or name argumentTypes.");
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join("\n", problems));

        // A guard on the guard: if the scan above ever stops finding the patch classes, this test
        // would pass by checking nothing.
        Assert.True(
            checkedNames > 30,
            $"Only {checkedNames} patch target names were checked; the scan has stopped seeing them.");
    }

    /// <summary>
    /// The Shade's slash keeps its own travel state. A clone that inherits Hornet's mid-swing
    /// bookkeeping either never starts or never stops.
    /// </summary>
    [Theory]
    [InlineData("hasStarted")]
    [InlineData("isSlashActive")]
    public void TheClonedSlashTravelCanBeResetToItsStartingState(string field)
    {
        GameApiContract.RequireField(
            typeof(NailSlashTravel), field, typeof(bool),
            "ConfigureSpawnedSlash clears it so the clone begins its own travel rather than resuming Hornet's.");
    }

    /// <summary>
    /// The companion's slash must damage enemies without generating silk for Hornet or reading as
    /// one of her nail strikes. All three are set on the clone; a lookup that stops resolving takes
    /// its half of that away silently.
    /// </summary>
    [Fact]
    public void TheClonedDamagerCanBeStoppedFromFeedingHornetsSilk()
    {
        GameApiContract.RequireField(
            typeof(DamageEnemies), "onlyDamageEnemies", typeof(bool),
            "Read back to tell whether the clone still needs the setter run on it.");

        GameApiContract.RequireMethod(
            typeof(DamageEnemies), "setOnlyDamageEnemies",
            "The field has side effects the setter applies; writing it directly does half the job.",
            "onlyDamage");

        var silk = typeof(DamageEnemies).GetField(
            "silkGeneration",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        Assert.True(silk != null, "DamageEnemies.silkGeneration is gone; the companion's hits would refill Hornet's silk.");
        Assert.True(silk!.FieldType.IsEnum, $"DamageEnemies.silkGeneration is {silk.FieldType.Name}, which is not the enum the clone writes.");
    }

    /// <summary>
    /// Terrain hazards are told from enemy contact by this one field, and the two are handled
    /// completely differently - one teleports the companion to Hornet, the other knocks it back.
    /// </summary>
    [Fact]
    public void AHazardCanBeToldFromAnEnemy()
    {
        var field = typeof(DamageHero).GetField(
            "hazardType",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        Assert.True(field != null, "DamageHero.hazardType is gone; every hazard would be handled as ordinary enemy contact.");
        Assert.True(field!.FieldType.IsEnum, $"DamageHero.hazardType is {field.FieldType.Name}, which is not the enum ClassifyDamage reads.");
    }

    /// <summary>
    /// Both halves of the remask trigger are patched. Patching only one leaves the companion
    /// counted as having entered a region it never leaves.
    /// </summary>
    [Theory]
    [InlineData("OnTriggerEnter2D")]
    [InlineData("OnTriggerExit2D")]
    public void TheRemaskTriggerCanBeInterceptedBothWays(string name)
    {
        GameApiContract.RequireMethod(
            typeof(Remasker), name,
            "Remasker_ShadeProxy_Patch suppresses it for the companion's aggro proxy.",
            "collision");
    }

    /// <summary>
    /// The scene-entry audio trace patches these two overloads by shape. Naming an overloaded
    /// method in a HarmonyPatch attribute throws, so both are resolved by parameter list - and a
    /// resolution that fails disables the trace with only a log line to say so.
    /// </summary>
    [Fact]
    public void TheAudioCallsTheSceneEntryTraceWatchesStillExist()
    {
        Assert.True(
            AccessTools.Method(typeof(AudioSource), "PlayOneShot", new[] { typeof(AudioClip), typeof(float) }) != null,
            "AudioSource.PlayOneShot(AudioClip, float) is gone; the scene-entry audio trace records nothing.");

        Assert.True(
            AccessTools.Method(typeof(AudioSource), "Play", Type.EmptyTypes) != null,
            "AudioSource.Play() is gone; the scene-entry audio trace records nothing.");
    }

    /// <summary>
    /// Companion-aware trigger counting. <c>IsCounted</c> is virtual and the line-of-sight subclass
    /// overrides it, so both declarations are patched - patching the base alone leaves every
    /// line-of-sight range counting the companion as Hornet.
    /// </summary>
    [Theory]
    [InlineData(typeof(TrackTriggerObjects))]
    [InlineData(typeof(TrackTriggerObjectsLineOfSight))]
    public void TriggerCountingCanBeTaughtAboutTheCompanion(Type owner)
    {
        Assert.True(
            AccessTools.DeclaredMethod(owner, "IsCounted", new[] { typeof(GameObject) }) != null,
            $"{owner.Name} declares no IsCounted(GameObject); the companion would be counted as Hornet in its ranges.\n"
            + GameApiContract.DescribeMembers(owner));
    }

    /// <summary>
    /// The shade charm pane borrows the game's own pane input rather than reimplementing it, and
    /// switches these off while it owns the controls. A lookup that stops resolving hands the pane
    /// back to the game's navigation without saying so.
    /// </summary>
    [Theory]
    [InlineData("allowHorizontalSelection", typeof(bool))]
    [InlineData("allowVerticalSelection", typeof(bool))]
    [InlineData("allowRepeat", typeof(bool))]
    [InlineData("allowRepeatSubmit", typeof(bool))]
    [InlineData("allowRightStickSpeed", typeof(bool))]
    [InlineData("pane", typeof(InventoryPaneBase))]
    [InlineData("paneList", typeof(InventoryPaneList))]
    public void TheInventoryPaneInputCanBeTakenOverAndGivenBack(string field, Type fieldType)
    {
        GameApiContract.RequireField(
            typeof(InventoryPaneInput), field, fieldType,
            "ShadeInventoryPaneIntegration reads and restores it around the shade charm pane.");
    }

    /// <summary>
    /// The charm pane has to sit in the game's own pane list: name its tab, know how many panes are
    /// unlocked, and find its own index in the list.
    /// </summary>
    [Fact]
    public void TheInventoryPaneListCanBeJoined()
    {
        // Only that it is there: resolving its type loads Unity.TextMeshPro, which the test host
        // has no copy of.
        Assert.True(
            typeof(InventoryPaneList).GetField("currentPaneText", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null,
            "InventoryPaneList.currentPaneText is gone; the pane cannot write its own name into the list's header.");

        var unlocked = typeof(InventoryPaneList).GetProperty(
            "UnlockedPaneCount",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True(unlocked != null && unlocked.CanRead, "InventoryPaneList.UnlockedPaneCount is gone; the pane cannot tell whether it is reachable.");
        Assert.Equal(typeof(int), unlocked!.PropertyType);

        GameApiContract.RequireMethod(
            typeof(InventoryPaneList), "GetPaneIndex",
            "Resolves the shade pane's position so navigation can land on it.",
            "paneName");

        GameApiContract.RequireField(
            typeof(InventoryPaneList), "nextPaneOpen", typeof(string),
            "Names the pane the inventory will open on, which is how the shade pane is opened directly.");
    }

    /// <summary>
    /// A cloned menu row is inserted into the screen's own button list, which has to be told the
    /// list changed and which row was last on. Left unset, the highlight throws itself back to the
    /// screen's default row on every submit.
    /// </summary>
    [Theory]
    [InlineData("isTopLevelMenu", typeof(bool))]
    [InlineData("skipDisabled", typeof(bool))]
    [InlineData("isDirty", typeof(bool))]
    [InlineData("lastSelected", typeof(MenuSelectable))]
    public void ABorrowedMenuListCanBeReconfigured(string field, Type fieldType)
    {
        GameApiContract.RequireField(
            typeof(MenuButtonList), field, fieldType,
            "The shade screens rebuild the borrowed list around their own rows.");
    }

    /// <summary>
    /// A charm sold in a shop is a <c>ShopItem</c> built by hand, because nothing public constructs
    /// one. Every field here is written on the way out; one that stops resolving throws inside the
    /// build and the charm simply is not in the shop.
    /// </summary>
    [Theory]
    [InlineData("displayName", typeof(TeamCherry.Localization.LocalisedString))]
    [InlineData("description", typeof(TeamCherry.Localization.LocalisedString))]
    [InlineData("descriptionMultiple", typeof(TeamCherry.Localization.LocalisedString))]
    [InlineData("itemSprite", typeof(Sprite))]
    [InlineData("itemSpriteScale", typeof(float))]
    [InlineData("cost", typeof(int))]
    [InlineData("savedItem", typeof(SavedItem))]
    [InlineData("playerDataBoolName", typeof(string))]
    [InlineData("setExtraPlayerDataBools", typeof(string[]))]
    public void ACharmCanBePutOnAShopShelf(string field, Type fieldType)
    {
        GameApiContract.RequireField(
            typeof(ShopItem), field, fieldType,
            "ShopPlacementHandler builds the shop entry for a shade charm out of these.");
    }

    /// <summary>The currency a shop charges in, and the two display members around the item.</summary>
    [Fact]
    public void AShopChargesInACurrencyAndDrawsAnIcon()
    {
        var currency = typeof(ShopItem).GetField(
            "currencyType",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        Assert.True(currency != null, "ShopItem.currencyType is gone; a shade charm would be sold for the wrong currency.");
        Assert.True(currency!.FieldType.IsEnum, $"ShopItem.currencyType is {currency.FieldType.Name}, not the enum the handler writes.");

        GameApiContract.RequireField(
            typeof(ShopOwnerBase), "shopTitle", typeof(TeamCherry.Localization.LocalisedString),
            "Read to tell one shop owner from another when placing a charm.");

        GameApiContract.RequireField(
            typeof(SimpleShopItemDisplay), "itemIcon", typeof(SpriteRenderer),
            "The charm's own sprite is swapped onto it, and swapped back when the row is reused.");
    }

    /// <summary>
    /// A charm dropped by a boss is added to that enemy's own drop table, which is a private list of
    /// two private nested types. Nothing here is public, and a lookup that fails leaves the drop
    /// silently absent - which is the whole reason this file exists.
    /// </summary>
    [Fact]
    public void ACharmCanBeAddedToABossDropTable()
    {
        var groups = AccessTools.Field(typeof(HealthManager), "itemDropGroups");
        Assert.True(groups != null, "HealthManager.itemDropGroups is gone; boss-drop charms are never placed.");

        var groupType = AccessTools.Inner(typeof(HealthManager), "ItemDropGroup");
        var probabilityType = AccessTools.Inner(typeof(HealthManager), "ItemDropProbability");
        Assert.True(groupType != null, "HealthManager.ItemDropGroup is gone; boss-drop charms are never placed.");
        Assert.True(probabilityType != null, "HealthManager.ItemDropProbability is gone; boss-drop charms are never placed.");

        Assert.True(AccessTools.Field(groupType, "Drops") != null, $"ItemDropGroup has no Drops.\n{GameApiContract.DescribeMembers(groupType)}");
        Assert.True(AccessTools.Field(groupType, "TotalProbability") != null, $"ItemDropGroup has no TotalProbability.\n{GameApiContract.DescribeMembers(groupType)}");
        Assert.True(AccessTools.Field(probabilityType, "item") != null, $"ItemDropProbability has no item.\n{GameApiContract.DescribeMembers(probabilityType)}");

        // Inherited from Probability.ProbabilityBase<T>, which is why this is asked of AccessTools
        // rather than of the type directly.
        Assert.True(AccessTools.Field(probabilityType, "Probability") != null, $"ItemDropProbability has no Probability.\n{GameApiContract.DescribeMembers(probabilityType)}");
    }

    /// <summary>
    /// The shade charm pane is inserted into the game's own pane array and dressed like the panes
    /// beside it. These are all reached with <c>FieldRefAccess</c>, which throws out of a static
    /// initialiser rather than returning null - so a rename takes the whole integration with it at
    /// the first touch rather than at a point that names the field.
    /// </summary>
    [Theory]
    [InlineData(typeof(InventoryPaneList), "panes", typeof(InventoryPane[]))]
    [InlineData(typeof(InventoryPaneList), "paneListDisplay", typeof(InventoryPaneListDisplay))]
    [InlineData(typeof(InventoryPane), "listIcon", typeof(Sprite))]
    [InlineData(typeof(InventoryPane), "playerDataTest", typeof(PlayerDataTest))]
    [InlineData(typeof(InventoryPane), "hasNewPDBool", typeof(string))]
    public void TheShadePaneCanBeDressedLikeTheGamesOwn(Type owner, string field, Type fieldType)
    {
        GameApiContract.RequireField(
            owner, field, fieldType,
            "ShadeInventoryPaneIntegration writes it so the charm pane appears in the inventory like any other.");
    }

    /// <summary>
    /// Alert ranges are re-answered for the companion. The patch reads the range's own verdict and
    /// its line-of-sight mode, and needs the parent it was spawned under to tell whose range it is.
    /// </summary>
    [Theory]
    [InlineData("haveLineOfSight", typeof(bool))]
    [InlineData("isHeroInRange", typeof(bool))]
    [InlineData("initialParent", typeof(Transform))]
    public void AnAlertRangeCanBeReAnsweredForTheCompanion(string field, Type fieldType)
    {
        GameApiContract.RequireField(
            typeof(AlertRange), field, fieldType,
            "AlertRange_FixedUpdate_Patch rewrites the range's answer so the companion can be seen.");
    }

    /// <summary>The mode decides whether the range even asks about sight, so it is read before answering.</summary>
    [Fact]
    public void AnAlertRangesLineOfSightModeCanBeRead()
    {
        var field = typeof(AlertRange).GetField(
            "lineOfSight",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        Assert.True(field != null, "AlertRange.lineOfSight is gone; the companion would be seen through walls or not at all.");
        Assert.True(field!.FieldType.IsEnum, $"AlertRange.lineOfSight is {field.FieldType.Name}, not the enum the patch switches on.");
    }

    /// <summary>
    /// Which pane the inventory input is driving, read by the cancel trace so a menu bug report says
    /// which pane refused to close.
    /// </summary>
    [Fact]
    public void ThePaneTheInventoryInputIsDrivingCanBeNamed()
    {
        var field = typeof(InventoryPaneInput).GetField(
            "paneControl",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        Assert.True(field != null, "InventoryPaneInput.paneControl is gone; the cancel trace cannot say which pane it was on.");
        Assert.True(field!.FieldType.IsEnum, $"InventoryPaneInput.paneControl is {field.FieldType.Name}, not the pane-type enum.");
    }
}
