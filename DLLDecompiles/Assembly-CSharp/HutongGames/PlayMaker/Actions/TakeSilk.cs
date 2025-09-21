using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001238 RID: 4664
	[ActionCategory("Hollow Knight")]
	public class TakeSilk : FsmStateAction
	{
		// Token: 0x06007B79 RID: 31609 RVA: 0x0024FB1A File Offset: 0x0024DD1A
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.amount = new FsmInt();
		}

		// Token: 0x06007B7A RID: 31610 RVA: 0x0024FB34 File Offset: 0x0024DD34
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HeroController component = gameObject.GetComponent<HeroController>();
				if (component != null)
				{
					component.TakeSilk(this.amount.Value);
				}
				base.Finish();
			}
		}

		// Token: 0x04007BB3 RID: 31667
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007BB4 RID: 31668
		public FsmInt amount;
	}
}
