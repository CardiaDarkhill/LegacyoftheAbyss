using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D69 RID: 3433
	[ActionCategory("Enemy AI")]
	[Tooltip("Flies and keeps a certain distance from target, with smoother movement")]
	public class SmoothFlyTo : RigidBody2dActionBase
	{
		// Token: 0x06006452 RID: 25682 RVA: 0x001F99BE File Offset: 0x001F7BBE
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.accelerationForce = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06006453 RID: 25683 RVA: 0x001F99EE File Offset: 0x001F7BEE
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006454 RID: 25684 RVA: 0x001F99FC File Offset: 0x001F7BFC
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x001F9A0A File Offset: 0x001F7C0A
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoChase();
		}

		// Token: 0x06006456 RID: 25686 RVA: 0x001F9A45 File Offset: 0x001F7C45
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x06006457 RID: 25687 RVA: 0x001F9A50 File Offset: 0x001F7C50
		private void DoChase()
		{
			if (this.rb2d == null)
			{
				return;
			}
			this.distanceAway = Mathf.Sqrt(Mathf.Pow(this.self.Value.transform.position.x - (this.target.Value.transform.position.x + this.offset.Value.x), 2f) + Mathf.Pow(this.self.Value.transform.position.y - (this.target.Value.transform.position.y + this.offset.Value.y), 2f));
			Vector2 vector = this.rb2d.linearVelocity;
			if (this.distanceAway <= this.distance.Value - this.targetRadius.Value || this.distanceAway >= this.distance.Value + this.targetRadius.Value)
			{
				Vector2 vector2 = new Vector2(this.target.Value.transform.position.x + this.offset.Value.x - this.self.Value.transform.position.x, this.target.Value.transform.position.y + this.offset.Value.y - this.self.Value.transform.position.y);
				vector2 = Vector2.ClampMagnitude(vector2, 1f);
				vector2 = new Vector2(vector2.x * this.accelerationForce.Value, vector2.y * this.accelerationForce.Value);
				if (this.distanceAway < this.distance.Value)
				{
					vector2 = new Vector2(-vector2.x, -vector2.y);
				}
				this.rb2d.AddForce(vector2);
				vector = Vector2.ClampMagnitude(vector, this.speedMax.Value);
				this.rb2d.linearVelocity = vector;
				return;
			}
			vector = this.rb2d.linearVelocity;
			if (vector.x < 0f)
			{
				vector.x *= this.deceleration.Value;
				if (vector.x > 0f)
				{
					vector.x = 0f;
				}
			}
			else if (vector.x > 0f)
			{
				vector.x *= this.deceleration.Value;
				if (vector.x < 0f)
				{
					vector.x = 0f;
				}
			}
			if (vector.y < 0f)
			{
				vector.y *= this.deceleration.Value;
				if (vector.y > 0f)
				{
					vector.y = 0f;
				}
			}
			else if (vector.y > 0f)
			{
				vector.y *= this.deceleration.Value;
				if (vector.y < 0f)
				{
					vector.y = 0f;
				}
			}
			this.rb2d.linearVelocity = vector;
		}

		// Token: 0x040062CF RID: 25295
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040062D0 RID: 25296
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040062D1 RID: 25297
		public FsmFloat distance;

		// Token: 0x040062D2 RID: 25298
		public FsmFloat speedMax;

		// Token: 0x040062D3 RID: 25299
		public FsmFloat accelerationForce;

		// Token: 0x040062D4 RID: 25300
		public FsmFloat targetRadius;

		// Token: 0x040062D5 RID: 25301
		public FsmFloat deceleration;

		// Token: 0x040062D6 RID: 25302
		public FsmVector3 offset;

		// Token: 0x040062D7 RID: 25303
		private float distanceAway;

		// Token: 0x040062D8 RID: 25304
		private FsmGameObject self;
	}
}
