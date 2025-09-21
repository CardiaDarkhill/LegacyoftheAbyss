using System;

// Token: 0x020006E1 RID: 1761
public sealed class MenuButtonLimitedGraphicsCondition : MenuButtonListCondition
{
	// Token: 0x06003F75 RID: 16245 RVA: 0x001180D3 File Offset: 0x001162D3
	public override bool IsFulfilled()
	{
		return !Platform.Current.LimitedGraphicsSettings;
	}
}
