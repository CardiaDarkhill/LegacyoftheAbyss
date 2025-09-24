using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001276 RID: 4726
	[ActionCategory("CategoryName")]
	[Tooltip("TOOLTIP")]
	public class SetGameObjectIfNull : FsmStateAction
	{
		// Token: 0x06007C82 RID: 31874 RVA: 0x00253938 File Offset: 0x00251B38
		public override void Reset()
		{
			this.target = null;
			this.replaceWith = null;
		}

		// Token: 0x06007C83 RID: 31875 RVA: 0x00253948 File Offset: 0x00251B48
		public override void OnEnter()
		{
			if (this.target.Value == null)
			{
				this.target.Value = this.replaceWith.GetSafe(this);
			}
			base.Finish();
		}

		// Token: 0x04007C94 RID: 31892
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04007C95 RID: 31893
		public FsmOwnerDefault replaceWith;
	}
}
