using System;
using HutongGames.PlayMaker;

// Token: 0x0200078A RID: 1930
public class SetStaticVariable : FsmStateAction
{
	// Token: 0x06004476 RID: 17526 RVA: 0x0012BEA8 File Offset: 0x0012A0A8
	public override void Reset()
	{
		this.variableName = null;
		this.setValue = null;
		this.sceneTransitionsLimit = null;
	}

	// Token: 0x06004477 RID: 17527 RVA: 0x0012BEC0 File Offset: 0x0012A0C0
	public override void OnEnter()
	{
		if (!this.variableName.IsNone && !this.setValue.IsNone)
		{
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.setValue);
			StaticVariableList.SetValue(this.variableName.Value, valueFromFsmVar, this.sceneTransitionsLimit.Value);
		}
		base.Finish();
	}

	// Token: 0x04004583 RID: 17795
	public FsmString variableName;

	// Token: 0x04004584 RID: 17796
	public FsmVar setValue;

	// Token: 0x04004585 RID: 17797
	public FsmInt sceneTransitionsLimit;
}
