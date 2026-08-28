#nullable disable
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Decides, per enemy, whether that enemy should be chasing/facing/shooting at the Shade instead of
/// Hornet.
/// <para>
/// This is the "who" half of Shade aggro; <see cref="LegacyHelper.EnemyAiRetargeting"/> is the
/// "how". Registration - an enemy noticing the Shade at all - already worked via
/// <see cref="ShadeAggroTracker"/> and the alert-range proxy; what was missing was target
/// <i>selection</i> once alerted.
/// </para>
/// <para>
/// Decisions are latched per enemy and only re-examined every <see cref="ReevaluateInterval"/>
/// seconds, for two reasons. It keeps the cost off the per-frame path (enemy AI actions call in from
/// <c>OnUpdate</c>), and more importantly it stops an enemy standing between Hornet and the Shade
/// from switching target every frame, which would leave it twitching on the spot instead of
/// committing to either. <see cref="PreferShade"/> adds hysteresis on top of that so a decision has
/// to be clearly wrong before it flips, rather than flipping the moment the two are equidistant.
/// </para>
/// </summary>
internal static class ShadeAggroTargeting
{
    /// <summary>How long a per-enemy decision stands before it is looked at again.</summary>
    private const float ReevaluateInterval = 0.75f;

    /// <summary>
    /// How much closer the Shade has to be before an enemy switches to it, as a fraction of the
    /// distance to Hornet - and how much further it is allowed to drift before the enemy switches
    /// back. Both directions use the same margin, so there is a dead band around "equidistant" where
    /// whatever was decided last simply stands.
    /// </summary>
    private const float SwitchMargin = 0.2f;

    /// <summary>
    /// Beyond this, the Shade is not a candidate at all. Without it an enemy could be pulled toward a
    /// Shade that is closer than Hornet only because Hornet has run a long way off - which reads as
    /// the enemy losing interest in the player entirely.
    /// </summary>
    private const float MaxShadeTargetDistance = 30f;

    private sealed class EnemyDecision
    {
        internal float NextEvaluationTime;
        internal bool TargetShade;
        internal bool HasDecided;
    }

    // Keyed weakly so entries die with their enemy; a plain dictionary would hold every enemy from
    // every scene alive for the session.
    private static readonly ConditionalWeakTable<GameObject, EnemyDecision> Decisions = new();

    /// <summary>
    /// The Shade's GameObject if it is currently a legitimate target, else null. Fast enough to call
    /// as the first check on a per-frame path: it is a static reference plus a handful of bools.
    /// </summary>
    internal static bool HasEligibleShade()
    {
        if (!ModConfig.Instance.shadeEnemyTargetingEnabled)
        {
            return false;
        }

        var instances = LegacyHelper.ShadeController.ActiveInstances;
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] != null && instances[i].IsAggroEligible)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The eligible Shade closest to <paramref name="enemy"/>, or null. With more than one Shade in
    /// the scene an enemy aims at the nearest rather than at whichever happened to spawn first.
    /// </summary>
    internal static GameObject GetShadeTargetFor(GameObject enemy)
    {
        if (!ModConfig.Instance.shadeEnemyTargetingEnabled)
        {
            return null;
        }

        var instances = LegacyHelper.ShadeController.ActiveInstances;
        bool haveOrigin = enemy != null;
        Vector2 origin = haveOrigin ? (Vector2)enemy.transform.position : Vector2.zero;

        GameObject best = null;
        float bestSqr = float.MaxValue;

        for (int i = 0; i < instances.Count; i++)
        {
            var shade = instances[i];
            if (shade == null || !shade.IsAggroEligible)
            {
                continue;
            }

            if (!haveOrigin)
            {
                return shade.gameObject;
            }

            float sqr = ((Vector2)shade.transform.position - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = shade.gameObject;
            }
        }

        return best;
    }

    /// <summary>
    /// Whether <paramref name="enemy"/> should currently be aiming at the Shade rather than at
    /// Hornet. <paramref name="shadeObject"/> is the value from <see cref="GetShadeTarget"/>.
    /// </summary>
    internal static bool ShouldTargetShade(GameObject enemy, GameObject shadeObject)
    {
        if (enemy == null || shadeObject == null)
        {
            return false;
        }

        try
        {
            var hero = HeroController.UnsafeInstance;
            if (hero == null)
            {
                // Nothing to compare against: leave whatever the FSM already had alone rather than
                // inventing a preference.
                return false;
            }

            var decision = Decisions.GetOrCreateValue(enemy);
            float now = Time.time;
            if (decision.HasDecided && now < decision.NextEvaluationTime)
            {
                return decision.TargetShade;
            }

            Vector2 enemyPosition = enemy.transform.position;
            float hornetDistance = Vector2.Distance(enemyPosition, hero.transform.position);
            float shadeDistance = Vector2.Distance(enemyPosition, shadeObject.transform.position);

            bool preferShade = shadeDistance <= MaxShadeTargetDistance &&
                PreferShade(hornetDistance, shadeDistance, decision.HasDecided && decision.TargetShade, SwitchMargin);

            if (ModConfig.Instance.logShade && (!decision.HasDecided || preferShade != decision.TargetShade))
            {
                try
                {
                    LegacyHelper.LogInfo(System.FormattableString.Invariant(
                        $"Shade aggro: '{enemy.name}' now targeting {(preferShade ? "the Shade" : "Hornet")} (hornet={hornetDistance:0.0}, shade={shadeDistance:0.0})"));
                }
                catch
                {
                }
            }

            decision.TargetShade = preferShade;
            decision.HasDecided = true;
            decision.NextEvaluationTime = now + ReevaluateInterval;
            return preferShade;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// The comparison itself, split out with no Unity types so it can be exercised directly.
    /// <para>
    /// Asymmetric on purpose: taking the Shade as a target requires it to be <paramref name="switchMargin"/>
    /// closer than Hornet, while keeping it only requires it not to be that much further away. An
    /// enemy therefore commits to a target and stays committed through the wobble of two moving
    /// objects, instead of alternating every time the distances cross.
    /// </para>
    /// </summary>
    internal static bool PreferShade(float hornetDistance, float shadeDistance, bool currentlyTargetingShade, float switchMargin)
    {
        if (currentlyTargetingShade)
        {
            return shadeDistance <= hornetDistance * (1f + switchMargin);
        }

        return shadeDistance < hornetDistance * (1f - switchMargin);
    }
}
