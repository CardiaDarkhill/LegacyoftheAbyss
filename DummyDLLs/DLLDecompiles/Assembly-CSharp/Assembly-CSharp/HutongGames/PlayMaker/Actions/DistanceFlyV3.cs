using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1F RID: 3103
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target. Optionally try to stay on left or right of target")]
	public class DistanceFlyV3 : RigidBody2dActionBase
	{
		// Token: 0x06005E7B RID: 24187 RVA: 0x001DD344 File Offset: 0x001DB544
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.targetsHeight = false;
			this.height = null;
			this.maxHeight = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005E7C RID: 24188 RVA: 0x001DD394 File Offset: 0x001DB594
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E7D RID: 24189 RVA: 0x001DD3A2 File Offset: 0x001DB5A2
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E7E RID: 24190 RVA: 0x001DD3B0 File Offset: 0x001DB5B0
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoBuzz();
		}

		// Token: 0x06005E7F RID: 24191 RVA: 0x001DD3EB File Offset: 0x001DB5EB
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06005E80 RID: 24192 RVA: 0x001DD3F4 File Offset: 0x001DB5F4
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
				if (this.stayLeft.Value && !this.stayRight.Value && this.self.Value.transform.position.x > this.target.Value.transform.position.x + 1f)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else if (this.stayRight.Value && !this.stayLeft.Value && this.self.Value.transform.position.x < this.target.Value.transform.position.x - 1f)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				if (!this.targetsHeight)
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
				if (this.stayLeft.Value && !this.stayRight.Value && this.self.Value.transform.position.x > this.target.Value.transform.position.x + 1f)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else if (this.stayRight.Value && !this.stayLeft.Value && this.self.Value.transform.position.x < this.target.Value.transform.position.x - 1f)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else
				{
					linearVelocity.x += this.acceleration.Value;
				}
				if (!this.targetsHeight)
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
				if (!this.maxHeight.IsNone && this.maxHeightPrecedence.Value && this.maxHeight.Value != 0f && this.self.Value.transform.position.y > this.maxHeight.Value)
				{
					linearVelocity.y -= this.acceleration.Value;
				}
				else if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.height.Value || this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.minHeightAboveHero.Value)
				{
					linearVelocity.y += this.acceleration.Value;
				}
				else if (!this.maxHeight.IsNone && this.maxHeight.Value != 0f && this.self.Value.transform.position.y > this.maxHeight.Value)
				{
					linearVelocity.y -= this.acceleration.Value;
				}
				else if (this.self.Value.transform.position.y > this.target.Value.transform.position.y + this.height.Value)
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

		// Token: 0x04005ADA RID: 23258
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005ADB RID: 23259
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005ADC RID: 23260
		public FsmFloat distance;

		// Token: 0x04005ADD RID: 23261
		public FsmFloat speedMax;

		// Token: 0x04005ADE RID: 23262
		public FsmFloat acceleration;

		// Token: 0x04005ADF RID: 23263
		[Tooltip("If true, object tries to keep to a certain height relative to target")]
		public bool targetsHeight;

		// Token: 0x04005AE0 RID: 23264
		public FsmFloat height;

		// Token: 0x04005AE1 RID: 23265
		public FsmFloat minHeightAboveHero;

		// Token: 0x04005AE2 RID: 23266
		[UIHint(UIHint.Variable)]
		public FsmFloat maxHeight;

		// Token: 0x04005AE3 RID: 23267
		public FsmBool maxHeightPrecedence;

		// Token: 0x04005AE4 RID: 23268
		public FsmBool stayLeft;

		// Token: 0x04005AE5 RID: 23269
		public FsmBool stayRight;

		// Token: 0x04005AE6 RID: 23270
		private float distanceAway;

		// Token: 0x04005AE7 RID: 23271
		private FsmGameObject self;
	}
}
