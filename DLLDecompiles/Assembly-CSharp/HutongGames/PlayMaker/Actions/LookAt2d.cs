using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD0 RID: 4048
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a 2d or 3d position.")]
	public class LookAt2d : FsmStateAction
	{
		// Token: 0x06006F97 RID: 28567 RVA: 0x00227F58 File Offset: 0x00226158
		public override void Reset()
		{
			this.gameObject = null;
			this.vector2Target = null;
			this.vector3Target = new FsmVector3
			{
				UseVariable = true
			};
			this.debug = false;
			this.debugLineColor = Color.green;
			this.everyFrame = true;
		}

		// Token: 0x06006F98 RID: 28568 RVA: 0x00227FA8 File Offset: 0x002261A8
		public override void OnEnter()
		{
			this.DoLookAt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F99 RID: 28569 RVA: 0x00227FBE File Offset: 0x002261BE
		public override void OnUpdate()
		{
			this.DoLookAt();
		}

		// Token: 0x06006F9A RID: 28570 RVA: 0x00227FC8 File Offset: 0x002261C8
		private void DoLookAt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = new Vector3(this.vector2Target.Value.x, this.vector2Target.Value.y, 0f);
			if (!this.vector3Target.IsNone)
			{
				vector += this.vector3Target.Value;
			}
			Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
			vector2.Normalize();
			float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			ownerDefaultTarget.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
			if (this.debug.Value)
			{
				Debug.DrawLine(ownerDefaultTarget.transform.position, vector, this.debugLineColor.Value);
			}
		}

		// Token: 0x04006F72 RID: 28530
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F73 RID: 28531
		[Tooltip("The 2d position to Look At.")]
		public FsmVector2 vector2Target;

		// Token: 0x04006F74 RID: 28532
		[Tooltip("The 3d position to Look At. If not set to none, will be added to the 2d target")]
		public FsmVector3 vector3Target;

		// Token: 0x04006F75 RID: 28533
		[Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
		public FsmFloat rotationOffset;

		// Token: 0x04006F76 RID: 28534
		[Title("Draw Debug Line")]
		[Tooltip("Draw a debug line from the GameObject to the Target.")]
		public FsmBool debug;

		// Token: 0x04006F77 RID: 28535
		[Tooltip("Color to use for the debug line.")]
		public FsmColor debugLineColor;

		// Token: 0x04006F78 RID: 28536
		[Tooltip("Repeat every frame.")]
		public bool everyFrame = true;
	}
}
