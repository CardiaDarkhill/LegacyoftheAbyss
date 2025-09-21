using System;

// Token: 0x020006E0 RID: 1760
public class MenuButtonKeyboardListCondition : MenuButtonListCondition
{
	// Token: 0x06003F73 RID: 16243 RVA: 0x001180B6 File Offset: 0x001162B6
	public override bool IsFulfilled()
	{
		return !DemoHelper.IsDemoMode && Platform.Current.WillDisplayKeyboardSettings;
	}
}
