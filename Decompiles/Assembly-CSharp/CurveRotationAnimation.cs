using System;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class CurveRotationAnimation : VectorCurveAnimator
{
	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000374 RID: 884 RVA: 0x00011E37 File Offset: 0x00010037
	// (set) Token: 0x06000375 RID: 885 RVA: 0x00011E59 File Offset: 0x00010059
	protected override Vector3 Vector
	{
		get
		{
			if (this.space != Space.Self)
			{
				return base.CurrentTransform.eulerAngles;
			}
			return base.CurrentTransform.localEulerAngles;
		}
		set
		{
			if (this.space == Space.Self)
			{
				base.CurrentTransform.localEulerAngles = value;
				return;
			}
			base.CurrentTransform.eulerAngles = value;
		}
	}
}
