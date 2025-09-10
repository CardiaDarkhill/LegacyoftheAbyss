using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012BD RID: 4797
	[ActionCategory("Hollow Knight")]
	public class SendHealthManagerDeathEvent : FsmStateAction
	{
		// Token: 0x06007D81 RID: 32129 RVA: 0x00256927 File Offset: 0x00254B27
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x06007D82 RID: 32130 RVA: 0x00256934 File Offset: 0x00254B34
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SendDeathEvent();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D74 RID: 32116
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}
}
