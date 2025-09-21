using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C20 RID: 3104
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalk : RigidBody2dActionBase
	{
		// Token: 0x06005E82 RID: 24194 RVA: 0x001DDA9F File Offset: 0x001DBC9F
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speed = 0f;
		}

		// Token: 0x06005E83 RID: 24195 RVA: 0x001DDABF File Offset: 0x001DBCBF
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E84 RID: 24196 RVA: 0x001DDACD File Offset: 0x001DBCCD
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E85 RID: 24197 RVA: 0x001DDADC File Offset: 0x001DBCDC
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.changeAnimation)
			{
				this.animator = this.self.Value.GetComponent<tk2dSpriteAnimator>();
			}
			this.changeTimer = 0f;
			this.DoWalk();
		}

		// Token: 0x06005E86 RID: 24198 RVA: 0x001DDB4B File Offset: 0x001DBD4B
		public override void OnUpdate()
		{
			if (this.changeTimer > 0f)
			{
				this.changeTimer -= Time.deltaTime;
			}
		}

		// Token: 0x06005E87 RID: 24199 RVA: 0x001DDB6C File Offset: 0x001DBD6C
		public override void OnFixedUpdate()
		{
			this.DoWalk();
		}

		// Token: 0x06005E88 RID: 24200 RVA: 0x001DDB74 File Offset: 0x001DBD74
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
						linearVelocity.x = this.speed.Value;
						this.movingRight = true;
						this.changeTimer = this.ANIM_CHANGE_TIME;
					}
				}
				else if (this.movingRight && this.changeTimer <= 0f)
				{
					linearVelocity.x = -this.speed.Value;
					this.movingRight = false;
					this.changeTimer = this.ANIM_CHANGE_TIME;
				}
			}
			else if (this.distanceAway < this.distance.Value - this.range.Value)
			{
				if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					if (this.movingRight && this.changeTimer <= 0f)
					{
						linearVelocity.x = -this.speed.Value;
						this.movingRight = false;
						this.changeTimer = this.ANIM_CHANGE_TIME;
					}
				}
				else if (!this.movingRight && this.changeTimer <= 0f)
				{
					linearVelocity.x = this.speed.Value;
					this.movingRight = true;
					this.changeTimer = this.ANIM_CHANGE_TIME;
				}
			}
			if (this.rb2d.linearVelocity.x > -0.1f && this.rb2d.linearVelocity.x < 0.1f)
			{
				if (this.distanceAway >= this.distance.Value)
				{
					if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
					{
						linearVelocity.x = this.speed.Value;
						this.movingRight = true;
					}
					else
					{
						linearVelocity.x = -this.speed.Value;
						this.movingRight = false;
					}
				}
				else if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
				{
					linearVelocity.x = -this.speed.Value;
					this.movingRight = false;
				}
				else
				{
					linearVelocity.x = this.speed.Value;
					this.movingRight = true;
				}
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

		// Token: 0x04005AE8 RID: 23272
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005AE9 RID: 23273
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005AEA RID: 23274
		public FsmFloat distance;

		// Token: 0x04005AEB RID: 23275
		public FsmFloat speed;

		// Token: 0x04005AEC RID: 23276
		public FsmFloat range;

		// Token: 0x04005AED RID: 23277
		public bool changeAnimation;

		// Token: 0x04005AEE RID: 23278
		public bool spriteFacesRight;

		// Token: 0x04005AEF RID: 23279
		public FsmString forwardAnimation;

		// Token: 0x04005AF0 RID: 23280
		public FsmString backAnimation;

		// Token: 0x04005AF1 RID: 23281
		private float distanceAway;

		// Token: 0x04005AF2 RID: 23282
		private FsmGameObject self;

		// Token: 0x04005AF3 RID: 23283
		private tk2dSpriteAnimator animator;

		// Token: 0x04005AF4 RID: 23284
		private bool movingRight;

		// Token: 0x04005AF5 RID: 23285
		private float ANIM_CHANGE_TIME = 0.6f;

		// Token: 0x04005AF6 RID: 23286
		private float changeTimer;

		// Token: 0x04005AF7 RID: 23287
		private bool randomStart;
	}
}
