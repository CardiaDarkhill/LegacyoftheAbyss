using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC1 RID: 3777
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets a Game Object's Tag.")]
	public class SetTag : FsmStateAction
	{
		// Token: 0x06006ABA RID: 27322 RVA: 0x00214A91 File Offset: 0x00212C91
		public override void Reset()
		{
			this.gameObject = null;
			this.tag = "Untagged";
		}

		// Token: 0x06006ABB RID: 27323 RVA: 0x00214AAC File Offset: 0x00212CAC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.tag = this.tag.Value;
			}
			base.Finish();
		}

		// Token: 0x040069FE RID: 27134
		[Tooltip("The Game Object to set.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069FF RID: 27135
		[UIHint(UIHint.Tag)]
		[Tooltip("The tag. Note: Use Unity's <a href=\"http://unity3d.com/support/documentation/Components/class-TagManager.html\">Tag Manager</a> to add/edit tags.")]
		public FsmString tag;
	}
}
