using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE2 RID: 4066
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the various properties of a WheelJoint2d component")]
	public class SetWheelJoint2dProperties : FsmStateAction
	{
		// Token: 0x06006FF3 RID: 28659 RVA: 0x00229A30 File Offset: 0x00227C30
		public override void Reset()
		{
			this.useMotor = new FsmBool
			{
				UseVariable = true
			};
			this.motorSpeed = new FsmFloat
			{
				UseVariable = true
			};
			this.maxMotorTorque = new FsmFloat
			{
				UseVariable = true
			};
			this.angle = new FsmFloat
			{
				UseVariable = true
			};
			this.dampingRatio = new FsmFloat
			{
				UseVariable = true
			};
			this.frequency = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006FF4 RID: 28660 RVA: 0x00229AB0 File Offset: 0x00227CB0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._wj2d = ownerDefaultTarget.GetComponent<WheelJoint2D>();
				if (this._wj2d != null)
				{
					this._motor = this._wj2d.motor;
					this._suspension = this._wj2d.suspension;
				}
			}
			this.SetProperties();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FF5 RID: 28661 RVA: 0x00229B28 File Offset: 0x00227D28
		public override void OnUpdate()
		{
			this.SetProperties();
		}

		// Token: 0x06006FF6 RID: 28662 RVA: 0x00229B30 File Offset: 0x00227D30
		private void SetProperties()
		{
			if (this._wj2d == null)
			{
				return;
			}
			if (!this.useMotor.IsNone)
			{
				this._wj2d.useMotor = this.useMotor.Value;
			}
			if (!this.motorSpeed.IsNone)
			{
				this._motor.motorSpeed = this.motorSpeed.Value;
				this._wj2d.motor = this._motor;
			}
			if (!this.maxMotorTorque.IsNone)
			{
				this._motor.maxMotorTorque = this.maxMotorTorque.Value;
				this._wj2d.motor = this._motor;
			}
			if (!this.angle.IsNone)
			{
				this._suspension.angle = this.angle.Value;
				this._wj2d.suspension = this._suspension;
			}
			if (!this.dampingRatio.IsNone)
			{
				this._suspension.dampingRatio = this.dampingRatio.Value;
				this._wj2d.suspension = this._suspension;
			}
			if (!this.frequency.IsNone)
			{
				this._suspension.frequency = this.frequency.Value;
				this._wj2d.suspension = this._suspension;
			}
		}

		// Token: 0x04006FEB RID: 28651
		[RequiredField]
		[Tooltip("The WheelJoint2d target")]
		[CheckForComponent(typeof(WheelJoint2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FEC RID: 28652
		[ActionSection("Motor")]
		[Tooltip("Should a motor force be applied automatically to the Rigidbody2D?")]
		public FsmBool useMotor;

		// Token: 0x04006FED RID: 28653
		[Tooltip("The desired speed for the Rigidbody2D to reach as it moves with the joint.")]
		public FsmFloat motorSpeed;

		// Token: 0x04006FEE RID: 28654
		[Tooltip("The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.")]
		public FsmFloat maxMotorTorque;

		// Token: 0x04006FEF RID: 28655
		[ActionSection("Suspension")]
		[Tooltip("The world angle along which the suspension will move. This provides 2D constrained motion similar to a SliderJoint2D. This is typically how suspension works in the real world.")]
		public FsmFloat angle;

		// Token: 0x04006FF0 RID: 28656
		[Tooltip("The amount by which the suspension spring force is reduced in proportion to the movement speed.")]
		public FsmFloat dampingRatio;

		// Token: 0x04006FF1 RID: 28657
		[Tooltip("The frequency at which the suspension spring oscillates.")]
		public FsmFloat frequency;

		// Token: 0x04006FF2 RID: 28658
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006FF3 RID: 28659
		private WheelJoint2D _wj2d;

		// Token: 0x04006FF4 RID: 28660
		private JointMotor2D _motor;

		// Token: 0x04006FF5 RID: 28661
		private JointSuspension2D _suspension;
	}
}
