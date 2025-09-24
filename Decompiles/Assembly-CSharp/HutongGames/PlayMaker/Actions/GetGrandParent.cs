using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6A RID: 3178
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Parent of a Game Object's Parent")]
	public class GetGrandParent : FsmStateAction
	{
		// Token: 0x06005FFF RID: 24575 RVA: 0x001E6686 File Offset: 0x001E4886
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x001E6698 File Offset: 0x001E4898
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget && ownerDefaultTarget.transform.parent)
			{
				this.storeResult.Value = ((ownerDefaultTarget.transform.parent.parent == null) ? null : ownerDefaultTarget.transform.parent.parent.gameObject);
			}
			else
			{
				this.storeResult.Value = null;
			}
			base.Finish();
		}

		// Token: 0x04005D51 RID: 23889
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D52 RID: 23890
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;
	}
}
