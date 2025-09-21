using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB4 RID: 3764
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Parent of a Game Object.")]
	public class GetParent : FsmStateAction
	{
		// Token: 0x06006A84 RID: 27268 RVA: 0x00214307 File Offset: 0x00212507
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06006A85 RID: 27269 RVA: 0x00214318 File Offset: 0x00212518
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this.storeResult.Value = ((ownerDefaultTarget.transform.parent == null) ? null : ownerDefaultTarget.transform.parent.gameObject);
			}
			else
			{
				this.storeResult.Value = null;
			}
			base.Finish();
		}

		// Token: 0x040069D7 RID: 27095
		[RequiredField]
		[Tooltip("The Game Object to find the parent of.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069D8 RID: 27096
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the parent object (or null if no parent) in a variable.")]
		public FsmGameObject storeResult;
	}
}
