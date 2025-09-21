using System;
using HutongGames.PlayMaker;

// Token: 0x0200078C RID: 1932
public class GetStaticVariable : FsmStateAction
{
	// Token: 0x0600447D RID: 17533 RVA: 0x0012C014 File Offset: 0x0012A214
	public override void Reset()
	{
		this.variableName = null;
		this.storeValue = null;
	}

	// Token: 0x0600447E RID: 17534 RVA: 0x0012C024 File Offset: 0x0012A224
	public override void OnEnter()
	{
		if (!this.variableName.IsNone && !this.storeValue.IsNone && StaticVariableList.Exists(this.variableName.Value))
		{
			this.storeValue.SetValue(StaticVariableList.GetValue(this.variableName.Value));
		}
		base.Finish();
	}

	// Token: 0x0400458B RID: 17803
	public FsmString variableName;

	// Token: 0x0400458C RID: 17804
	[UIHint(UIHint.Variable)]
	public FsmVar storeValue;
}
