using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            "Read by DamageEnemies_Start_Mod to decide whether a damage object is Hornet's, and "
            + "whether it is a needle strike or a silk skill.");
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
    /// The Wanderer crest's slash prefabs are reachable only through these arrays - nothing public
    /// maps a Config back to the ConfigGroup holding its prefabs. If they stop resolving the shaman
    /// moveset silently falls back to the plain nail slash.
    /// </summary>
    [Theory]
    [InlineData("configs")]
    [InlineData("specialConfigs")]
    public void TheCrestSlashPrefabsCanBeFound(string field)
    {
        GameApiContract.RequireField(
            typeof(HeroController), field, typeof(HeroController.ConfigGroup[]),
            "FindShamanConfigGroup searches it for the ConfigGroup matching the spell crest's config.");
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
}
