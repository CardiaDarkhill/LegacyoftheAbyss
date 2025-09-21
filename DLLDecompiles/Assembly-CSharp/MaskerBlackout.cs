using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200064B RID: 1611
public class MaskerBlackout : MonoBehaviour
{
	// Token: 0x1700068C RID: 1676
	// (get) Token: 0x060039AE RID: 14766 RVA: 0x000FD210 File Offset: 0x000FB410
	public static bool IsAnyFading
	{
		get
		{
			using (HashSet<MaskerBlackout>.Enumerator enumerator = MaskerBlackout._activeBlackouts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.fadeRoutine != null)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x060039AF RID: 14767 RVA: 0x000FD268 File Offset: 0x000FB468
	private void OnEnable()
	{
		this.spriteRenderer.enabled = true;
		MaskerBlackout._activeBlackouts.Add(this);
	}

	// Token: 0x060039B0 RID: 14768 RVA: 0x000FD282 File Offset: 0x000FB482
	private void OnDisable()
	{
		MaskerBlackout._activeBlackouts.Remove(this);
	}

	// Token: 0x060039B1 RID: 14769 RVA: 0x000FD290 File Offset: 0x000FB490
	public static bool AddInside(Object blackoutMask, float fadeInTime)
	{
		if (MaskerBlackout._insideMasks.Add(blackoutMask) && MaskerBlackout._insideMasks.Count == 1)
		{
			MaskerBlackout.StartMaskFade(1f, fadeInTime);
			return true;
		}
		return false;
	}

	// Token: 0x060039B2 RID: 14770 RVA: 0x000FD2BA File Offset: 0x000FB4BA
	public static bool RemoveInside(Object blackoutMask, float fadeOutTime)
	{
		if (MaskerBlackout._insideMasks.Remove(blackoutMask) && MaskerBlackout._insideMasks.Count == 0)
		{
			MaskerBlackout.StartMaskFade(0f, fadeOutTime);
			return true;
		}
		return false;
	}

	// Token: 0x060039B3 RID: 14771 RVA: 0x000FD2E4 File Offset: 0x000FB4E4
	public static void SetMaskFade(float value)
	{
		foreach (MaskerBlackout maskerBlackout in MaskerBlackout._activeBlackouts)
		{
			if (maskerBlackout)
			{
				if (maskerBlackout.fadeRoutine != null)
				{
					maskerBlackout.StopCoroutine(maskerBlackout.fadeRoutine);
				}
				maskerBlackout.fadeRoutine = null;
				maskerBlackout.SetMaskValue(value);
			}
		}
	}

	// Token: 0x060039B4 RID: 14772 RVA: 0x000FD35C File Offset: 0x000FB55C
	private static void StartMaskFade(float value, float time)
	{
		foreach (MaskerBlackout maskerBlackout in MaskerBlackout._activeBlackouts)
		{
			if (maskerBlackout)
			{
				if (maskerBlackout.fadeRoutine != null)
				{
					maskerBlackout.StopCoroutine(maskerBlackout.fadeRoutine);
				}
				if (time > 0f)
				{
					maskerBlackout.fadeRoutine = maskerBlackout.StartCoroutine(maskerBlackout.MaskFade(value, time));
				}
				else
				{
					maskerBlackout.fadeRoutine = null;
					maskerBlackout.SetMaskValue(value);
				}
			}
		}
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x000FD3F0 File Offset: 0x000FB5F0
	private IEnumerator MaskFade(float value, float time)
	{
		float elapsed = 0f;
		float startValue = this.lastValue;
		while (elapsed < time)
		{
			float t = elapsed / time;
			this.SetMaskValue(Mathf.Lerp(startValue, value, t));
			yield return null;
			elapsed += Time.deltaTime;
		}
		this.SetMaskValue(value);
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x060039B6 RID: 14774 RVA: 0x000FD410 File Offset: 0x000FB610
	private void SetMaskValue(float value)
	{
		this.lastValue = value;
		Color color = this.spriteRenderer.color;
		color.a = value;
		this.spriteRenderer.color = color;
	}

	// Token: 0x04003C68 RID: 15464
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04003C69 RID: 15465
	private Coroutine fadeRoutine;

	// Token: 0x04003C6A RID: 15466
	private float lastValue;

	// Token: 0x04003C6B RID: 15467
	private static readonly HashSet<MaskerBlackout> _activeBlackouts = new HashSet<MaskerBlackout>();

	// Token: 0x04003C6C RID: 15468
	private static readonly HashSet<Object> _insideMasks = new HashSet<Object>();
}
