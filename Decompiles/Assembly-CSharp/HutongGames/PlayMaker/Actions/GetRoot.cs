using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB7 RID: 3767
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the top most parent of the Game Object.\nIf the game object has no parent, returns itself.")]
	public class GetRoot : FsmStateAction
	{
		// Token: 0x06006A90 RID: 27280 RVA: 0x002144CB File Offset: 0x002126CB
		public override void Reset()
		{
			this.gameObject = null;
			this.storeRoot = null;
		}

		// Token: 0x06006A91 RID: 27281 RVA: 0x002144DB File Offset: 0x002126DB
		public override void OnEnter()
		{
			this.DoGetRoot();
			base.Finish();
		}

		// Token: 0x06006A92 RID: 27282 RVA: 0x002144EC File Offset: 0x002126EC
		private void DoGetRoot()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeRoot.Value = ownerDefaultTarget.transform.root.gameObject;
		}

		// Token: 0x040069DE RID: 27102
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069DF RID: 27103
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the root of the Game Object in a Game Object Variable.")]
		public FsmGameObject storeRoot;
	}
}
