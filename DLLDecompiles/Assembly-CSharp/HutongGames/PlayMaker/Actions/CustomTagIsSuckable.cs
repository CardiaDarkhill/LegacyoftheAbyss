using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001279 RID: 4729
	[ActionCategory("Hollow Knight")]
	public class CustomTagIsSuckable : FsmStateAction
	{
		// Token: 0x06007C8E RID: 31886 RVA: 0x00253C9B File Offset: 0x00251E9B
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeIsSuckable = new FsmBool();
		}

		// Token: 0x06007C8F RID: 31887 RVA: 0x00253CB4 File Offset: 0x00251EB4
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				if (gameObject.GetComponent<CustomTag>() == null)
				{
					this.storeIsSuckable = false;
					base.Fsm.Event(this.isNotSuckableEvent);
				}
				else
				{
					bool flag = gameObject.GetComponent<CustomTag>().IsSuckable();
					this.storeIsSuckable = flag;
					base.Fsm.Event(flag ? this.isSuckableEvent : this.isNotSuckableEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007CA3 RID: 31907
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007CA4 RID: 31908
		public FsmEvent isSuckableEvent;

		// Token: 0x04007CA5 RID: 31909
		public FsmEvent isNotSuckableEvent;

		// Token: 0x04007CA6 RID: 31910
		public FsmBool storeIsSuckable;
	}
}
