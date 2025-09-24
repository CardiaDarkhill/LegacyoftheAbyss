using System;
using UnityEngine;

// Token: 0x0200064E RID: 1614
public abstract class OverMaskerBase : MonoBehaviour
{
	// Token: 0x060039C4 RID: 14788 RVA: 0x000FD5C4 File Offset: 0x000FB7C4
	private void Awake()
	{
		if (!OverMaskerBase.cached)
		{
			OverMaskerBase.blackoutMaskSetting = new OverMaskerBase.OverMaskValue(SortingLayer.NameToID("Over"), 1);
			OverMaskerBase.underBlackoutMaskSetting = new OverMaskerBase.OverMaskValue(SortingLayer.NameToID("Over"), -1);
			OverMaskerBase.cached = true;
		}
		OverMaskerBase.OverMaskType overMaskType = this.overMaskType;
		if (overMaskType == OverMaskerBase.OverMaskType.Blackout)
		{
			this.ApplySettings(OverMaskerBase.blackoutMaskSetting);
			return;
		}
		if (overMaskType != OverMaskerBase.OverMaskType.UnderBlackout)
		{
			this.ApplySettings(OverMaskerBase.blackoutMaskSetting);
			return;
		}
		this.ApplySettings(OverMaskerBase.underBlackoutMaskSetting);
	}

	// Token: 0x060039C5 RID: 14789
	protected abstract void ApplySettings(int sortingLayer, short order);

	// Token: 0x060039C6 RID: 14790 RVA: 0x000FD63B File Offset: 0x000FB83B
	protected void ApplySettings(OverMaskerBase.OverMaskValue overMaskValue)
	{
		this.ApplySettings(overMaskValue.layerID, overMaskValue.order);
	}

	// Token: 0x04003C70 RID: 15472
	[SerializeField]
	private OverMaskerBase.OverMaskType overMaskType;

	// Token: 0x04003C71 RID: 15473
	private static bool cached;

	// Token: 0x04003C72 RID: 15474
	private static OverMaskerBase.OverMaskValue blackoutMaskSetting;

	// Token: 0x04003C73 RID: 15475
	private static OverMaskerBase.OverMaskValue underBlackoutMaskSetting;

	// Token: 0x02001969 RID: 6505
	[Serializable]
	protected enum OverMaskType
	{
		// Token: 0x040095B4 RID: 38324
		Blackout,
		// Token: 0x040095B5 RID: 38325
		UnderBlackout
	}

	// Token: 0x0200196A RID: 6506
	protected sealed class OverMaskValue
	{
		// Token: 0x06009424 RID: 37924 RVA: 0x002A1830 File Offset: 0x0029FA30
		public OverMaskValue(int layerID, short order)
		{
			this.layerID = layerID;
			this.order = order;
		}

		// Token: 0x040095B6 RID: 38326
		public readonly int layerID;

		// Token: 0x040095B7 RID: 38327
		public readonly short order;
	}
}
