using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C21 RID: 3105
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalkServitor : RigidBody2dActionBase
	{
		// Token: 0x06005E8A RID: 24202 RVA: 0x001DDFF4 File Offset: 0x001DC1F4
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speed = 0f;
		}

		// Token: 0x06005E8B RID: 24203 RVA: 0x001DE014 File Offset: 0x001DC214
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x001DE022 File Offset: 0x001DC222
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E8D RID: 24205 RVA: 0x001DE030 File Offset: 0x001DC230
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.changeAnimation)
			{
				this.animator = this.self.Value.GetComponent<tk2dSpriteAnimator>();
			}
			this.DoWalk();
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x001DE094 File Offset: 0x001DC294
		public override void OnUpdate()
		{
			if (this.changeTimer > 0f)
			{
				this.changeTimer -= Time.deltaTime;
			}
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x001DE0B5 File Offset: 0x001DC2B5
		public override void OnFixedUpdate()
		{
			this.DoWalk();
		}

		// Token: 0x06005E90 RID: 24208 RVA: 0x001DE0C0 File Offset: 0x001DC2C0
		private void DoWalk()
		{
			if (this.rb2d == null)
			{
				return;
			}
			this.distanceAway = this.self.Value.transform.position.x - this.target.Value.transform.position.x;
			if (this.distanceAway < 0f)
			{
				this.distanceAway *= -1f;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.distanceAway > this.distance.Value + this.range.Value)
			{
				if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					if (!this.movingRight && this.changeTimer <= 0f)
					{
						this.movingRight = true;
						this.changeTimer = 0.5f;
					}
				}
				else if (this.movingRight && this.changeTimer <= 0f)
				{
					this.movingRight = false;
					this.changeTimer = 0.5f;
				}
			}
			else if (this.distanceAway < this.distance.Value - this.range.Value)
			{
				if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					if (this.movingRight && this.changeTimer <= 0f)
					{
						this.movingRight = false;
						this.changeTimer = 0.5f;
					}
				}
				else if (!this.movingRight && this.changeTimer <= 0f)
				{
					this.movingRight = true;
					this.changeTimer = 0.5f;
				}
			}
			if (this.rb2d.linearVelocity.x > -0.1f && this.rb2d.linearVelocity.x < 0.1f)
			{
				this.stoppedTimer += Time.deltaTime;
				if (this.stoppedTimer >= 0.15f)
				{
					if (Random.value > 0.5f)
					{
						this.movingRight = true;
					}
					else
					{
						this.movingRight = false;
					}
					this.stoppedTimer = 0f;
					this.changeTimer = 0.5f;
				}
			}
			else
			{
				this.stoppedTimer = 0f;
			}
			if (this.movingRight)
			{
				linearVelocity.x += this.acceleration.Value * Time.deltaTime;
			}
			else
			{
				linearVelocity.x -= this.acceleration.Value * Time.deltaTime;
			}
			if (linearVelocity.x < -this.speed.Value)
			{
				linearVelocity.x = -this.speed.Value;
			}
			if (linearVelocity.x > this.speed.Value)
			{
				linearVelocity.x = this.speed.Value;
			}
			this.rb2d.linearVelocity = linearVelocity;
			if (this.changeAnimation)
			{
				if (this.self.Value.transform.localScale.x > 0f)
				{
					if ((this.spriteFacesRight && this.movingRight) || (!this.spriteFacesRight && !this.movingRight))
					{
						this.animator.Play(this.forwardAnimation.Value);
					}
					if ((!this.spriteFacesRight && this.movingRight) || (this.spriteFacesRight && !this.movingRight))
					{
						this.animator.Play(this.backAnimation.Value);
						return;
					}
				}
				else
				{
					if ((this.spriteFacesRight && this.movingRight) || (!this.spriteFacesRight && !this.movingRight))
					{
						this.animator.Play(this.backAnimation.Value);
					}
					if ((!this.spriteFacesRight && this.movingRight) || (this.spriteFacesRight && !this.movingRight))
					{
						this.animator.Play(this.forwardAnimation.Value);
					}
				}
			}
		}

		// Token: 0x04005AF8 RID: 23288
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005AF9 RID: 23289
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005AFA RID: 23290
		public FsmFloat distance;

		// Token: 0x04005AFB RID: 23291
		public FsmFloat acceleration;

		// Token: 0x04005AFC RID: 23292
		public FsmFloat speed;

		// Token: 0x04005AFD RID: 23293
		public FsmFloat range;

		// Token: 0x04005AFE RID: 23294
		public bool changeAnimation;

		// Token: 0x04005AFF RID: 23295
		public bool spriteFacesRight;

		// Token: 0x04005B00 RID: 23296
		public FsmString forwardAnimation;

		// Token: 0x04005B01 RID: 23297
		public FsmString backAnimation;

		// Token: 0x04005B02 RID: 23298
		private float distanceAway;

		// Token: 0x04005B03 RID: 23299
		private FsmGameObject self;

		// Token: 0x04005B04 RID: 23300
		private tk2dSpriteAnimator animator;

		// Token: 0x04005B05 RID: 23301
		private bool movingRight;

		// Token: 0x04005B06 RID: 23302
		private const float ANIM_CHANGE_TIME = 0.5f;

		// Token: 0x04005B07 RID: 23303
		private float changeTimer;

		// Token: 0x04005B08 RID: 23304
		private float stoppedTimer;
	}
}
