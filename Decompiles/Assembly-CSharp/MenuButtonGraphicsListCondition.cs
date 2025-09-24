using System;

// Token: 0x020006DC RID: 1756
public class MenuButtonGraphicsListCondition : MenuButtonListCondition
{
	// Token: 0x06003F6A RID: 16234 RVA: 0x00117F90 File Offset: 0x00116190
	public override bool IsFulfilled()
	{
		return Platform.Current.WillDisplayGraphicsSettings;
	}
}
