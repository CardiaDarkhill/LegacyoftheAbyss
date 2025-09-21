using System;

// Token: 0x020006E6 RID: 1766
public class MenuButtonQuitListCondition : MenuButtonListCondition
{
	// Token: 0x06003F8C RID: 16268 RVA: 0x001185D3 File Offset: 0x001167D3
	public override bool IsFulfilled()
	{
		return Platform.Current.WillDisplayQuitButton;
	}
}
