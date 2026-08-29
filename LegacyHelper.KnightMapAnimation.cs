#nullable disable
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    /// <summary>
    /// Has the Knight take out its own map when the quick map is opened.
    /// <para>
    /// Hornet is left entirely alone: she keeps her own animation set, and nothing here touches
    /// her. This only gives the second player the matching pose, which the bundle already carries.
    /// </para>
    /// </summary>
    internal static class KnightMapAnimation
    {
        internal static void SetOpen(bool open)
        {
            foreach (var companion in ShadeCompanionRegistry.All)
            {
                companion.Controller?.SetKnightMapOpen(open);
            }
        }
    }

    [HarmonyPatch]
    private class GameMap_TryOpenQuickMap_KnightMap
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var method in ResolveByName(typeof(GameMap), "TryOpenQuickMap"))
            {
                yield return method;
            }
        }

        private static void Postfix(bool __result)
        {
            if (__result)
            {
                KnightMapAnimation.SetOpen(true);
            }
        }
    }

    [HarmonyPatch]
    private class GameMap_CloseQuickMap_KnightMap
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var method in ResolveByName(typeof(GameMap), "CloseQuickMap"))
            {
                yield return method;
            }
        }

        private static void Postfix() => KnightMapAnimation.SetOpen(false);
    }

    /// <summary>
    /// One method of a given name, or nothing. Resolved by shape rather than named through the
    /// attribute, because <c>AccessTools</c> throws on an overload and an unrecognised assembly
    /// should cost one feature rather than every patch in the mod.
    /// </summary>
    private static IEnumerable<MethodBase> ResolveByName(System.Type owner, string name)
    {
        var candidates = new List<MethodBase>();
        foreach (var method in owner.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            if (method.Name == name)
            {
                candidates.Add(method);
            }
        }

        if (candidates.Count != 1)
        {
            LogWarning($"Knight map animation disabled: {owner.Name}.{name} resolved to {candidates.Count} methods.");
            yield break;
        }

        yield return candidates[0];
    }
}
