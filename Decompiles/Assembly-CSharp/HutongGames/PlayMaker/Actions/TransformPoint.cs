using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010ED RID: 4333
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms a Position from a Game Object's local space to world space.")]
	public class TransformPoint : FsmStateAction
	{
		// Token: 0x06007539 RID: 30009 RVA: 0x0023D896 File Offset: 0x0023BA96
		public override void Reset()
		{
			this.gameObject = null;
			this.localPosition = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600753A RID: 30010 RVA: 0x0023D8B4 File Offset: 0x0023BAB4
		public override void OnEnter()
		{
			this.DoTransformPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600753B RID: 30011 RVA: 0x0023D8CA File Offset: 0x0023BACA
		public override void OnUpdate()
		{
			this.DoTransformPoint();
		}

		// Token: 0x0600753C RID: 30012 RVA: 0x0023D8D4 File Offset: 0x0023BAD4
		private void DoTransformPoint()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.TransformPoint(this.localPosition.Value);
		}

		// Token: 0x040075A9 RID: 30121
		[RequiredField]
		[Tooltip("The Game Object that defines local space.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040075AA RID: 30122
		[RequiredField]
		[Tooltip("A local position vector.")]
		public FsmVector3 localPosition;

		// Token: 0x040075AB RID: 30123
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the transformed position, now in world space, in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		// Token: 0x040075AC RID: 30124
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
