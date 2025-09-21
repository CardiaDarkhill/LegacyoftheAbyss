using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E2 RID: 4322
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a Game Object around each Axis. Use a Vector3 Variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class Rotate : FsmStateAction
	{
		// Token: 0x060074F3 RID: 29939 RVA: 0x0023C3DC File Offset: 0x0023A5DC
		public override void Reset()
		{
			this.gameObject = null;
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
			this.space = Space.Self;
			this.perSecond = false;
			this.everyFrame = true;
			this.lateUpdate = false;
			this.fixedUpdate = false;
		}

		// Token: 0x060074F4 RID: 29940 RVA: 0x0023C450 File Offset: 0x0023A650
		public override void OnPreprocess()
		{
			if (this.fixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
			}
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x060074F5 RID: 29941 RVA: 0x0023C47A File Offset: 0x0023A67A
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate && !this.fixedUpdate)
			{
				this.DoRotate();
				base.Finish();
			}
		}

		// Token: 0x060074F6 RID: 29942 RVA: 0x0023C4A0 File Offset: 0x0023A6A0
		public override void OnUpdate()
		{
			if (!this.lateUpdate && !this.fixedUpdate)
			{
				this.DoRotate();
			}
		}

		// Token: 0x060074F7 RID: 29943 RVA: 0x0023C4B8 File Offset: 0x0023A6B8
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoRotate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074F8 RID: 29944 RVA: 0x0023C4D6 File Offset: 0x0023A6D6
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.DoRotate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074F9 RID: 29945 RVA: 0x0023C4F4 File Offset: 0x0023A6F4
		private void DoRotate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = this.vector.IsNone ? new Vector3(this.xAngle.Value, this.yAngle.Value, this.zAngle.Value) : this.vector.Value;
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
			if (!this.perSecond)
			{
				ownerDefaultTarget.transform.Rotate(vector, this.space);
				return;
			}
			ownerDefaultTarget.transform.Rotate(vector * Time.deltaTime, this.space);
		}

		// Token: 0x04007548 RID: 30024
		[RequiredField]
		[Tooltip("The game object to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007549 RID: 30025
		[Tooltip("A rotation vector specifying rotation around x, y, and z axis. NOTE: You can override individual axis below.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		// Token: 0x0400754A RID: 30026
		[Tooltip("Rotation around x axis.")]
		public FsmFloat xAngle;

		// Token: 0x0400754B RID: 30027
		[Tooltip("Rotation around y axis.")]
		public FsmFloat yAngle;

		// Token: 0x0400754C RID: 30028
		[Tooltip("Rotation around z axis.")]
		public FsmFloat zAngle;

		// Token: 0x0400754D RID: 30029
		[Tooltip("Rotate in local or world space.")]
		public Space space;

		// Token: 0x0400754E RID: 30030
		[Tooltip("Rotation is specified in degrees per second. In other words, the amount to rotate in over one second. This allows rotations to be frame rate independent. It is the same as multiplying the rotation by Time.deltaTime.")]
		public bool perSecond;

		// Token: 0x0400754F RID: 30031
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007550 RID: 30032
		[Tooltip("Perform the rotation in LateUpdate. This is useful if you want to override the rotation of objects that are animated or otherwise rotated in Update.")]
		public bool lateUpdate;

		// Token: 0x04007551 RID: 30033
		[Tooltip("Perform the rotation in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;
	}
}
