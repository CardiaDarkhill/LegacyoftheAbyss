using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000795 RID: 1941
public static class TweenExtensions
{
	// Token: 0x060044A2 RID: 17570 RVA: 0x0012C834 File Offset: 0x0012AA34
	public static void ScaleTo(this Transform transform, MonoBehaviour runner, Vector3 localScale, float duration, float delay = 0f, bool dontTrack = false, bool isRealtime = false, Action onEnd = null)
	{
		TweenExtensions.<>c__DisplayClass1_0 CS$<>8__locals1 = new TweenExtensions.<>c__DisplayClass1_0();
		CS$<>8__locals1.transform = transform;
		CS$<>8__locals1.localScale = localScale;
		CS$<>8__locals1.onEnd = onEnd;
		CS$<>8__locals1.dontTrack = dontTrack;
		if (!runner)
		{
			Debug.LogError("No runner provided for ScaleTo");
			return;
		}
		Coroutine routine;
		if (!CS$<>8__locals1.dontTrack && TweenExtensions._scaleRoutines.TryGetValue(CS$<>8__locals1.transform, out routine))
		{
			runner.StopCoroutine(routine);
			TweenExtensions._scaleRoutines.Remove(CS$<>8__locals1.transform);
		}
		if (!CS$<>8__locals1.transform)
		{
			Debug.LogError("No transform provided for ScaleTo");
			return;
		}
		if (duration <= 0f || !CS$<>8__locals1.transform.gameObject.activeInHierarchy)
		{
			CS$<>8__locals1.transform.localScale = CS$<>8__locals1.localScale;
			return;
		}
		TweenExtensions.<>c__DisplayClass1_1 CS$<>8__locals2 = new TweenExtensions.<>c__DisplayClass1_1();
		CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
		CS$<>8__locals2.initialScale = CS$<>8__locals2.CS$<>8__locals1.transform.localScale;
		Coroutine value = runner.StartTimerRoutine(delay, duration, new Action<float>(CS$<>8__locals2.<ScaleTo>g__Handler|0), null, delegate
		{
			Action onEnd2 = CS$<>8__locals2.CS$<>8__locals1.onEnd;
			if (onEnd2 != null)
			{
				onEnd2();
			}
			if (!CS$<>8__locals2.CS$<>8__locals1.dontTrack)
			{
				TweenExtensions._scaleRoutines.Remove(CS$<>8__locals2.CS$<>8__locals1.transform);
			}
		}, isRealtime);
		if (!CS$<>8__locals2.CS$<>8__locals1.dontTrack)
		{
			TweenExtensions._scaleRoutines[CS$<>8__locals2.CS$<>8__locals1.transform] = value;
		}
	}

	// Token: 0x060044A3 RID: 17571 RVA: 0x0012C960 File Offset: 0x0012AB60
	public static void ClenaupInactiveCoroutines()
	{
		if (TweenExtensions._scaleRoutines.Count == 0)
		{
			return;
		}
		Transform[] array = (from transform in TweenExtensions._scaleRoutines.Keys
		where transform == null
		select transform).ToArray<Transform>();
		if (array.Length != 0)
		{
			Debug.LogWarning(string.Format("Inactive coroutines were cleared: {0}", array.Length));
		}
		foreach (Transform key in array)
		{
			TweenExtensions._scaleRoutines.Remove(key);
		}
	}

	// Token: 0x040045A1 RID: 17825
	private static readonly Dictionary<Transform, Coroutine> _scaleRoutines = new Dictionary<Transform, Coroutine>();
}
