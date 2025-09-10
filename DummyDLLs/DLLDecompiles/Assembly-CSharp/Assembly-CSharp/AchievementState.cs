using System;

// Token: 0x0200045E RID: 1118
public struct AchievementState
{
	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x06002829 RID: 10281 RVA: 0x000B1D9C File Offset: 0x000AFF9C
	public static AchievementState Invalid
	{
		get
		{
			return new AchievementState
			{
				isValid = false
			};
		}
	}

	// Token: 0x04002451 RID: 9297
	public bool isValid;

	// Token: 0x04002452 RID: 9298
	public bool isUnlocked;

	// Token: 0x04002453 RID: 9299
	public bool isProgressive;

	// Token: 0x04002454 RID: 9300
	public long progressValue;

	// Token: 0x04002455 RID: 9301
	public long maxValue;
}
