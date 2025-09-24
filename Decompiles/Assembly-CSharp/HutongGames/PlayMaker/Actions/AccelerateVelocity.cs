using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9A RID: 2970
	[ActionCategory("Physics 2d")]
	[Tooltip("Accelerates objects velocity, and clamps top speed")]
	public class AccelerateVelocity : RigidBody2dActionBase
	{
		// Token: 0x06005BDB RID: 23515 RVA: 0x001CE630 File Offset: 0x001CC830
		public override void Reset()
		{
			this.gameObject = null;
			this.xAccel = new FsmFloat
			{
				UseVariable = true
			};
			this.yAccel = new FsmFloat
			{
				UseVariable = true
			};
			this.xMaxSpeed = new FsmFloat
			{
				UseVariable = true
			};
			this.yMaxSpeed = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06005BDC RID: 23516 RVA: 0x001CE68C File Offset: 0x001CC88C
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BDD RID: 23517 RVA: 0x001CE69A File Offset: 0x001CC89A
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
		}

		// Token: 0x06005BDE RID: 23518 RVA: 0x001CE6B3 File Offset: 0x001CC8B3
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BDF RID: 23519 RVA: 0x001CE6C1 File Offset: 0x001CC8C1
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
		}

		// Token: 0x06005BE0 RID: 23520 RVA: 0x001CE6CC File Offset: 0x001CC8CC
		private void DoSetVelocity()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (!this.xAccel.IsNone)
			{
				float num = linearVelocity.x + this.xAccel.Value;
				num = Mathf.Clamp(num, -this.xMaxSpeed.Value, this.xMaxSpeed.Value);
				linearVelocity = new Vector2(num, linearVelocity.y);
			}
			if (!this.yAccel.IsNone)
			{
				float num2 = linearVelocity.y + this.yAccel.Value;
				num2 = Mathf.Clamp(num2, -this.yMaxSpeed.Value, this.yMaxSpeed.Value);
				linearVelocity = new Vector2(linearVelocity.x, num2);
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005741 RID: 22337
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005742 RID: 22338
		public FsmFloat xAccel;

		// Token: 0x04005743 RID: 22339
		public FsmFloat yAccel;

		// Token: 0x04005744 RID: 22340
		public FsmFloat xMaxSpeed;

		// Token: 0x04005745 RID: 22341
		public FsmFloat yMaxSpeed;
	}
}
