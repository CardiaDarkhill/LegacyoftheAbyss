using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class CaravanMusicControl : MonoBehaviour
{
	// Token: 0x060008AE RID: 2222 RVA: 0x00028A0C File Offset: 0x00026C0C
	private void Awake()
	{
		this.enterRange.InsideStateChanged += this.OnEnterRangeInsideStateChanged;
		this.exitRange.InsideStateChanged += this.OnExitRangeInsideStateChanged;
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00028A3C File Offset: 0x00026C3C
	private void Start()
	{
		this.hc = HeroController.instance;
		if (this.hc.isHeroInPosition && this.enterRange.IsInside)
		{
			this.SetInside(true);
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00028A6A File Offset: 0x00026C6A
	private void OnDestroy()
	{
		if (this.wasInside)
		{
			this.SetInside(false);
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x00028A7B File Offset: 0x00026C7B
	private void OnEnterRangeInsideStateChanged(bool isInside)
	{
		if (!this.hc || !this.hc.isHeroInPosition)
		{
			return;
		}
		if (isInside)
		{
			this.SetInside(true);
		}
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00028AA2 File Offset: 0x00026CA2
	private void OnExitRangeInsideStateChanged(bool isInside)
	{
		if (!this.hc || !this.hc.isHeroInPosition)
		{
			return;
		}
		if (!isInside)
		{
			this.SetInside(false);
		}
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x00028ACC File Offset: 0x00026CCC
	private void SetInside(bool value)
	{
		if (value == this.wasInside)
		{
			return;
		}
		if (value)
		{
			CaravanMusicControl._insideCount++;
			if (CaravanMusicControl._insideCount == 1)
			{
				EventRegister.SendEvent(EventRegisterEvents.FleaMusicStart, null);
			}
		}
		else
		{
			CaravanMusicControl._insideCount--;
			if (CaravanMusicControl._insideCount <= 0)
			{
				EventRegister.SendEvent(EventRegisterEvents.FleaMusicStop, null);
			}
		}
		this.wasInside = value;
	}

	// Token: 0x04000852 RID: 2130
	[SerializeField]
	private TrackTriggerObjects enterRange;

	// Token: 0x04000853 RID: 2131
	[SerializeField]
	private TrackTriggerObjects exitRange;

	// Token: 0x04000854 RID: 2132
	private HeroController hc;

	// Token: 0x04000855 RID: 2133
	private bool wasInside;

	// Token: 0x04000856 RID: 2134
	private static int _insideCount;
}
