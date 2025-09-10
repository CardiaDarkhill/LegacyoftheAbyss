using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C56 RID: 3158
	[ActionCategory(ActionCategory.GameObject)]
	public class GetActiveChild : FsmStateAction
	{
		// Token: 0x06005FA0 RID: 24480 RVA: 0x001E55D6 File Offset: 0x001E37D6
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06005FA1 RID: 24481 RVA: 0x001E55E6 File Offset: 0x001E37E6
		public override void OnEnter()
		{
			this.DoGetChild();
			base.Finish();
		}

		// Token: 0x06005FA2 RID: 24482 RVA: 0x001E55F4 File Offset: 0x001E37F4
		private void DoGetChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget.transform.childCount == 0)
			{
				return;
			}
			foreach (object obj in ownerDefaultTarget.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.gameObject.activeSelf)
				{
					this.storeResult.Value = transform.gameObject;
				}
			}
		}

		// Token: 0x04005CF6 RID: 23798
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005CF7 RID: 23799
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;
	}
}
