using System;
using HutongGames.PlayMaker;

// Token: 0x02000400 RID: 1024
public class CheckSendEventLimit : FsmStateAction
{
	// Token: 0x060022C7 RID: 8903 RVA: 0x0009FBB9 File Offset: 0x0009DDB9
	public override void Reset()
	{
		this.gameObject = new FsmGameObject();
		this.target = new FsmEventTarget();
		this.trueEvent = null;
		this.falseEvent = null;
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x0009FBE0 File Offset: 0x0009DDE0
	public override void OnEnter()
	{
		if (this.gameObject.Value)
		{
			LimitSendEvents component = base.Owner.gameObject.GetComponent<LimitSendEvents>();
			if (component && !component.Add(this.gameObject.Value))
			{
				base.Fsm.Event(this.target, this.falseEvent);
			}
			else
			{
				base.Fsm.Event(this.target, this.trueEvent);
			}
		}
		base.Finish();
	}

	// Token: 0x04002196 RID: 8598
	public FsmGameObject gameObject;

	// Token: 0x04002197 RID: 8599
	public FsmEventTarget target;

	// Token: 0x04002198 RID: 8600
	public FsmEvent trueEvent;

	// Token: 0x04002199 RID: 8601
	public FsmEvent falseEvent;
}
