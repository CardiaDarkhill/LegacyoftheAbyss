using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D99 RID: 3481
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the Parent of a Game Object. It uses the Transform.SetParent method")]
	public class SetTransformParent : FsmStateAction
	{
		// Token: 0x06006527 RID: 25895 RVA: 0x001FE5E3 File Offset: 0x001FC7E3
		public override void Reset()
		{
			this.gameObject = null;
			this.parent = null;
			this.worldPositionStays = true;
		}

		// Token: 0x06006528 RID: 25896 RVA: 0x001FE600 File Offset: 0x001FC800
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			GameObject value = this.parent.Value;
			Transform transform = null;
			if (value != null)
			{
				transform = value.transform;
			}
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.transform.SetParent(transform, this.worldPositionStays.Value);
			}
			base.Finish();
		}

		// Token: 0x04006422 RID: 25634
		[RequiredField]
		[Tooltip("The Game Object to parent.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006423 RID: 25635
		[Tooltip("The new parent for the Game Object.")]
		public FsmGameObject parent;

		// Token: 0x04006424 RID: 25636
		[Tooltip("If true, the parent-relative position, scale and rotation is modified such that the object keeps the same world space position, rotation and scale as before.")]
		public FsmBool worldPositionStays;
	}
}
