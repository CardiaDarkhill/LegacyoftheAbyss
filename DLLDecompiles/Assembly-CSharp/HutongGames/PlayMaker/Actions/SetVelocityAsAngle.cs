using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5E RID: 3422
	[ActionCategory("Physics 2d")]
	[Tooltip("Sets the 2d Velocity of a Game Object, using an angle and a speed value. For the angle, 0 is to the right and the degrees increase clockwise.")]
	public class SetVelocityAsAngle : RigidBody2dActionBase
	{
		// Token: 0x06006417 RID: 25623 RVA: 0x001F88A2 File Offset: 0x001F6AA2
		public override void Reset()
		{
			this.gameObject = null;
			this.angle = new FsmFloat
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006418 RID: 25624 RVA: 0x001F88D6 File Offset: 0x001F6AD6
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006419 RID: 25625 RVA: 0x001F88E4 File Offset: 0x001F6AE4
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x001F88F2 File Offset: 0x001F6AF2
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600641B RID: 25627 RVA: 0x001F891F File Offset: 0x001F6B1F
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600641C RID: 25628 RVA: 0x001F8938 File Offset: 0x001F6B38
		private void DoSetVelocity()
		{
			if (this.rb2d == null)
			{
				return;
			}
			float x = this.speed.Value * Mathf.Cos(this.angle.Value * 0.017453292f);
			float y = this.speed.Value * Mathf.Sin(this.angle.Value * 0.017453292f);
			Vector2 linearVelocity;
			linearVelocity.x = x;
			linearVelocity.y = y;
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04006286 RID: 25222
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006287 RID: 25223
		[RequiredField]
		public FsmFloat angle;

		// Token: 0x04006288 RID: 25224
		[RequiredField]
		public FsmFloat speed;

		// Token: 0x04006289 RID: 25225
		public bool everyFrame;
	}
}
