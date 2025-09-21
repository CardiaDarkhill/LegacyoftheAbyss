using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD3 RID: 3283
	public class SetPlayerDataTimeLimit : PlayerDataVariableAction
	{
		// Token: 0x060061D9 RID: 25049 RVA: 0x001EF52D File Offset: 0x001ED72D
		public override void Reset()
		{
			this.VariableName = null;
			this.Delay = null;
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x001EF53D File Offset: 0x001ED73D
		public override void OnEnter()
		{
			PlayerData.instance.SetVariable(this.VariableName.Value, Time.time + this.Delay.Value);
			base.Finish();
		}

		// Token: 0x060061DB RID: 25051 RVA: 0x001EF56B File Offset: 0x001ED76B
		public override bool GetShouldErrorCheck()
		{
			return true;
		}

		// Token: 0x060061DC RID: 25052 RVA: 0x001EF56E File Offset: 0x001ED76E
		public override string GetVariableName()
		{
			return this.VariableName.Value;
		}

		// Token: 0x060061DD RID: 25053 RVA: 0x001EF57B File Offset: 0x001ED77B
		public override Type GetVariableType()
		{
			return typeof(float);
		}

		// Token: 0x04005FFB RID: 24571
		[RequiredField]
		public FsmString VariableName;

		// Token: 0x04005FFC RID: 24572
		[RequiredField]
		public FsmFloat Delay;
	}
}
