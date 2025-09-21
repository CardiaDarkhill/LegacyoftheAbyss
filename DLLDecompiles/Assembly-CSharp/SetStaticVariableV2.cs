using System;
using HutongGames.PlayMaker;

// Token: 0x0200078B RID: 1931
public class SetStaticVariableV2 : FsmStateAction
{
	// Token: 0x06004479 RID: 17529 RVA: 0x0012BF23 File Offset: 0x0012A123
	public override void Reset()
	{
		this.variableName = null;
		this.setValue = null;
		this.sceneTransitionsLimit = null;
		this.everyFrame = null;
	}

	// Token: 0x0600447A RID: 17530 RVA: 0x0012BF44 File Offset: 0x0012A144
	public override void OnEnter()
	{
		bool flag = !this.variableName.IsNone && !this.setValue.IsNone;
		if (flag)
		{
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.setValue);
			StaticVariableList.SetValue(this.variableName.Value, valueFromFsmVar, this.sceneTransitionsLimit.Value);
			this.previousValue = valueFromFsmVar;
		}
		if (!flag || !this.everyFrame.Value)
		{
			base.Finish();
		}
	}

	// Token: 0x0600447B RID: 17531 RVA: 0x0012BFBC File Offset: 0x0012A1BC
	public override void OnUpdate()
	{
		object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.setValue);
		if (!valueFromFsmVar.Equals(this.previousValue))
		{
			this.previousValue = valueFromFsmVar;
			StaticVariableList.SetValue(this.variableName.Value, valueFromFsmVar, this.sceneTransitionsLimit.Value);
		}
	}

	// Token: 0x04004586 RID: 17798
	public FsmString variableName;

	// Token: 0x04004587 RID: 17799
	public FsmVar setValue;

	// Token: 0x04004588 RID: 17800
	public FsmInt sceneTransitionsLimit;

	// Token: 0x04004589 RID: 17801
	public FsmBool everyFrame;

	// Token: 0x0400458A RID: 17802
	private object previousValue;
}
