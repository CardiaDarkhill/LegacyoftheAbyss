#nullable disable
using System.Collections.Generic;
using UnityEngine;

internal static class ShadeAggroTracker
{
    internal readonly struct Target
    {
        public Target(LegacyHelper.ShadeController shade, LegacyHelper.ShadeController.AggroProxyTracker proxy, Vector2 position)
        {
            Shade = shade;
            Proxy = proxy;
            Position = position;
        }

        public LegacyHelper.ShadeController Shade { get; }
        public LegacyHelper.ShadeController.AggroProxyTracker Proxy { get; }
        public Vector2 Position { get; }
    }

    private static readonly Dictionary<AlertRange, HashSet<LegacyHelper.ShadeController.AggroProxyTracker>> RangeToProxies = new();
    private static readonly Dictionary<LegacyHelper.ShadeController.AggroProxyTracker, HashSet<AlertRange>> ProxyToRanges = new();
    private static readonly List<AlertRange> RangeRemovalBuffer = new();

    internal static void NotifyEntered(LegacyHelper.ShadeController.AggroProxyTracker proxy, TrackTriggerObjects enteredRange)
    {
        if (proxy == null || enteredRange == null)
        {
            return;
        }

        if (enteredRange is not AlertRange alertRange)
        {
            return;
        }

        if (!proxy.IsEligibleForAggro)
        {
            return;
        }

        Register(alertRange, proxy);

        if (ModConfig.Instance.logShade)
        {
            try
            {
                string owner = GetOwnerName(alertRange);
                LegacyHelper.Instance?.Logger?.LogInfo($"Shade aggro proxy entered alert range '{alertRange.name}' ({owner}).");
            }
            catch
            {
            }
        }
    }

    internal static void NotifyExited(LegacyHelper.ShadeController.AggroProxyTracker proxy, TrackTriggerObjects exitedRange)
    {
        if (proxy == null || exitedRange == null)
        {
            return;
        }

        if (exitedRange is AlertRange alertRange)
        {
            Remove(alertRange, proxy, log: true);
        }
    }

    internal static void NotifyDisabled(LegacyHelper.ShadeController.AggroProxyTracker proxy)
    {
        if (proxy == null)
        {
            return;
        }

        if (!ProxyToRanges.TryGetValue(proxy, out var ranges) || ranges.Count == 0)
        {
            ProxyToRanges.Remove(proxy);
            return;
        }

        RangeRemovalBuffer.Clear();
        RangeRemovalBuffer.AddRange(ranges);
        foreach (var range in RangeRemovalBuffer)
        {
            Remove(range, proxy, log: false);
        }
        ProxyToRanges.Remove(proxy);
    }

    internal static bool TryGetTargets(AlertRange range, List<Target> buffer)
    {
        buffer.Clear();
        if (range == null)
        {
            return false;
        }

        Cleanup(range);
        if (!RangeToProxies.TryGetValue(range, out var proxies) || proxies.Count == 0)
        {
            return false;
        }

        bool removedNullProxy = false;
        foreach (var proxy in proxies)
        {
            if (proxy == null)
            {
                removedNullProxy = true;
                continue;
            }

            if (!proxy.IsEligibleForAggro)
            {
                continue;
            }

            if (!proxy.TryGetOwner(out var shade) || shade == null || !shade.IsAggroEligible)
            {
                continue;
            }

            if (!proxy.TryGetTargetPoint(out var target))
            {
                continue;
            }

            buffer.Add(new Target(shade, proxy, target));
        }

        if (removedNullProxy)
        {
            proxies.RemoveWhere(p => p == null);
            if (proxies.Count == 0)
            {
                RangeToProxies.Remove(range);
                LegacyHelper.AlertRange_FixedUpdate_Patch.ResetLog(range);
            }
        }

        return buffer.Count > 0;
    }

    private static void Register(AlertRange range, LegacyHelper.ShadeController.AggroProxyTracker proxy)
    {
        Cleanup(range);
        if (!RangeToProxies.TryGetValue(range, out var proxies))
        {
            proxies = new HashSet<LegacyHelper.ShadeController.AggroProxyTracker>();
            RangeToProxies[range] = proxies;
        }
        proxies.Add(proxy);

        if (!ProxyToRanges.TryGetValue(proxy, out var ranges))
        {
            ranges = new HashSet<AlertRange>();
            ProxyToRanges[proxy] = ranges;
        }
        ranges.Add(range);
    }

    private static void Remove(AlertRange range, LegacyHelper.ShadeController.AggroProxyTracker proxy, bool log)
    {
        if (range == null || proxy == null)
        {
            return;
        }

        if (RangeToProxies.TryGetValue(range, out var proxies))
        {
            proxies.Remove(proxy);
            if (proxies.Count == 0)
            {
                RangeToProxies.Remove(range);
                LegacyHelper.AlertRange_FixedUpdate_Patch.ResetLog(range);
            }
        }

        if (ProxyToRanges.TryGetValue(proxy, out var ranges))
        {
            ranges.Remove(range);
            if (ranges.Count == 0)
            {
                ProxyToRanges.Remove(proxy);
            }
        }

        if (log && ModConfig.Instance.logShade)
        {
            try
            {
                string owner = GetOwnerName(range);
                LegacyHelper.Instance?.Logger?.LogInfo($"Shade aggro proxy exited alert range '{range.name}' ({owner}).");
            }
            catch
            {
            }
        }
    }

    private static void Cleanup(AlertRange range)
    {
        if (range == null)
        {
            return;
        }

        if (!RangeToProxies.TryGetValue(range, out var proxies) || proxies.Count == 0)
        {
            RangeToProxies.Remove(range);
            return;
        }

        proxies.RemoveWhere(p => p == null);
        if (proxies.Count == 0)
        {
            RangeToProxies.Remove(range);
            LegacyHelper.AlertRange_FixedUpdate_Patch.ResetLog(range);
        }
    }

    private static string GetOwnerName(AlertRange range)
    {
        if (range == null)
        {
            return "unknown";
        }

        try
        {
            var owner = range.transform;
            if (owner == null)
            {
                return "no transform";
            }

            var root = owner.root;
            return root != null ? root.name : owner.name;
        }
        catch
        {
            return "unknown";
        }
    }
}
#nullable restore
