using System;

// Token: 0x020006E7 RID: 1767
public sealed class MenuButtonSaveImportCondition : MenuButtonListCondition
{
	// Token: 0x06003F8E RID: 16270 RVA: 0x001185E7 File Offset: 0x001167E7
	public override bool IsFulfilled()
	{
		return Platform.Current && Platform.Current.ShowSaveDataImport;
	}
}
