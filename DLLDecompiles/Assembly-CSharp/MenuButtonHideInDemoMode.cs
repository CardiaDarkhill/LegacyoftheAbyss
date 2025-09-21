using System;
using UnityEngine;

// Token: 0x020006DD RID: 1757
public class MenuButtonHideInDemoMode : MenuButtonListCondition
{
	// Token: 0x06003F6C RID: 16236 RVA: 0x00117FA4 File Offset: 0x001161A4
	public override bool IsFulfilled()
	{
		if (this.onlyExhibitionMode)
		{
			return !DemoHelper.IsExhibitionMode;
		}
		return !DemoHelper.IsDemoMode;
	}

	// Token: 0x0400412E RID: 16686
	[SerializeField]
	private bool onlyExhibitionMode;
}
