using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FDB RID: 4059
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the various properties of a HingeJoint2d component")]
	public class SetHingeJoint2dProperties : FsmStateAction
	{
		// Token: 0x06006FCD RID: 28621 RVA: 0x00229054 File Offset: 0x00227254
		public override void Reset()
		{
			this.useLimits = new FsmBool
			{
				UseVariable = true
			};
			this.min = new FsmFloat
			{
				UseVariable = true
			};
			this.max = new FsmFloat
			{
				UseVariable = true
			};
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
			this.everyFrame = false;
		}

		// Token: 0x06006FCE RID: 28622 RVA: 0x002290D4 File Offset: 0x002272D4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._joint = ownerDefaultTarget.GetComponent<HingeJoint2D>();
				if (this._joint != null)
				{
					this._motor = this._joint.motor;
					this._limits = this._joint.limits;
				}
			}
			this.SetProperties();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FCF RID: 28623 RVA: 0x0022914C File Offset: 0x0022734C
		public override void OnUpdate()
		{
			this.SetProperties();
		}

		// Token: 0x06006FD0 RID: 28624 RVA: 0x00229154 File Offset: 0x00227354
		private void SetProperties()
		{
			if (this._joint == null)
			{
				return;
			}
			if (!this.useMotor.IsNone)
			{
				this._joint.useMotor = this.useMotor.Value;
			}
			if (!this.motorSpeed.IsNone)
			{
				this._motor.motorSpeed = this.motorSpeed.Value;
				this._joint.motor = this._motor;
			}
			if (!this.maxMotorTorque.IsNone)
			{
				this._motor.maxMotorTorque = this.maxMotorTorque.Value;
				this._joint.motor = this._motor;
			}
			if (!this.useLimits.IsNone)
			{
				this._joint.useLimits = this.useLimits.Value;
			}
			if (!this.min.IsNone)
			{
				this._limits.min = this.min.Value;
				this._joint.limits = this._limits;
			}
			if (!this.max.IsNone)
			{
				this._limits.max = this.max.Value;
				this._joint.limits = this._limits;
			}
		}

		// Token: 0x04006FC4 RID: 28612
		[RequiredField]
		[Tooltip("The HingeJoint2d target")]
		[CheckForComponent(typeof(HingeJoint2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FC5 RID: 28613
		[ActionSection("Limits")]
		[Tooltip("Should limits be placed on the range of rotation?")]
		public FsmBool useLimits;

		// Token: 0x04006FC6 RID: 28614
		[Tooltip("Lower angular limit of rotation.")]
		public FsmFloat min;

		// Token: 0x04006FC7 RID: 28615
		[Tooltip("Upper angular limit of rotation")]
		public FsmFloat max;

		// Token: 0x04006FC8 RID: 28616
		[ActionSection("Motor")]
		[Tooltip("Should a motor force be applied automatically to the Rigidbody2D?")]
		public FsmBool useMotor;

		// Token: 0x04006FC9 RID: 28617
		[Tooltip("The desired speed for the Rigidbody2D to reach as it moves with the joint.")]
		public FsmFloat motorSpeed;

		// Token: 0x04006FCA RID: 28618
		[Tooltip("The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.")]
		public FsmFloat maxMotorTorque;

		// Token: 0x04006FCB RID: 28619
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006FCC RID: 28620
		private HingeJoint2D _joint;

		// Token: 0x04006FCD RID: 28621
		private JointMotor2D _motor;

		// Token: 0x04006FCE RID: 28622
		private JointAngleLimits2D _limits;
	}
}
