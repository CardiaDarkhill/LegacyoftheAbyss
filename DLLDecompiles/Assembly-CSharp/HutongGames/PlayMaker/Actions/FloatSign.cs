using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4D RID: 3149
	[ActionCategory(ActionCategory.Math)]
	public class FloatSign : FsmStateAction
	{
		// Token: 0x06005F7B RID: 24443 RVA: 0x001E5055 File Offset: 0x001E3255
		public override void Reset()
		{
			this.Value = null;
			this.StoreValue = null;
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x001E5065 File Offset: 0x001E3265
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x001E507B File Offset: 0x001E327B
		private void DoAction()
		{
			this.StoreValue.Value = Mathf.Sign(this.Value.Value);
		}

		// Token: 0x04005CD7 RID: 23767
		public FsmFloat Value;

		// Token: 0x04005CD8 RID: 23768
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreValue;

		// Token: 0x04005CD9 RID: 23769
		public bool EveryFrame;
	}
}
