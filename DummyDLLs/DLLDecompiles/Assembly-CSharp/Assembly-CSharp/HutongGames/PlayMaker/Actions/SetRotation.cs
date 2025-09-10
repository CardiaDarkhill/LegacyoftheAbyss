using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E6 RID: 4326
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetRotation : FsmStateAction
	{
		// Token: 0x0600750D RID: 29965 RVA: 0x0023CA68 File Offset: 0x0023AC68
		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.yAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.zAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x0600750E RID: 29966 RVA: 0x0023CAD5 File Offset: 0x0023ACD5
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x0600750F RID: 29967 RVA: 0x0023CAEB File Offset: 0x0023ACEB
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetRotation();
				base.Finish();
			}
		}

		// Token: 0x06007510 RID: 29968 RVA: 0x0023CB09 File Offset: 0x0023AD09
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetRotation();
			}
		}

		// Token: 0x06007511 RID: 29969 RVA: 0x0023CB19 File Offset: 0x0023AD19
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetRotation();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007512 RID: 29970 RVA: 0x0023CB38 File Offset: 0x0023AD38
		private void DoSetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector;
			if (!this.quaternion.IsNone)
			{
				vector = this.quaternion.Value.eulerAngles;
			}
			else if (!this.vector.IsNone)
			{
				vector = this.vector.Value;
			}
			else
			{
				vector = ((this.space == Space.Self) ? ownerDefaultTarget.transform.localEulerAngles : ownerDefaultTarget.transform.eulerAngles);
			}
			if (!this.xAngle.IsNone)
			{
				vector.x = this.xAngle.Value;
			}
			if (!this.yAngle.IsNone)
			{
				vector.y = this.yAngle.Value;
			}
			if (!this.zAngle.IsNone)
			{
				vector.z = this.zAngle.Value;
			}
			if (this.space == Space.Self)
			{
				ownerDefaultTarget.transform.localEulerAngles = vector;
				return;
			}
			ownerDefaultTarget.transform.eulerAngles = vector;
		}

		// Token: 0x04007565 RID: 30053
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007566 RID: 30054
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored quaternion, or vector angles below.")]
		public FsmQuaternion quaternion;

		// Token: 0x04007567 RID: 30055
		[UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
		[Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
		public FsmVector3 vector;

		// Token: 0x04007568 RID: 30056
		[Tooltip("Angle around the X axis in degrees.")]
		public FsmFloat xAngle;

		// Token: 0x04007569 RID: 30057
		[Tooltip("Angle around the Y axis in degrees.")]
		public FsmFloat yAngle;

		// Token: 0x0400756A RID: 30058
		[Tooltip("Angle around the Z axis in degrees.")]
		public FsmFloat zAngle;

		// Token: 0x0400756B RID: 30059
		[Tooltip("Use local or world space.")]
		public Space space;

		// Token: 0x0400756C RID: 30060
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400756D RID: 30061
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
