using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010DA RID: 4314
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Rotation of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetRotation : FsmStateAction
	{
		// Token: 0x060074C4 RID: 29892 RVA: 0x0023B83B File Offset: 0x00239A3B
		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = null;
			this.yAngle = null;
			this.zAngle = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x060074C5 RID: 29893 RVA: 0x0023B875 File Offset: 0x00239A75
		public override void OnEnter()
		{
			this.DoGetRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074C6 RID: 29894 RVA: 0x0023B88B File Offset: 0x00239A8B
		public override void OnUpdate()
		{
			this.DoGetRotation();
		}

		// Token: 0x060074C7 RID: 29895 RVA: 0x0023B894 File Offset: 0x00239A94
		private void DoGetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.space == Space.World)
			{
				this.quaternion.Value = ownerDefaultTarget.transform.rotation;
				Vector3 eulerAngles = ownerDefaultTarget.transform.eulerAngles;
				this.vector.Value = eulerAngles;
				this.xAngle.Value = eulerAngles.x;
				this.yAngle.Value = eulerAngles.y;
				this.zAngle.Value = eulerAngles.z;
				return;
			}
			Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
			this.quaternion.Value = Quaternion.Euler(localEulerAngles);
			this.vector.Value = localEulerAngles;
			this.xAngle.Value = localEulerAngles.x;
			this.yAngle.Value = localEulerAngles.y;
			this.zAngle.Value = localEulerAngles.z;
		}

		// Token: 0x0400750F RID: 29967
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007510 RID: 29968
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the rotation as a Quaternion.")]
		public FsmQuaternion quaternion;

		// Token: 0x04007511 RID: 29969
		[UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
		[Tooltip("Get the rotation as Euler angles (rotation around each axis) and store in a Vector3 Variable.")]
		public FsmVector3 vector;

		// Token: 0x04007512 RID: 29970
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the angle around the X axis.")]
		public FsmFloat xAngle;

		// Token: 0x04007513 RID: 29971
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the angle around the Y axis.")]
		public FsmFloat yAngle;

		// Token: 0x04007514 RID: 29972
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the angle around the Z axis.")]
		public FsmFloat zAngle;

		// Token: 0x04007515 RID: 29973
		[Tooltip("The coordinate space to get the rotation in.")]
		public Space space;

		// Token: 0x04007516 RID: 29974
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
