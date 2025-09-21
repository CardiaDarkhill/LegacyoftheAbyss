using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001237 RID: 4663
	[ActionCategory("Hollow Knight")]
	public class AddSilk : FsmStateAction
	{
		// Token: 0x06007B76 RID: 31606 RVA: 0x0024FA81 File Offset: 0x0024DC81
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.amount = new FsmInt();
			this.playHeroEffect = new FsmBool();
		}

		// Token: 0x06007B77 RID: 31607 RVA: 0x0024FAA4 File Offset: 0x0024DCA4
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HeroController component = gameObject.GetComponent<HeroController>();
				if (component != null)
				{
					component.AddSilk(this.amount.Value, this.playHeroEffect.Value);
				}
				base.Finish();
			}
		}

		// Token: 0x04007BB0 RID: 31664
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007BB1 RID: 31665
		public FsmInt amount;

		// Token: 0x04007BB2 RID: 31666
		public FsmBool playHeroEffect;
	}
}
