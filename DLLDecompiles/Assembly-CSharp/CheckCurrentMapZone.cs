using System;
using HutongGames.PlayMaker;

// Token: 0x0200041D RID: 1053
[ActionCategory("Hollow Knight")]
public class CheckCurrentMapZone : FsmStateAction
{
	// Token: 0x060024EA RID: 9450 RVA: 0x000AA39F File Offset: 0x000A859F
	public override void Reset()
	{
		this.mapZone = null;
		this.equalEvent = null;
		this.notEqualEvent = null;
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x000AA3B8 File Offset: 0x000A85B8
	public override void OnEnter()
	{
		if (GameManager.instance)
		{
			if (this.mapZone.Value == GameManager.instance.GetCurrentMapZone())
			{
				base.Fsm.Event(this.equalEvent);
			}
			else
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}
		base.Finish();
	}

	// Token: 0x040022CA RID: 8906
	[RequiredField]
	public FsmString mapZone;

	// Token: 0x040022CB RID: 8907
	public FsmEvent equalEvent;

	// Token: 0x040022CC RID: 8908
	public FsmEvent notEqualEvent;
}
