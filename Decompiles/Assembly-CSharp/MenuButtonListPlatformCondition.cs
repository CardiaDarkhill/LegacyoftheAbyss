using System;
using UnityEngine;

// Token: 0x020006E4 RID: 1764
public class MenuButtonListPlatformCondition : MenuButtonListCondition
{
	// Token: 0x06003F88 RID: 16264 RVA: 0x0011856C File Offset: 0x0011676C
	public override bool IsFulfilled()
	{
		RuntimePlatform platform = Application.platform;
		bool activate = this.defaultActivation;
		foreach (MenuButtonListPlatformCondition.PlatformBoolPair platformBoolPair in this.platforms)
		{
			if (platformBoolPair.platform == platform)
			{
				activate = platformBoolPair.activate;
				break;
			}
		}
		return activate;
	}

	// Token: 0x0400413D RID: 16701
	public MenuButtonListPlatformCondition.PlatformBoolPair[] platforms;

	// Token: 0x0400413E RID: 16702
	[Space]
	public bool defaultActivation = true;

	// Token: 0x020019DA RID: 6618
	[Serializable]
	public struct PlatformBoolPair
	{
		// Token: 0x0400975E RID: 38750
		public RuntimePlatform platform;

		// Token: 0x0400975F RID: 38751
		public bool activate;
	}
}
