using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C15 RID: 3093
	[ActionCategory("Enemy AI")]
	[Tooltip("Flies and keeps a certain distance from target, with smoother movement")]
	public class DirectlyFlyTo : RigidBody2dActionBase
	{
		// Token: 0x06005E47 RID: 24135 RVA: 0x001DB54F File Offset: 0x001D974F
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.targetRadius = 0.1f;
		}

		// Token: 0x06005E48 RID: 24136 RVA: 0x001DB56F File Offset: 0x001D976F
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoFlyTo();
		}

		// Token: 0x06005E49 RID: 24137 RVA: 0x001DB5AA File Offset: 0x001D97AA
		public override void OnUpdate()
		{
			this.DoFlyTo();
		}

		// Token: 0x06005E4A RID: 24138 RVA: 0x001DB5B4 File Offset: 0x001D97B4
		private void DoFlyTo()
		{
			if (this.rb2d == null)
			{
				return;
			}
			float num = Mathf.Sqrt(Mathf.Pow(this.self.Value.transform.position.x - (this.target.Value.transform.position.x + this.offset.Value.x), 2f) + Mathf.Pow(this.self.Value.transform.position.y - (this.target.Value.transform.position.y + this.offset.Value.y), 2f));
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (num <= -this.targetRadius.Value || num >= this.targetRadius.Value)
			{
				float y = this.target.Value.transform.position.y + this.offset.Value.y - this.self.Value.transform.position.y;
				float x = this.target.Value.transform.position.x + this.offset.Value.x - this.self.Value.transform.position.x;
				float num2;
				for (num2 = Mathf.Atan2(y, x) * 57.295776f; num2 < 0f; num2 += 360f)
				{
				}
				float num3;
				if (num > this.maxSpeedDistance.Value)
				{
					num3 = this.maxSpeed.Value;
				}
				else
				{
					float num4 = num / this.maxSpeedDistance.Value;
					num3 = this.maxSpeed.Value * num4;
					if (num3 < this.minSpeed.Value)
					{
						num3 = this.minSpeed.Value;
					}
				}
				float x2 = num3 * Mathf.Cos(num2 * 0.017453292f);
				float y2 = num3 * Mathf.Sin(num2 * 0.017453292f);
				linearVelocity.x = x2;
				linearVelocity.y = y2;
				this.rb2d.linearVelocity = linearVelocity;
				return;
			}
			linearVelocity = new Vector2(0f, 0f);
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005A93 RID: 23187
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A94 RID: 23188
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005A95 RID: 23189
		public FsmFloat minSpeed;

		// Token: 0x04005A96 RID: 23190
		public FsmFloat maxSpeed;

		// Token: 0x04005A97 RID: 23191
		public FsmFloat maxSpeedDistance;

		// Token: 0x04005A98 RID: 23192
		public FsmFloat targetRadius;

		// Token: 0x04005A99 RID: 23193
		public FsmVector3 offset;

		// Token: 0x04005A9A RID: 23194
		private FsmGameObject self;
	}
}
