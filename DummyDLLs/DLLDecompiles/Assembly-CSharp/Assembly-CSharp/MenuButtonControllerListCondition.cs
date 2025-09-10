using System;

// Token: 0x020006DB RID: 1755
public class MenuButtonControllerListCondition : MenuButtonListCondition
{
	// Token: 0x06003F68 RID: 16232 RVA: 0x00117F7C File Offset: 0x0011617C
	public override bool IsFulfilled()
	{
		return Platform.Current.WillDisplayControllerSettings;
	}
}
