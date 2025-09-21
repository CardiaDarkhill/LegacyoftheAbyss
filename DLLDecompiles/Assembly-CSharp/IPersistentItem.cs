using System;

// Token: 0x020007C8 RID: 1992
public interface IPersistentItem
{
	// Token: 0x0600461E RID: 17950
	string GetId();

	// Token: 0x0600461F RID: 17951
	string GetSceneName();

	// Token: 0x06004620 RID: 17952
	string GetValueTypeName();

	// Token: 0x06004621 RID: 17953
	bool GetIsSemiPersistent();
}
