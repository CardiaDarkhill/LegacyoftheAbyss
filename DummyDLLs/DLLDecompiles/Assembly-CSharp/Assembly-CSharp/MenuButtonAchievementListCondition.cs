using System;

// Token: 0x020006DA RID: 1754
public class MenuButtonAchievementListCondition : MenuButtonListCondition
{
	// Token: 0x06003F66 RID: 16230 RVA: 0x00117F5C File Offset: 0x0011615C
	public override bool IsFulfilled()
	{
		return !Platform.Current.HasNativeAchievementsDialog && !DemoHelper.IsDemoMode;
	}
}
