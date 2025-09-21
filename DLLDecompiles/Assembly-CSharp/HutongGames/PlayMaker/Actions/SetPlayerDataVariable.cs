using System;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD1 RID: 3281
	public class SetPlayerDataVariable : PlayerDataVariableAction
	{
		// Token: 0x060061CD RID: 25037 RVA: 0x001EF3E7 File Offset: 0x001ED5E7
		public override void Reset()
		{
			this.VariableName = null;
			this.SetValue = null;
		}

		// Token: 0x060061CE RID: 25038 RVA: 0x001EF3F8 File Offset: 0x001ED5F8
		public override void OnEnter()
		{
			if (!this.VariableName.IsNone && !this.SetValue.IsNone)
			{
				this.SetValue.UpdateValue();
				PlayerData.instance.SetVariable(this.VariableName.Value, this.SetValue.GetValue(), this.SetValue.RealType);
			}
			base.Finish();
		}

		// Token: 0x060061CF RID: 25039 RVA: 0x001EF45B File Offset: 0x001ED65B
		public override bool GetShouldErrorCheck()
		{
			return !this.VariableName.UsesVariable;
		}

		// Token: 0x060061D0 RID: 25040 RVA: 0x001EF46B File Offset: 0x001ED66B
		public override string GetVariableName()
		{
			return this.VariableName.Value;
		}

		// Token: 0x060061D1 RID: 25041 RVA: 0x001EF478 File Offset: 0x001ED678
		public override Type GetVariableType()
		{
			return this.SetValue.RealType;
		}

		// Token: 0x04005FF5 RID: 24565
		[RequiredField]
		public FsmString VariableName;

		// Token: 0x04005FF6 RID: 24566
		[RequiredField]
		public FsmVar SetValue;
	}
}
