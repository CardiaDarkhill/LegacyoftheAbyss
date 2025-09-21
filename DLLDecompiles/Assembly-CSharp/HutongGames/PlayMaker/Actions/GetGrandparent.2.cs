using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC3 RID: 3779
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the parent of a Game Object's parent (the grandparent).")]
	public class GetGrandparent : FsmStateAction
	{
		// Token: 0x06006AC2 RID: 27330 RVA: 0x00214C7D File Offset: 0x00212E7D
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06006AC3 RID: 27331 RVA: 0x00214C90 File Offset: 0x00212E90
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget && ownerDefaultTarget.transform.parent && ownerDefaultTarget.transform.parent.parent)
			{
				this.storeResult.Value = ownerDefaultTarget.transform.parent.parent.gameObject;
			}
			else
			{
				this.storeResult.Value = null;
			}
			base.Finish();
		}

		// Token: 0x04006A04 RID: 27140
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006A05 RID: 27141
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;
	}
}
