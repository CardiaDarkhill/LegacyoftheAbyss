using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC0 RID: 3776
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the Parent of a Game Object.")]
	public class SetParent : FsmStateAction
	{
		// Token: 0x06006AB7 RID: 27319 RVA: 0x002149D2 File Offset: 0x00212BD2
		public override void Reset()
		{
			this.gameObject = null;
			this.parent = null;
			this.resetLocalPosition = null;
			this.resetLocalRotation = null;
		}

		// Token: 0x06006AB8 RID: 27320 RVA: 0x002149F0 File Offset: 0x00212BF0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.transform.parent = ((this.parent.Value == null) ? null : this.parent.Value.transform);
				if (this.resetLocalPosition.Value)
				{
					ownerDefaultTarget.transform.localPosition = Vector3.zero;
				}
				if (this.resetLocalRotation.Value)
				{
					ownerDefaultTarget.transform.localRotation = Quaternion.identity;
				}
			}
			base.Finish();
		}

		// Token: 0x040069FA RID: 27130
		[RequiredField]
		[Tooltip("The Game Object to parent.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069FB RID: 27131
		[Tooltip("The new parent for the Game Object.")]
		public FsmGameObject parent;

		// Token: 0x040069FC RID: 27132
		[Tooltip("Set the local position to 0,0,0 after parenting.")]
		public FsmBool resetLocalPosition;

		// Token: 0x040069FD RID: 27133
		[Tooltip("Set the local rotation to 0,0,0 after parenting.")]
		public FsmBool resetLocalRotation;
	}
}
