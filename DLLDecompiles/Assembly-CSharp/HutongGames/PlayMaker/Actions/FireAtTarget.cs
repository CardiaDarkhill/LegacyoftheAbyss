using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3B RID: 3131
	[ActionCategory("Enemy AI")]
	[Tooltip("Travel in a straight line towards target at set speed.")]
	public class FireAtTarget : RigidBody2dActionBase
	{
		// Token: 0x06005F28 RID: 24360 RVA: 0x001E2C41 File Offset: 0x001E0E41
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06005F29 RID: 24361 RVA: 0x001E2C6A File Offset: 0x001E0E6A
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005F2A RID: 24362 RVA: 0x001E2C78 File Offset: 0x001E0E78
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005F2B RID: 24363 RVA: 0x001E2C88 File Offset: 0x001E0E88
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			if (this.self == null || this.rb2d == null || this.target.Value == null)
			{
				base.Finish();
				return;
			}
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F2C RID: 24364 RVA: 0x001E2D0C File Offset: 0x001E0F0C
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F2D RID: 24365 RVA: 0x001E2D24 File Offset: 0x001E0F24
		private void DoSetVelocity()
		{
			if (this.rb2d == null || this.target.Value == null)
			{
				return;
			}
			float num = this.target.Value.transform.position.y + this.position.Value.y - this.self.Value.transform.position.y;
			float num2 = this.target.Value.transform.position.x + this.position.Value.x - this.self.Value.transform.position.x;
			float num3 = Mathf.Atan2(num, num2) * 57.295776f;
			if (!this.spread.IsNone)
			{
				num3 += Random.Range(-this.spread.Value, this.spread.Value);
			}
			this.x = this.speed.Value * Mathf.Cos(num3 * 0.017453292f);
			this.y = this.speed.Value * Mathf.Sin(num3 * 0.017453292f);
			Vector2 linearVelocity;
			linearVelocity.x = this.x.Value;
			linearVelocity.y = this.y.Value;
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005C1A RID: 23578
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005C1B RID: 23579
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04005C1C RID: 23580
		[RequiredField]
		public FsmFloat speed;

		// Token: 0x04005C1D RID: 23581
		public FsmVector3 position;

		// Token: 0x04005C1E RID: 23582
		public FsmFloat spread;

		// Token: 0x04005C1F RID: 23583
		private FsmGameObject self;

		// Token: 0x04005C20 RID: 23584
		private FsmFloat x;

		// Token: 0x04005C21 RID: 23585
		private FsmFloat y;

		// Token: 0x04005C22 RID: 23586
		public bool everyFrame;
	}
}
