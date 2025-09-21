using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1B RID: 3099
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceFly : RigidBody2dActionBase
	{
		// Token: 0x06005E5F RID: 24159 RVA: 0x001DBED8 File Offset: 0x001DA0D8
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.targetsHeight = false;
			this.height = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
			this.minAboveHero = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x001DBF33 File Offset: 0x001DA133
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E61 RID: 24161 RVA: 0x001DBF41 File Offset: 0x001DA141
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x001DBF4F File Offset: 0x001DA14F
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoBuzz();
		}

		// Token: 0x06005E63 RID: 24163 RVA: 0x001DBF8A File Offset: 0x001DA18A
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06005E64 RID: 24164 RVA: 0x001DBF94 File Offset: 0x001DA194
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			this.distanceAway = Mathf.Sqrt(Mathf.Pow(this.self.Value.transform.position.x - this.target.Value.transform.position.x, 2f) + Mathf.Pow(this.self.Value.transform.position.y - this.target.Value.transform.position.y, 2f));
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.distanceAway > this.distance.Value)
			{
				if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				if (!this.targetsHeight && this.minAboveHero.IsNone)
				{
					if (this.self.Value.transform.position.y < this.target.Value.transform.position.y)
					{
						linearVelocity.y += this.acceleration.Value;
					}
					else
					{
						linearVelocity.y -= this.acceleration.Value;
					}
				}
			}
			else
			{
				if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else
				{
					linearVelocity.x += this.acceleration.Value;
				}
				if (!this.targetsHeight && this.minAboveHero.IsNone)
				{
					if (this.self.Value.transform.position.y < this.target.Value.transform.position.y)
					{
						linearVelocity.y -= this.acceleration.Value;
					}
					else
					{
						linearVelocity.y += this.acceleration.Value;
					}
				}
			}
			if (this.targetsHeight)
			{
				if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.height.Value)
				{
					linearVelocity.y += this.acceleration.Value;
				}
				if (this.self.Value.transform.position.y > this.target.Value.transform.position.y + this.height.Value)
				{
					linearVelocity.y -= this.acceleration.Value;
				}
			}
			if (!this.minAboveHero.IsNone)
			{
				if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.minAboveHero.Value)
				{
					linearVelocity.y += this.acceleration.Value;
				}
				else
				{
					linearVelocity.y -= this.acceleration.Value;
				}
			}
			if (linearVelocity.x > this.speedMax.Value)
			{
				linearVelocity.x = this.speedMax.Value;
			}
			if (linearVelocity.x < -this.speedMax.Value)
			{
				linearVelocity.x = -this.speedMax.Value;
			}
			if (linearVelocity.y > this.speedMax.Value)
			{
				linearVelocity.y = this.speedMax.Value;
			}
			if (linearVelocity.y < -this.speedMax.Value)
			{
				linearVelocity.y = -this.speedMax.Value;
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005AAD RID: 23213
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005AAE RID: 23214
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005AAF RID: 23215
		public FsmFloat distance;

		// Token: 0x04005AB0 RID: 23216
		public FsmFloat speedMax;

		// Token: 0x04005AB1 RID: 23217
		public FsmFloat acceleration;

		// Token: 0x04005AB2 RID: 23218
		[Tooltip("If true, object tries to keep to a certain height relative to target")]
		public bool targetsHeight;

		// Token: 0x04005AB3 RID: 23219
		public FsmFloat height;

		// Token: 0x04005AB4 RID: 23220
		public FsmFloat minAboveHero;

		// Token: 0x04005AB5 RID: 23221
		private float distanceAway;

		// Token: 0x04005AB6 RID: 23222
		private FsmGameObject self;
	}
}
