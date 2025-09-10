using System;

// Token: 0x02000386 RID: 902
public class GGGetTotalBindings : FSMUtility.GetIntFsmStateAction
{
	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06001EAF RID: 7855 RVA: 0x0008C9AE File Offset: 0x0008ABAE
	public override int IntValue
	{
		get
		{
			return BossSequenceBindingsDisplay.CountTotalBindings();
		}
	}
}
