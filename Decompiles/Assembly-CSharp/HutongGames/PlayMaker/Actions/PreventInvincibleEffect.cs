using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B9 RID: 4793
	[ActionCategory("Hollow Knight")]
	public class PreventInvincibleEffect : FsmStateAction
	{
		// Token: 0x06007D75 RID: 32117 RVA: 0x00256717 File Offset: 0x00254917
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.preventEffect = new FsmBool();
		}

		// Token: 0x06007D76 RID: 32118 RVA: 0x00256730 File Offset: 0x00254930
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetPreventInvincibleEffect(this.preventEffect.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D6C RID: 32108
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D6D RID: 32109
		public FsmBool preventEffect;
	}
}
