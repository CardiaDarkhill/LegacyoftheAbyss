#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// The one place that knows what "terrain" means to the AI.
    /// <para>
    /// Two subsystems ask that question for different reasons - line of sight, so the Shade does not
    /// commit to an enemy behind a wall, and path sweeping, so it does not walk into one - and they
    /// have to agree. Splitting the layer lookup between them would make "it can see it but cannot
    /// reach it" a possible state for reasons that had nothing to do with the level.
    /// </para>
    /// <para>
    /// Triggers are excluded deliberately. Terrain colliders are solid; the trigger volumes in front
    /// of them are alert ranges, camera locks and vibration regions, and treating those as walls
    /// would blind the Shade in the middle of an open room.
    /// </para>
    /// </summary>
    internal static class ShadeAiTerrain
    {
        private static ContactFilter2D filter;
        private static bool resolved;
        private static bool available;

        // Shared buffers: these are called from Update on one Shade, never nested.
        private static readonly RaycastHit2D[] Hits = new RaycastHit2D[1];
        private static readonly Collider2D[] OverlapHits = new Collider2D[1];

        /// <summary>
        /// False when this build has no <c>Terrain</c> layer, in which case every query answers
        /// "clear". Said out loud once at startup rather than silently disabling both features - a
        /// Shade that cannot see walls and a Shade in a scene with no walls look identical.
        /// </summary>
        internal static bool Available
        {
            get
            {
                Ensure();
                return available;
            }
        }

        private static void Ensure()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            try
            {
                int layer = LayerMask.NameToLayer("Terrain");
                available = layer >= 0;
                filter = new ContactFilter2D
                {
                    useTriggers = false,
                    useLayerMask = available,
                    layerMask = available ? 1 << layer : ~0,
                    useDepth = false
                };

                if (!available)
                {
                    LegacyHelper.LogInfo(
                        "Shade AI: no \"Terrain\" layer in this build - line-of-sight and path checks are disabled.");
                }
            }
            catch
            {
                available = false;
            }
        }

        /// <summary>
        /// Whether a body of the given radius would be inside terrain standing at this point - i.e.
        /// whether the spot is somewhere the Shade could exist at all, rather than whether it can get
        /// there.
        /// </summary>
        internal static bool PointBlocked(Vector2 point, float radius)
        {
            Ensure();
            if (!available)
            {
                return false;
            }

            try
            {
                return Physics2D.OverlapCircle(point, Mathf.Max(0.05f, radius), filter, OverlapHits) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Whether solid terrain sits between two points.</summary>
        internal static bool LineBlocked(Vector2 from, Vector2 to)
        {
            Ensure();
            if (!available)
            {
                return false;
            }

            try
            {
                return Physics2D.Linecast(from, to, filter, Hits) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Whether a body of the given radius could travel along a heading without hitting terrain.
        /// <para>
        /// A sweep rather than a line, because the Shade is a 0.9x1.4 capsule and not a point: a gap
        /// a line slips through is not necessarily one the Shade fits in, and steering it into a slot
        /// narrower than its own body is how it ends up grinding along a wall.
        /// </para>
        /// </summary>
        internal static bool SweepBlocked(Vector2 origin, Vector2 direction, float distance, float radius)
        {
            Ensure();
            if (!available || distance <= 0f || direction.sqrMagnitude <= 0.0001f)
            {
                return false;
            }

            try
            {
                return Physics2D.CircleCast(origin, Mathf.Max(0.05f, radius), direction.normalized, filter, Hits, distance) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
