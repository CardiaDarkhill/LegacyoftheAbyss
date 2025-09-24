using System;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class CurveOffsetAnimation : VectorCurveAnimator
{
	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000371 RID: 881 RVA: 0x00011DE9 File Offset: 0x0000FFE9
	// (set) Token: 0x06000372 RID: 882 RVA: 0x00011E0B File Offset: 0x0001000B
	protected override Vector3 Vector
	{
		get
		{
			if (this.space != Space.Self)
			{
				return base.CurrentTransform.position;
			}
			return base.CurrentTransform.localPosition;
		}
		set
		{
			if (this.space == Space.Self)
			{
				base.CurrentTransform.localPosition = value;
				return;
			}
			base.CurrentTransform.position = value;
		}
	}
}
