using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B3 RID: 4787
	[ActionCategory("Hollow Knight")]
	public class GetHP : FsmStateAction
	{
		// Token: 0x06007D62 RID: 32098 RVA: 0x002563DE File Offset: 0x002545DE
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeValue = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x06007D63 RID: 32099 RVA: 0x00256400 File Offset: 0x00254600
		public override void OnEnter()
		{
			this.storeValue.Value = 0;
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					this.storeValue.Value = component.hp;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D5D RID: 32093
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D5E RID: 32094
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;
	}
}
