using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012BC RID: 4796
	[ActionCategory("Hollow Knight")]
	public class CheckInvincibility : FsmStateAction
	{
		// Token: 0x06007D7E RID: 32126 RVA: 0x002568A3 File Offset: 0x00254AA3
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeValue = new FsmBool();
		}

		// Token: 0x06007D7F RID: 32127 RVA: 0x002568BC File Offset: 0x00254ABC
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					this.storeValue.Value = component.CheckInvincible();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D72 RID: 32114
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D73 RID: 32115
		public FsmBool storeValue;
	}
}
