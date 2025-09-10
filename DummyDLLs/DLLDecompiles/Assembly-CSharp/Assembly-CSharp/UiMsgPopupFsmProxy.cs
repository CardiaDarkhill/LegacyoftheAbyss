using System;

// Token: 0x02000744 RID: 1860
public class UiMsgPopupFsmProxy : UIMsgPopupBaseBase
{
	// Token: 0x06004250 RID: 16976 RVA: 0x00124EE5 File Offset: 0x001230E5
	public void Activated()
	{
		UIMsgPopupBaseBase.UpdatePosition(base.transform);
	}

	// Token: 0x06004251 RID: 16977 RVA: 0x00124EF2 File Offset: 0x001230F2
	public void Deactivated()
	{
		if (UIMsgPopupBaseBase.LastActiveMsgShared == base.transform)
		{
			UIMsgPopupBaseBase.LastActiveMsgShared = null;
		}
	}
}
