using System;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class CurveScaleAnimation : VectorCurveAnimator
{
	// Token: 0x06000377 RID: 887 RVA: 0x00011E85 File Offset: 0x00010085
	protected override bool UsesSpace()
	{
		return false;
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000378 RID: 888 RVA: 0x00011E88 File Offset: 0x00010088
	// (set) Token: 0x06000379 RID: 889 RVA: 0x00011E95 File Offset: 0x00010095
	protected override Vector3 Vector
	{
		get
		{
			return base.CurrentTransform.localScale;
		}
		set
		{
			base.CurrentTransform.localScale = value;
		}
	}
}
