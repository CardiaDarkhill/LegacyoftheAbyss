using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD2 RID: 3282
	public class CheckPlayerDataTimeLimit : PlayerDataVariableAction
	{
		// Token: 0x060061D3 RID: 25043 RVA: 0x001EF48D File Offset: 0x001ED68D
		public override void Reset()
		{
			this.VariableName = null;
			this.AboveEvent = null;
			this.BelowEvent = null;
			this.StoreBool = null;
		}

		// Token: 0x060061D4 RID: 25044 RVA: 0x001EF4AC File Offset: 0x001ED6AC
		public override void OnEnter()
		{
			float variable = PlayerData.instance.GetVariable(this.VariableName.Value);
			bool flag = Time.time >= variable;
			this.StoreBool.Value = flag;
			base.Fsm.Event(flag ? this.AboveEvent : this.BelowEvent);
			base.Finish();
		}

		// Token: 0x060061D5 RID: 25045 RVA: 0x001EF509 File Offset: 0x001ED709
		public override bool GetShouldErrorCheck()
		{
			return true;
		}

		// Token: 0x060061D6 RID: 25046 RVA: 0x001EF50C File Offset: 0x001ED70C
		public override string GetVariableName()
		{
			return this.VariableName.Value;
		}

		// Token: 0x060061D7 RID: 25047 RVA: 0x001EF519 File Offset: 0x001ED719
		public override Type GetVariableType()
		{
			return typeof(float);
		}

		// Token: 0x04005FF7 RID: 24567
		[RequiredField]
		public FsmString VariableName;

		// Token: 0x04005FF8 RID: 24568
		public FsmEvent AboveEvent;

		// Token: 0x04005FF9 RID: 24569
		public FsmEvent BelowEvent;

		// Token: 0x04005FFA RID: 24570
		[UIHint(UIHint.Variable)]
		public FsmBool StoreBool;
	}
}
