using System;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD0 RID: 3280
	public class PlayerDataVariableTest : PlayerDataVariableAction
	{
		// Token: 0x060061C7 RID: 25031 RVA: 0x001EF31A File Offset: 0x001ED51A
		public override void Reset()
		{
			this.VariableName = null;
			this.ExpectedValue = null;
			this.IsExpectedEvent = null;
			this.IsNotExpectedEvent = null;
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x001EF338 File Offset: 0x001ED538
		public override void OnEnter()
		{
			if (!this.VariableName.IsNone && !this.ExpectedValue.IsNone)
			{
				object variable = PlayerData.instance.GetVariable(this.VariableName.Value, this.ExpectedValue.RealType);
				base.Fsm.Event((variable != null && variable.Equals(this.ExpectedValue.GetValue())) ? this.IsExpectedEvent : this.IsNotExpectedEvent);
			}
			base.Finish();
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x001EF3B5 File Offset: 0x001ED5B5
		public override bool GetShouldErrorCheck()
		{
			return !this.VariableName.UsesVariable;
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x001EF3C5 File Offset: 0x001ED5C5
		public override string GetVariableName()
		{
			return this.VariableName.Value;
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x001EF3D2 File Offset: 0x001ED5D2
		public override Type GetVariableType()
		{
			return this.ExpectedValue.RealType;
		}

		// Token: 0x04005FF1 RID: 24561
		[RequiredField]
		public FsmString VariableName;

		// Token: 0x04005FF2 RID: 24562
		[RequiredField]
		public FsmVar ExpectedValue;

		// Token: 0x04005FF3 RID: 24563
		public FsmEvent IsExpectedEvent;

		// Token: 0x04005FF4 RID: 24564
		public FsmEvent IsNotExpectedEvent;
	}
}
