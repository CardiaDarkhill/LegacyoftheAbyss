using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4E RID: 3150
	[ActionCategory(ActionCategory.Math)]
	public class FloatSignToBool : FsmStateAction
	{
		// Token: 0x06005F7F RID: 24447 RVA: 0x001E50A0 File Offset: 0x001E32A0
		public override void Reset()
		{
			this.Value = null;
			this.StoreIsPositive = null;
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x001E50B0 File Offset: 0x001E32B0
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F81 RID: 24449 RVA: 0x001E50C6 File Offset: 0x001E32C6
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005F82 RID: 24450 RVA: 0x001E50CE File Offset: 0x001E32CE
		private void DoAction()
		{
			this.StoreIsPositive.Value = (Mathf.Sign(this.Value.Value) > 0f);
		}

		// Token: 0x04005CDA RID: 23770
		public FsmFloat Value;

		// Token: 0x04005CDB RID: 23771
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsPositive;

		// Token: 0x04005CDC RID: 23772
		public bool EveryFrame;
	}
}
