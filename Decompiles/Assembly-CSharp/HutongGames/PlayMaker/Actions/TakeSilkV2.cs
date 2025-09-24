using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001239 RID: 4665
	[ActionCategory("Hollow Knight")]
	public class TakeSilkV2 : FsmStateAction
	{
		// Token: 0x06007B7C RID: 31612 RVA: 0x0024FB9F File Offset: 0x0024DD9F
		public override void Reset()
		{
			this.Target = null;
			this.Amount = null;
			this.TakeSource = null;
		}

		// Token: 0x06007B7D RID: 31613 RVA: 0x0024FBB8 File Offset: 0x0024DDB8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if (component != null)
				{
					component.TakeSilk(this.Amount.Value, (SilkSpool.SilkTakeSource)this.TakeSource.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007BB5 RID: 31669
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		// Token: 0x04007BB6 RID: 31670
		public FsmInt Amount;

		// Token: 0x04007BB7 RID: 31671
		[ObjectType(typeof(SilkSpool.SilkTakeSource))]
		public FsmEnum TakeSource;
	}
}
