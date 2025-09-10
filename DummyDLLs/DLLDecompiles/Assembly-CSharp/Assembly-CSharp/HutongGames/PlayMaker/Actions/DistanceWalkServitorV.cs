using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C22 RID: 3106
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalkServitorV : RigidBody2dActionBase
	{
		// Token: 0x06005E92 RID: 24210 RVA: 0x001DE4DF File Offset: 0x001DC6DF
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speed = 0f;
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x001DE4FF File Offset: 0x001DC6FF
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x001DE50D File Offset: 0x001DC70D
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x001DE51C File Offset: 0x001DC71C
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

		// Token: 0x06005E96 RID: 24214 RVA: 0x001DE580 File Offset: 0x001DC780
		public override void OnUpdate()
		{
			if (this.changeTimer > 0f)
			{
				this.changeTimer -= Time.deltaTime;
			}
		}

		// Token: 0x06005E97 RID: 24215 RVA: 0x001DE5A1 File Offset: 0x001DC7A1
		public override void OnFixedUpdate()
		{
			this.DoWalk();
		}

		// Token: 0x06005E98 RID: 24216 RVA: 0x001DE5AC File Offset: 0x001DC7AC
		private void DoWalk()
		{
			if (this.rb2d == null)
			{
				return;
			}
			this.distanceAway = this.self.Value.transform.position.y - this.target.Value.transform.position.y;
			if (this.distanceAway < 0f)
			{
				this.distanceAway *= -1f;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.distanceAway > this.distance.Value + this.range.Value)
			{
				if (this.self.Value.transform.position.y < this.target.Value.transform.position.y)
				{
					if (!this.movingUp && this.changeTimer <= 0f)
					{
						this.movingUp = true;
						this.changeTimer = 0.5f;
					}
				}
				else if (this.movingUp && this.changeTimer <= 0f)
				{
					this.movingUp = false;
					this.changeTimer = 0.5f;
				}
			}
			else if (this.distanceAway < this.distance.Value - this.range.Value)
			{
				if (this.self.Value.transform.position.y < this.target.Value.transform.position.y)
				{
					if (this.movingUp && this.changeTimer <= 0f)
					{
						this.movingUp = false;
						this.changeTimer = 0.5f;
					}
				}
				else if (!this.movingUp && this.changeTimer <= 0f)
				{
					this.movingUp = true;
					this.changeTimer = 0.5f;
				}
			}
			if (this.rb2d.linearVelocity.y > -0.1f && this.rb2d.linearVelocity.y < 0.1f)
			{
				this.stoppedTimer += Time.deltaTime;
				if (this.stoppedTimer >= 0.15f)
				{
					if (Random.value > 0.5f)
					{
						this.movingUp = true;
					}
					else
					{
						this.movingUp = false;
					}
					this.stoppedTimer = 0f;
					this.changeTimer = 0.5f;
				}
			}
			else
			{
				this.stoppedTimer = 0f;
			}
			if (this.movingUp)
			{
				linearVelocity.y += this.acceleration.Value * Time.deltaTime;
			}
			else
			{
				linearVelocity.y -= this.acceleration.Value * Time.deltaTime;
			}
			if (linearVelocity.y < -this.speed.Value)
			{
				linearVelocity.y = -this.speed.Value;
			}
			if (linearVelocity.y > this.speed.Value)
			{
				linearVelocity.y = this.speed.Value;
			}
			this.rb2d.linearVelocity = linearVelocity;
			if (this.changeAnimation)
			{
				if (this.self.Value.transform.localScale.x > 0f)
				{
					if ((this.spriteFacesRight && this.movingUp) || (!this.spriteFacesRight && !this.movingUp))
					{
						this.animator.Play(this.forwardAnimation.Value);
					}
					if ((!this.spriteFacesRight && this.movingUp) || (this.spriteFacesRight && !this.movingUp))
					{
						this.animator.Play(this.backAnimation.Value);
						return;
					}
				}
				else
				{
					if ((this.spriteFacesRight && this.movingUp) || (!this.spriteFacesRight && !this.movingUp))
					{
						this.animator.Play(this.backAnimation.Value);
					}
					if ((!this.spriteFacesRight && this.movingUp) || (this.spriteFacesRight && !this.movingUp))
					{
						this.animator.Play(this.forwardAnimation.Value);
					}
				}
			}
		}

		// Token: 0x04005B09 RID: 23305
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005B0A RID: 23306
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005B0B RID: 23307
		public FsmFloat distance;

		// Token: 0x04005B0C RID: 23308
		public FsmFloat acceleration;

		// Token: 0x04005B0D RID: 23309
		public FsmFloat speed;

		// Token: 0x04005B0E RID: 23310
		public FsmFloat range;

		// Token: 0x04005B0F RID: 23311
		public bool changeAnimation;

		// Token: 0x04005B10 RID: 23312
		public bool spriteFacesRight;

		// Token: 0x04005B11 RID: 23313
		public FsmString forwardAnimation;

		// Token: 0x04005B12 RID: 23314
		public FsmString backAnimation;

		// Token: 0x04005B13 RID: 23315
		private float distanceAway;

		// Token: 0x04005B14 RID: 23316
		private FsmGameObject self;

		// Token: 0x04005B15 RID: 23317
		private tk2dSpriteAnimator animator;

		// Token: 0x04005B16 RID: 23318
		private bool movingUp;

		// Token: 0x04005B17 RID: 23319
		private const float ANIM_CHANGE_TIME = 0.5f;

		// Token: 0x04005B18 RID: 23320
		private float changeTimer;

		// Token: 0x04005B19 RID: 23321
		private float stoppedTimer;
	}
}
