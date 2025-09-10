using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA0 RID: 2976
	[ActionCategory("Physics 2d")]
	[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis. I added the ability to limit speed.")]
	public class AddForce2dAsAngle : RigidBody2dActionBase
	{
		// Token: 0x06005BF6 RID: 23542 RVA: 0x001CEB7C File Offset: 0x001CCD7C
		public override void Reset()
		{
			this.gameObject = null;
			this.atPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.angle = null;
			this.speed = null;
			this.maxSpeed = null;
			this.maxSpeedX = null;
			this.maxSpeedY = null;
			this.everyFrame = false;
		}

		// Token: 0x06005BF7 RID: 23543 RVA: 0x001CEBCC File Offset: 0x001CCDCC
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BF8 RID: 23544 RVA: 0x001CEBDA File Offset: 0x001CCDDA
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BF9 RID: 23545 RVA: 0x001CEBE8 File Offset: 0x001CCDE8
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoAddForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005BFA RID: 23546 RVA: 0x001CEC15 File Offset: 0x001CCE15
		public override void OnFixedUpdate()
		{
			this.DoAddForce();
		}

		// Token: 0x06005BFB RID: 23547 RVA: 0x001CEC20 File Offset: 0x001CCE20
		private void DoAddForce()
		{
			this.x = this.speed.Value * Mathf.Cos(this.angle.Value * 0.017453292f);
			this.y = this.speed.Value * Mathf.Sin(this.angle.Value * 0.017453292f);
			if (!this.rb2d)
			{
				return;
			}
			Vector2 force = new Vector2(this.x, this.y);
			if (!this.atPosition.IsNone)
			{
				this.rb2d.AddForceAtPosition(force, this.atPosition.Value, this.forceMode);
			}
			else
			{
				this.rb2d.AddForce(force, this.forceMode);
			}
			if (!this.maxSpeedX.IsNone)
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
			if (!this.maxSpeedY.IsNone)
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
			if (!this.maxSpeed.IsNone)
			{
				Vector2 vector = this.rb2d.linearVelocity;
				vector = Vector2.ClampMagnitude(vector, this.maxSpeed.Value);
				this.rb2d.linearVelocity = vector;
			}
		}

		// Token: 0x04005757 RID: 22359
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005758 RID: 22360
		[Tooltip("Option for applying the force")]
		public ForceMode2D forceMode;

		// Token: 0x04005759 RID: 22361
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
		public FsmVector2 atPosition;

		// Token: 0x0400575A RID: 22362
		[RequiredField]
		public FsmFloat angle;

		// Token: 0x0400575B RID: 22363
		[RequiredField]
		public FsmFloat speed;

		// Token: 0x0400575C RID: 22364
		private float x;

		// Token: 0x0400575D RID: 22365
		private float y;

		// Token: 0x0400575E RID: 22366
		public FsmFloat maxSpeed;

		// Token: 0x0400575F RID: 22367
		public FsmFloat maxSpeedX;

		// Token: 0x04005760 RID: 22368
		public FsmFloat maxSpeedY;

		// Token: 0x04005761 RID: 22369
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
