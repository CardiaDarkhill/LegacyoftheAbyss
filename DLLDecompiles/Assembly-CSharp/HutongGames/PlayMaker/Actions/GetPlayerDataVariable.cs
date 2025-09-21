using System;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCF RID: 3279
	public class GetPlayerDataVariable : PlayerDataVariableAction
	{
		// Token: 0x060061C1 RID: 25025 RVA: 0x001EF27D File Offset: 0x001ED47D
		public override void Reset()
		{
			this.VariableName = null;
			this.StoreValue = null;
		}

		// Token: 0x060061C2 RID: 25026 RVA: 0x001EF290 File Offset: 0x001ED490
		public override void OnEnter()
		{
			if (!this.VariableName.IsNone && !this.StoreValue.IsNone)
			{
				this.StoreValue.SetValue(PlayerData.instance.GetVariable(this.VariableName.Value, this.StoreValue.RealType));
			}
			base.Finish();
		}

		// Token: 0x060061C3 RID: 25027 RVA: 0x001EF2E8 File Offset: 0x001ED4E8
		public override bool GetShouldErrorCheck()
		{
			return !this.VariableName.UsesVariable;
		}

		// Token: 0x060061C4 RID: 25028 RVA: 0x001EF2F8 File Offset: 0x001ED4F8
		public override string GetVariableName()
		{
			return this.VariableName.Value;
		}

		// Token: 0x060061C5 RID: 25029 RVA: 0x001EF305 File Offset: 0x001ED505
		public override Type GetVariableType()
		{
			return this.StoreValue.RealType;
		}

		// Token: 0x04005FEF RID: 24559
		[RequiredField]
		public FsmString VariableName;

		// Token: 0x04005FF0 RID: 24560
		[UIHint(UIHint.Variable)]
		public FsmVar StoreValue;
	}
}
