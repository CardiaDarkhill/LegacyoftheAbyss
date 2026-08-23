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

        bool refusable = fields.Any(f =>
            f.FieldType == typeof(HutongGames.PlayMaker.FsmEvent) &&
            f.Name.IndexOf("cannot", StringComparison.OrdinalIgnoreCase) >= 0);

        if (!refusable)
        {
            Assert.Fail($"{name} has no 'cannot' branch; refusing it would strand the FSM.\n{GameApiContract.DescribeMembers(type)}");
        }

        Assert.NotNull(type.GetMethod("OnEnter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
    }

    [Fact]
    public void HeroDamageEntryPointsBindTheirPrefixes()
    {
        GameApiContract.RequireMethod(
            typeof(HeroController), "TakeQuickDamage",
            "Patched to spare Hornet a hit she is not standing in.", "damageAmount");

        GameApiContract.RequireMethod(
            typeof(HeroController), "TakeDamage",
            "Patched to spare Hornet a hit she is not standing in.", "go", "damageAmount");
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
    /// How the mod finds the hero-damage entry points it patches. Naming them through a
    /// <c>[HarmonyPatch]</c> attribute threw <c>AmbiguousMatchException</c> out of <c>PatchAll</c> and
    /// took the whole mod down, so they are resolved by parameter shape instead.
    /// </summary>
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
