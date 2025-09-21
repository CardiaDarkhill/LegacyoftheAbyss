using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010EC RID: 4332
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms a Direction from a Game Object's local space to world space.")]
	public class TransformDirection : FsmStateAction
	{
		// Token: 0x06007534 RID: 30004 RVA: 0x0023D807 File Offset: 0x0023BA07
		public override void Reset()
		{
			this.gameObject = null;
			this.localDirection = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06007535 RID: 30005 RVA: 0x0023D825 File Offset: 0x0023BA25
		public override void OnEnter()
		{
			this.DoTransformDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007536 RID: 30006 RVA: 0x0023D83B File Offset: 0x0023BA3B
		public override void OnUpdate()
		{
			this.DoTransformDirection();
		}

		// Token: 0x06007537 RID: 30007 RVA: 0x0023D844 File Offset: 0x0023BA44
		private void DoTransformDirection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.TransformDirection(this.localDirection.Value);
		}

		// Token: 0x040075A5 RID: 30117
		[RequiredField]
		[Tooltip("The Game Object that defines local space.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040075A6 RID: 30118
		[RequiredField]
		[Tooltip("A direction vector in the object's local space.")]
		public FsmVector3 localDirection;

		// Token: 0x040075A7 RID: 30119
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the transformed direction vector, now in world space, in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		// Token: 0x040075A8 RID: 30120
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
