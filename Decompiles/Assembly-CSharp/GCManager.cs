using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

// Token: 0x02000766 RID: 1894
public class GCManager : MonoBehaviour
{
	// Token: 0x140000E3 RID: 227
	// (add) Token: 0x0600437F RID: 17279 RVA: 0x00128E90 File Offset: 0x00127090
	// (remove) Token: 0x06004380 RID: 17280 RVA: 0x00128EC4 File Offset: 0x001270C4
	private static event Action OnGCStutter;

	// Token: 0x17000794 RID: 1940
	// (get) Token: 0x06004381 RID: 17281 RVA: 0x00128EF7 File Offset: 0x001270F7
	// (set) Token: 0x06004382 RID: 17282 RVA: 0x00128EFE File Offset: 0x001270FE
	public static bool DisabledManualCollect { get; set; }

	// Token: 0x17000795 RID: 1941
	// (get) Token: 0x06004383 RID: 17283 RVA: 0x00128F06 File Offset: 0x00127106
	// (set) Token: 0x06004384 RID: 17284 RVA: 0x00128F0D File Offset: 0x0012710D
	public static double HeapUsageThreshold { get; private set; }

	// Token: 0x06004385 RID: 17285 RVA: 0x00128F18 File Offset: 0x00127118
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		double num = 0.12102111566341002 * (double)SystemInfo.systemMemorySize;
		if (num < 384.0)
		{
			num = 384.0;
		}
		else if (num > 1024.0)
		{
			num = 1024.0;
		}
		GCManager.HeapUsageThreshold = num;
		if (Application.isEditor)
		{
			return;
		}
		if (GarbageCollector.isIncremental)
		{
			return;
		}
		GameObject gameObject = new GameObject("GCManager", new Type[]
		{
			typeof(GCManager)
		});
		gameObject.hideFlags |= HideFlags.HideAndDontSave;
		Object.DontDestroyOnLoad(gameObject);
	}

	// Token: 0x06004386 RID: 17286 RVA: 0x00128FAB File Offset: 0x001271AB
	private void Awake()
	{
		GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
	}

	// Token: 0x06004387 RID: 17287 RVA: 0x00128FB3 File Offset: 0x001271B3
	private void OnDestroy()
	{
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
	}

	// Token: 0x06004388 RID: 17288 RVA: 0x00128FBC File Offset: 0x001271BC
	private void Update()
	{
		if (GarbageCollector.GCMode == GarbageCollector.Mode.Enabled)
		{
			return;
		}
		if (Time.realtimeSinceStartupAsDouble - GCManager._lastGCTime < 1.0)
		{
			return;
		}
		double num = (double)GCManager.GetMonoHeapUsage() / 1024.0 / 1024.0;
		if (num > GCManager.HeapUsageThreshold)
		{
			if (this.pauseGCAttempts)
			{
				return;
			}
			GCManager.ForceCollect(true, false);
			GCManager.HandleGCStutter();
			if (num > GCManager.HeapUsageThreshold)
			{
				this.pauseGCAttempts = true;
				return;
			}
		}
		else
		{
			this.pauseGCAttempts = false;
		}
	}

	// Token: 0x06004389 RID: 17289 RVA: 0x00129038 File Offset: 0x00127238
	public static void Collect()
	{
		if (GCManager.DisabledManualCollect)
		{
			return;
		}
		GCManager.ForceCollect(true, false);
	}

	// Token: 0x0600438A RID: 17290 RVA: 0x00129049 File Offset: 0x00127249
	public static void ForceCollect(bool blocking = true, bool compacting = false)
	{
		GarbageCollector.Mode gcmode = GarbageCollector.GCMode;
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking, compacting);
		GarbageCollector.GCMode = gcmode;
		GCManager._lastGCTime = Time.realtimeSinceStartupAsDouble;
	}

	// Token: 0x0600438B RID: 17291 RVA: 0x00129072 File Offset: 0x00127272
	public static long GetMemoryUsage()
	{
		return GCManager.GetMemoryTotal() - Profiler.GetTotalUnusedReservedMemoryLong();
	}

	// Token: 0x0600438C RID: 17292 RVA: 0x0012907F File Offset: 0x0012727F
	public static long GetMemoryTotal()
	{
		return Profiler.GetTotalReservedMemoryLong();
	}

	// Token: 0x0600438D RID: 17293 RVA: 0x00129086 File Offset: 0x00127286
	public static long GetMonoHeapUsage()
	{
		return Profiler.GetMonoUsedSizeLong();
	}

	// Token: 0x0600438E RID: 17294 RVA: 0x0012908D File Offset: 0x0012728D
	public static long GetMonoHeapTotal()
	{
		return Profiler.GetMonoHeapSizeLong();
	}

	// Token: 0x0600438F RID: 17295 RVA: 0x00129094 File Offset: 0x00127294
	private static void HandleGCStutter()
	{
		GCManager._lastAutoGCTimes.Add(Time.realtimeSinceStartupAsDouble);
		while (GCManager._lastAutoGCTimes.Count > 5)
		{
			GCManager._lastAutoGCTimes.RemoveAt(0);
		}
		if (GCManager.IsGCStutterDetected())
		{
			float num = 0.1f;
			GCManager.HeapUsageThreshold *= (double)(1f + num);
			Debug.LogError(string.Format("GC stutter was detected! Heap size was increased by {0}%. Current heap threshold is: {1}MB", (double)num * 100.0, GCManager.HeapUsageThreshold));
			Action onGCStutter = GCManager.OnGCStutter;
			if (onGCStutter != null)
			{
				onGCStutter();
			}
			GCManager._lastAutoGCTimes.Clear();
		}
	}

	// Token: 0x06004390 RID: 17296 RVA: 0x00129130 File Offset: 0x00127330
	private static bool IsGCStutterDetected()
	{
		if (GCManager._lastAutoGCTimes.Count < 5)
		{
			return false;
		}
		double num = 0.0;
		for (int i = 0; i < GCManager._lastAutoGCTimes.Count - 1; i++)
		{
			num += Math.Abs(GCManager._lastAutoGCTimes[i] - GCManager._lastAutoGCTimes[i + 1]);
		}
		return num / (double)(GCManager._lastAutoGCTimes.Count - 1) < 120.0;
	}

	// Token: 0x06004392 RID: 17298 RVA: 0x001291B0 File Offset: 0x001273B0
	// Note: this type is marked as 'beforefieldinit'.
	static GCManager()
	{
		GCManager.OnGCStutter = null;
	}

	// Token: 0x04004518 RID: 17688
	private bool pauseGCAttempts;

	// Token: 0x04004519 RID: 17689
	private static double _lastGCTime;

	// Token: 0x0400451A RID: 17690
	private const int CachedAutoGCTimes = 5;

	// Token: 0x0400451B RID: 17691
	private const double StutterThresholdTime = 120.0;

	// Token: 0x0400451C RID: 17692
	private static List<double> _lastAutoGCTimes = new List<double>();
}
