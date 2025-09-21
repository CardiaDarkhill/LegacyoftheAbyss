using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1D RID: 3101
	[ActionCategory("Enemy AI")]
	[Tooltip("Flies and keeps a certain distance from target, with smoother movement")]
	public class DistanceFlySmooth : RigidBody2dActionBase
	{
		// Token: 0x06005E6D RID: 24173 RVA: 0x001DC96B File Offset: 0x001DAB6B
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.accelerationForce = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005E6E RID: 24174 RVA: 0x001DC99B File Offset: 0x001DAB9B
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E6F RID: 24175 RVA: 0x001DC9A9 File Offset: 0x001DABA9
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E70 RID: 24176 RVA: 0x001DC9B7 File Offset: 0x001DABB7
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoChase();
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x001DC9F2 File Offset: 0x001DABF2
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x06005E72 RID: 24178 RVA: 0x001DC9FC File Offset: 0x001DABFC
		private void DoChase()
		{
			if (this.rb2d == null || this.self.Value == null || this.target.Value == null)
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

		// Token: 0x04005AC4 RID: 23236
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005AC5 RID: 23237
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005AC6 RID: 23238
		public FsmFloat distance;

		// Token: 0x04005AC7 RID: 23239
		public FsmFloat speedMax;

		// Token: 0x04005AC8 RID: 23240
		public FsmFloat accelerationForce;

		// Token: 0x04005AC9 RID: 23241
		public FsmFloat targetRadius;

		// Token: 0x04005ACA RID: 23242
		public FsmFloat deceleration;

		// Token: 0x04005ACB RID: 23243
		public FsmVector3 offset;

		// Token: 0x04005ACC RID: 23244
		private float distanceAway;

		// Token: 0x04005ACD RID: 23245
		private FsmGameObject self;
	}
}
