#nullable enable

using System.Reflection;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// What the mod needs to know about the game's gauntlet scenes.
    /// </summary>
    internal static class ShadeAiBattleScenes
    {
        // BattleScene.started is private and there is no public equivalent.
        private static readonly FieldInfo? StartedField =
            typeof(BattleScene).GetField("started", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>Whether a gauntlet is running - not merely present in the scene.</summary>
        internal static bool HasStarted(BattleScene? scene)
        {
            return scene != null && StartedField != null && (bool)StartedField.GetValue(scene);
        }

        /// <summary>
        /// The gauntlet wave an enemy belongs to, if any. Returns false for everything else, which
        /// is most enemies.
        /// </summary>
        internal static bool TryResolveWave(Transform transform, out BattleScene scene, out int waveIndex)
        {
            scene = null!;
            waveIndex = -1;

            var wave = transform.GetComponentInParent<BattleWave>();
            if (wave == null)
            {
                return false;
            }

            var owner = wave.GetComponentInParent<BattleScene>();
            if (owner == null || owner.waves == null)
            {
                return false;
            }

            int index = owner.waves.IndexOf(wave);
            if (index < 0)
            {
                return false;
            }

            scene = owner;
            waveIndex = index;
            return true;
        }

        /// <summary>
        /// Whether a wave's enemies have been switched on.
        /// <para>
        /// A wave that has not started still has its enemies sitting in the scene as active
        /// GameObjects with live HealthManagers - <see cref="BattleSceneEnemy.SetActive"/> only
        /// disables their FSMs. They are invisible and inert, and to anything that finds enemies by
        /// walking HealthManagers they look like ordinary targets.
        /// </para>
        /// </summary>
        internal static bool IsWaveLive(BattleScene scene, int waveIndex)
        {
            return HasStarted(scene) && waveIndex <= scene.currentWave;
        }
    }
}
