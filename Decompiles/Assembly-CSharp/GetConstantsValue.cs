using System;
using HutongGames.PlayMaker;

// Token: 0x02000758 RID: 1880
[ActionCategory("Hollow Knight")]
public class GetConstantsValue : FsmStateAction
{
	// Token: 0x060042AA RID: 17066 RVA: 0x00125D54 File Offset: 0x00123F54
	public override void Reset()
	{
		this.variableName = null;
		this.storeValue = null;
	}

	// Token: 0x060042AB RID: 17067 RVA: 0x00125D64 File Offset: 0x00123F64
	public override void OnEnter()
	{
		if (!this.variableName.IsNone && !this.storeValue.IsNone)
		{
			switch (this.storeValue.Type)
			{
			case VariableType.Float:
				this.storeValue.SetValue(Constants.GetConstantValue<float>(this.variableName.Value));
				break;
			case VariableType.Int:
				this.storeValue.SetValue(Constants.GetConstantValue<int>(this.variableName.Value));
				break;
			case VariableType.Bool:
				this.storeValue.SetValue(Constants.GetConstantValue<bool>(this.variableName.Value));
				break;
			case VariableType.String:
				this.storeValue.SetValue(Constants.GetConstantValue<string>(this.variableName.Value));
				break;
			}
		}
		base.Finish();
	}

	// Token: 0x04004504 RID: 17668
	public FsmString variableName;

	// Token: 0x04004505 RID: 17669
	[UIHint(UIHint.Variable)]
	public FsmVar storeValue;
}
