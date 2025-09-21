using System;
using HutongGames.PlayMaker;

// Token: 0x0200078D RID: 1933
public class CheckStaticBool : FsmStateAction
{
	// Token: 0x06004480 RID: 17536 RVA: 0x0012C086 File Offset: 0x0012A286
	public override void Reset()
	{
		this.variableName = null;
		this.trueEvent = null;
		this.falseEvent = null;
		this.EveryFrame = false;
	}

	// Token: 0x06004481 RID: 17537 RVA: 0x0012C0A4 File Offset: 0x0012A2A4
	public override void OnEnter()
	{
		this.DoCheck();
		if (!this.EveryFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06004482 RID: 17538 RVA: 0x0012C0BA File Offset: 0x0012A2BA
	public override void OnUpdate()
	{
		this.DoCheck();
	}

	// Token: 0x06004483 RID: 17539 RVA: 0x0012C0C4 File Offset: 0x0012A2C4
	private void DoCheck()
	{
		if (!this.variableName.IsNone && StaticVariableList.Exists(this.variableName.Value) && StaticVariableList.GetValue<bool>(this.variableName.Value))
		{
			base.Fsm.Event(this.trueEvent);
			return;
		}
		base.Fsm.Event(this.falseEvent);
	}

	// Token: 0x0400458D RID: 17805
	public FsmString variableName;

	// Token: 0x0400458E RID: 17806
	public FsmEvent trueEvent;

	// Token: 0x0400458F RID: 17807
	public FsmEvent falseEvent;

	// Token: 0x04004590 RID: 17808
	public bool EveryFrame;
}
