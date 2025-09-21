using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001263 RID: 4707
	[ActionCategory("Hollow Knight")]
	public sealed class AddSilkV2 : FsmStateAction
	{
		// Token: 0x06007C3D RID: 31805 RVA: 0x00252578 File Offset: 0x00250778
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.amount = new FsmInt();
			this.playHeroEffect = new FsmBool();
			this.silent = null;
		}

		// Token: 0x06007C3E RID: 31806 RVA: 0x002525A4 File Offset: 0x002507A4
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HeroController component = gameObject.GetComponent<HeroController>();
				if (component != null)
				{
					if (this.silent.Value)
					{
						component.SuppressRefillSound(2);
					}
					component.AddSilk(this.amount.Value, this.playHeroEffect.Value);
				}
				base.Finish();
			}
		}

		// Token: 0x04007C4B RID: 31819
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007C4C RID: 31820
		public FsmInt amount;

		// Token: 0x04007C4D RID: 31821
		public FsmBool playHeroEffect;

		// Token: 0x04007C4E RID: 31822
		public FsmBool silent;
	}
}
