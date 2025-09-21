using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B14 RID: 2836
	[ActionCategory("Physics 2d")]
	[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis. I added the ability to limit speed.")]
	public class AddForce2dV2 : RigidBody2dActionBase
	{
		// Token: 0x0600593C RID: 22844 RVA: 0x001C45A0 File Offset: 0x001C27A0
		public override void Reset()
		{
			this.gameObject = null;
			this.atPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.vector = null;
			this.vector3 = new FsmVector3
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.maxSpeed = null;
			this.maxSpeedX = null;
			this.maxSpeedY = null;
			this.everyFrame = false;
		}

		// Token: 0x0600593D RID: 22845 RVA: 0x001C461F File Offset: 0x001C281F
		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600593E RID: 22846 RVA: 0x001C4633 File Offset: 0x001C2833
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoAddForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600593F RID: 22847 RVA: 0x001C4660 File Offset: 0x001C2860
		public override void OnFixedUpdate()
		{
			this.DoAddForce();
		}

		// Token: 0x06005940 RID: 22848 RVA: 0x001C4668 File Offset: 0x001C2868
		private void DoAddForce()
		{
			if (!this.rb2d)
			{
				return;
			}
			Vector2 force = this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.Value;
			if (!this.vector3.IsNone)
			{
				force.x = this.vector3.Value.x;
				force.y = this.vector3.Value.y;
			}
			if (!this.x.IsNone)
			{
				force.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				force.y = this.y.Value;
			}
			if (!this.atPosition.IsNone)
			{
				this.rb2d.AddForceAtPosition(force, this.atPosition.Value);
			}
			else
			{
				this.rb2d.AddForce(force);
			}
			if (this.maxSpeedX != null)
			{
				Vector2 linearVelocity = this.rb2d.linearVelocity;
				if (linearVelocity.x > this.maxSpeedX.Value)
				{
					linearVelocity = new Vector2(this.maxSpeedX.Value, linearVelocity.y);
				}
				if (linearVelocity.x < -this.maxSpeedX.Value)
				{
					linearVelocity = new Vector2(-this.maxSpeedX.Value, linearVelocity.y);
				}
				this.rb2d.linearVelocity = linearVelocity;
			}
			if (this.maxSpeedY != null)
			{
				Vector2 linearVelocity2 = this.rb2d.linearVelocity;
				if (linearVelocity2.y > this.maxSpeedY.Value)
				{
					linearVelocity2 = new Vector2(linearVelocity2.x, this.maxSpeedY.Value);
				}
				if (linearVelocity2.y < -this.maxSpeedY.Value)
				{
					linearVelocity2 = new Vector2(linearVelocity2.x, -this.maxSpeedY.Value);
				}
				this.rb2d.linearVelocity = linearVelocity2;
			}
			if (this.maxSpeed != null)
			{
				Vector2 linearVelocity3 = this.rb2d.linearVelocity;
				linearVelocity3 = Vector2.ClampMagnitude(linearVelocity3, this.maxSpeed.Value);
				this.rb2d.linearVelocity = linearVelocity3;
			}
		}

		// Token: 0x04005495 RID: 21653
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005496 RID: 21654
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
		public FsmVector2 atPosition;

		// Token: 0x04005497 RID: 21655
		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
		public FsmVector2 vector;

		// Token: 0x04005498 RID: 21656
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04005499 RID: 21657
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x0400549A RID: 21658
		[Tooltip("A Vector3 force to add. z is ignored")]
		public FsmVector3 vector3;

		// Token: 0x0400549B RID: 21659
		public FsmFloat maxSpeed;

		// Token: 0x0400549C RID: 21660
		public FsmFloat maxSpeedX;

		// Token: 0x0400549D RID: 21661
		public FsmFloat maxSpeedY;

		// Token: 0x0400549E RID: 21662
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
