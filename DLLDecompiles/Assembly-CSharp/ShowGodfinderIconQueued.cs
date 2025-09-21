using System;
using HutongGames.PlayMaker;

// Token: 0x020003B0 RID: 944
[ActionCategory("Hollow Knight")]
public class ShowGodfinderIconQueued : FsmStateAction
{
	// Token: 0x06001FB6 RID: 8118 RVA: 0x00090F13 File Offset: 0x0008F113
	public override void Reset()
	{
		this.delay = null;
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x00090F1C File Offset: 0x0008F11C
	public override void OnEnter()
	{
		GodfinderIcon.ShowIconQueued(this.delay.Value);
		base.Finish();
	}

	// Token: 0x04001EC0 RID: 7872
	public FsmFloat delay;
}
