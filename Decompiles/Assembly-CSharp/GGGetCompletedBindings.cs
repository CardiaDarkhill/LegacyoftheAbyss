using System;

// Token: 0x02000385 RID: 901
public class GGGetCompletedBindings : FSMUtility.GetIntFsmStateAction
{
	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06001EAD RID: 7853 RVA: 0x0008C99F File Offset: 0x0008AB9F
	public override int IntValue
	{
		get
		{
			return BossSequenceBindingsDisplay.CountCompletedBindings();
		}
	}
}
