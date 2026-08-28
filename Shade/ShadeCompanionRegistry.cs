#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Every companion currently modelled, keyed by slot id. The primary companion always exists so
    /// that <see cref="ShadeRuntime"/>'s static API always has something to delegate to.
    /// </summary>
    internal static class ShadeCompanionRegistry
    {
        internal const int PrimaryId = 0;

        private static readonly Dictionary<int, ShadeCompanion> s_companions = new();
        private static readonly List<ShadeCompanion> s_ordered = new();

        static ShadeCompanionRegistry()
        {
            Add(new ShadeCompanion(PrimaryId, ShadeCharacterRegistry.Default.Id));
        }

        internal static event Action<ShadeCompanion>? CompanionAdded;
        internal static event Action<ShadeCompanion>? CompanionRemoved;

        internal static ShadeCompanion Primary => s_companions[PrimaryId];

        /// <summary>Companions in slot order. Safe to enumerate while spawning: it is a snapshot.</summary>
        internal static IReadOnlyList<ShadeCompanion> All => s_ordered.ToArray();

        internal static int Count => s_ordered.Count;

        internal static bool TryGet(int id, out ShadeCompanion companion)
            => s_companions.TryGetValue(id, out companion!);

        internal static ShadeCompanion GetOrCreate(int id, ShadeCharacterId? character = null)
        {
            if (s_companions.TryGetValue(id, out var existing))
            {
                return existing;
            }

            var created = new ShadeCompanion(id, character ?? ShadeCharacterRegistry.Default.Id);
            Add(created);
            CompanionAdded?.Invoke(created);
            return created;
        }

        /// <summary>Allocates the lowest free slot above the primary.</summary>
        internal static ShadeCompanion CreateNext(ShadeCharacterId? character = null)
        {
            int id = PrimaryId;
            while (s_companions.ContainsKey(id))
            {
                id++;
            }

            return GetOrCreate(id, character);
        }

        /// <summary>Removes a companion. The primary cannot be removed; it is reset instead.</summary>
        internal static bool Remove(int id)
        {
            if (id == PrimaryId || !s_companions.TryGetValue(id, out var companion))
            {
                return false;
            }

            s_companions.Remove(id);
            s_ordered.Remove(companion);
            CompanionRemoved?.Invoke(companion);
            return true;
        }

        /// <summary>Drops every secondary companion and resets the primary's state.</summary>
        internal static void Clear()
        {
            foreach (var companion in s_ordered.Where(c => !c.IsPrimary).ToArray())
            {
                Remove(companion.Id);
            }

            Primary.Reset();
        }

        /// <summary>The companion driven by the given controller, or null if it is not registered.</summary>
        internal static ShadeCompanion? FromController(LegacyHelper.ShadeController? controller)
            => controller == null ? null : s_ordered.FirstOrDefault(c => ReferenceEquals(c.Controller, controller));

        private static void Add(ShadeCompanion companion)
        {
            s_companions[companion.Id] = companion;
            s_ordered.Add(companion);
            s_ordered.Sort((a, b) => a.Id.CompareTo(b.Id));
        }
    }
}
