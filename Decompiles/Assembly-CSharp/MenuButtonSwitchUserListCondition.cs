using System;

// Token: 0x020006E9 RID: 1769
public class MenuButtonSwitchUserListCondition : MenuButtonListCondition
{
	// Token: 0x06003F94 RID: 16276 RVA: 0x00118655 File Offset: 0x00116855
	public override bool IsFulfilled()
	{
		return Platform.Current.CanReEngage;
	}
}
