#nullable disable
using System;
using System.Collections.Generic;
using UnityEngine;

// The trigger proxy that makes enemies notice the Shade. It stands in for Hornet inside AlertRange
// and TrackTriggerObjects volumes; ShadeAggroTracker decides what the enemy does about it.
public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        internal sealed class AggroProxyTracker : MonoBehaviour, ITrackTriggerObject
        {
            private ShadeController owner;
            private Collider2D proxyCollider;
            private readonly HashSet<Remasker> remaskersInside = new HashSet<Remasker>();
            private static readonly List<Remasker> RemaskerBuffer = new List<Remasker>();

            /// <summary>How long the same collider is ignored for after being recorded once.</summary>
            private const float ProxyEntryThrottleSeconds = 1f;

            /// <summary>Ancestors named in a recorded path. Enough to identify the boss, not the whole scene.</summary>
            private const int ProxyEntryPathDepth = 2;

            /// <summary>
            /// When each collider was last recorded, so the same hitbox does not fill the event ring.
            /// <para>
            /// Pruned rather than kept: the companion's body survives room changes, so this would
            /// otherwise hold one entry - and one live reference to a by-then destroyed collider -
            /// for every hitbox and detection range that had ever touched the proxy in the session.
            /// </para>
            /// </summary>
            private readonly Dictionary<Collider2D, float> _lastProxyEntryTimes = new Dictionary<Collider2D, float>();

            /// <summary>How many entries to carry before dropping the ones that can no longer suppress anything.</summary>
            private const int ProxyEntryMemoryCap = 64;

            private static readonly List<Collider2D> ProxyEntryPruneBuffer = new List<Collider2D>();

            internal void Attach(ShadeController shade, Collider2D collider)
            {
                owner = shade;
                proxyCollider = collider;
                remaskersInside.Clear();
                _lastProxyEntryTimes.Clear();
            }

            internal bool IsEligibleForAggro => owner != null && owner.IsAggroEligible;

            internal bool TryGetOwner(out ShadeController shade)
            {
                shade = owner;
                return shade != null;
            }

            internal bool TryGetTargetPoint(out Vector2 target)
            {
                target = transform.position;
                if (!IsEligibleForAggro)
                {
                    return false;
                }

                if (!proxyCollider || !proxyCollider.enabled || !proxyCollider.gameObject.activeInHierarchy)
                {
                    return false;
                }

                try
                {
                    target = proxyCollider.bounds.center;
                }
                catch
                {
                    target = transform.position;
                }

                return true;
            }

            public void OnTrackTriggerEntered(TrackTriggerObjects enteredRange)
            {
                ShadeAggroTracker.NotifyEntered(this, enteredRange);
            }

            public void OnTrackTriggerExited(TrackTriggerObjects exitedRange)
            {
                ShadeAggroTracker.NotifyExited(this, exitedRange);
            }

            private void OnDisable()
            {
                ForceExitTrackedRemaskers();
                ShadeAggroTracker.NotifyDisabled(this);
            }

            private void OnDestroy()
            {
                ForceExitTrackedRemaskers();
                ShadeAggroTracker.NotifyDisabled(this);
            }

            private void OnTriggerEnter2D(Collider2D other)
            {
                RecordProxyEntry(other);
                TrackRemasker(other, entering: true);
            }

            /// <summary>
            /// Notes every trigger the proxy walks into, for the bug report event ring.
            /// <para>
            /// The proxy exists to look exactly like Hornet to enemy detection, which means it also
            /// looks like Hornet to anything else that tests for her - including boss attacks that,
            /// once triggered, go on to act on <c>HeroController.instance</c> rather than on whatever
            /// actually tripped them. When that happens the visible symptom lands on Hornet and no
            /// artefact in a report names the object responsible. This is the line that names it.
            /// </para>
            /// <para>
            /// Enters only, and throttled per object: exits are not what starts an attack, and a
            /// region the Shade is hovering in and out of would otherwise flush the ring.
            /// </para>
            /// </summary>
            private void RecordProxyEntry(Collider2D other)
            {
                if (!other)
                {
                    return;
                }

                try
                {
                    float now = Time.realtimeSinceStartup;
                    if (_lastProxyEntryTimes.TryGetValue(other, out float previous) &&
                        now - previous < ProxyEntryThrottleSeconds)
                    {
                        return;
                    }

                    _lastProxyEntryTimes[other] = now;
                    if (_lastProxyEntryTimes.Count > ProxyEntryMemoryCap)
                    {
                        PruneProxyEntryTimes(now);
                    }

                    // Self and ancestor are reported separately, and the distinction matters: an
                    // ancestor DamageHero is just "this belongs to something that can hurt you",
                    // which is true of every collider on an enemy including its harmless detection
                    // ranges. Only a DamageHero on the collider's own object means "this collider is
                    // the thing that hurts". Reporting the two as one flag made an attack hitbox and
                    // a battle range look identical in the first report that used this.
                    bool ownFsm = other.GetComponent<PlayMakerFSM>() != null;
                    bool ownDamageHero = other.GetComponent<DamageHero>() != null;
                    bool parentFsm = !ownFsm && other.GetComponentInParent<PlayMakerFSM>() != null;
                    bool parentDamageHero = !ownDamageHero && other.GetComponentInParent<DamageHero>() != null;

                    LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                        "shade-proxy-entered",
                        LegacyHelper.DescribeHierarchy(other.transform, ProxyEntryPathDepth),
                        FormattableString.Invariant(
                            $"layer={LayerMask.LayerToName(other.gameObject.layer)} tag={other.gameObject.tag} trigger={other.isTrigger} fsm={(ownFsm ? "self" : parentFsm ? "parent" : "none")} damageHero={(ownDamageHero ? "self" : parentDamageHero ? "parent" : "none")}"));
                }
                catch
                {
                }
            }

            /// <summary>
            /// Drops every entry that can no longer suppress a record: one older than the throttle,
            /// or one whose collider has been destroyed.
            /// </summary>
            private void PruneProxyEntryTimes(float now)
            {
                ProxyEntryPruneBuffer.Clear();
                foreach (var entry in _lastProxyEntryTimes)
                {
                    if (!entry.Key || now - entry.Value >= ProxyEntryThrottleSeconds)
                    {
                        ProxyEntryPruneBuffer.Add(entry.Key);
                    }
                }

                foreach (var stale in ProxyEntryPruneBuffer)
                {
                    _lastProxyEntryTimes.Remove(stale);
                }

                ProxyEntryPruneBuffer.Clear();
            }

            private void TrackRemasker(Collider2D other, bool entering)
            {
                if (!other)
                {
                    return;
                }

                var remasker = other.GetComponent<Remasker>();
                if (!remasker)
                {
                    remasker = other.GetComponentInParent<Remasker>();
                }

                if (!remasker)
                {
                    return;
                }

                if (entering)
                {
                    remaskersInside.Add(remasker);
                }
                else
                {
                    remaskersInside.Remove(remasker);
                }
            }

            internal void ForceExitTrackedRemaskers()
            {
                if (remaskersInside.Count == 0)
                {
                    return;
                }

                RemaskerBuffer.Clear();
                RemaskerBuffer.AddRange(remaskersInside);
                foreach (var remasker in RemaskerBuffer)
                {
                    if (!remasker)
                    {
                        continue;
                    }

                    try
                    {
                        remasker.Exited(true);
                    }
                    catch
                    {
                    }
                }

                RemaskerBuffer.Clear();
                remaskersInside.Clear();
            }

            internal void NotifyRemaskerIgnored(Remasker remasker)
            {
                if (!remasker)
                {
                    return;
                }

                remaskersInside.Remove(remasker);
            }
        }
    }
}
#nullable restore
