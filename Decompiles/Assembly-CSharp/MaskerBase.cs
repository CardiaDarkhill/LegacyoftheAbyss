using System;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200064A RID: 1610
[ExecuteInEditMode]
public class MaskerBase : NestedFadeGroup
{
	// Token: 0x1700068B RID: 1675
	// (get) Token: 0x060039A8 RID: 14760 RVA: 0x000FD14F File Offset: 0x000FB34F
	protected override float ExtraAlpha
	{
		get
		{
			if (this.type != MaskerBase.Types.Mask)
			{
				return 1f;
			}
			if (!MaskerBase.UseTestingAlphaInPlayMode && Application.isPlaying)
			{
				return 1f;
			}
			return MaskerBase.EditorTestingAlpha;
		}
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x000FD178 File Offset: 0x000FB378
	protected override void OnEnable()
	{
		base.OnEnable();
		MaskerBase._maskerList.Add(this);
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x000FD18B File Offset: 0x000FB38B
	protected override void OnDisable()
	{
		base.OnDisable();
		MaskerBase._maskerList.Remove(this);
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x000FD1A0 File Offset: 0x000FB3A0
	public static void RefreshAll()
	{
		foreach (MaskerBase maskerBase in MaskerBase._maskerList)
		{
			maskerBase.RefreshAlpha(false);
		}
	}

	// Token: 0x04003C63 RID: 15459
	[SerializeField]
	private MaskerBase.Types type;

	// Token: 0x04003C64 RID: 15460
	public static float EditorTestingAlpha = 1f;

	// Token: 0x04003C65 RID: 15461
	public static bool ApplyToInverseMasks;

	// Token: 0x04003C66 RID: 15462
	public static bool UseTestingAlphaInPlayMode;

	// Token: 0x04003C67 RID: 15463
	private static readonly List<MaskerBase> _maskerList = new List<MaskerBase>();

	// Token: 0x02001967 RID: 6503
	private enum Types
	{
		// Token: 0x040095AA RID: 38314
		Mask,
		// Token: 0x040095AB RID: 38315
		Other
	}
}
