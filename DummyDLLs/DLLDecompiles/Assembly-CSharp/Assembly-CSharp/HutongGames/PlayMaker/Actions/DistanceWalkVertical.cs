using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C23 RID: 3107
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target.")]
	public class DistanceWalkVertical : RigidBody2dActionBase
	{
		// Token: 0x06005E9A RID: 24218 RVA: 0x001DE9CB File Offset: 0x001DCBCB
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speed = 0f;
		}

		// Token: 0x06005E9B RID: 24219 RVA: 0x001DE9EB File Offset: 0x001DCBEB
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E9C RID: 24220 RVA: 0x001DE9F9 File Offset: 0x001DCBF9
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E9D RID: 24221 RVA: 0x001DEA08 File Offset: 0x001DCC08
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

		// Token: 0x06005E9E RID: 24222 RVA: 0x001DEA6C File Offset: 0x001DCC6C
		public override void OnUpdate()
		{
			if (this.changeTimer > 0f)
			{
				this.changeTimer -= Time.deltaTime;
			}
		}

		// Token: 0x06005E9F RID: 24223 RVA: 0x001DEA8D File Offset: 0x001DCC8D
		public override void OnFixedUpdate()
		{
			this.DoWalk();
		}

		// Token: 0x06005EA0 RID: 24224 RVA: 0x001DEA98 File Offset: 0x001DCC98
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
						linearVelocity.y = this.speed.Value;
						this.movingUp = true;
						this.changeTimer = this.ANIM_CHANGE_TIME;
					}
				}
				else if (this.movingUp && this.changeTimer <= 0f)
				{
					linearVelocity.y = -this.speed.Value;
					this.movingUp = false;
					this.changeTimer = this.ANIM_CHANGE_TIME;
				}
			}
			else if (this.distanceAway < this.distance.Value - this.range.Value)
			{
				if (this.self.Value.transform.position.y < this.target.Value.transform.position.y)
				{
					if (this.movingUp && this.changeTimer <= 0f)
					{
						linearVelocity.y = -this.speed.Value;
						this.movingUp = false;
						this.changeTimer = this.ANIM_CHANGE_TIME;
					}
				}
				else if (!this.movingUp && this.changeTimer <= 0f)
				{
					linearVelocity.y = this.speed.Value;
					this.movingUp = true;
					this.changeTimer = this.ANIM_CHANGE_TIME;
				}
			}
			if (this.rb2d.linearVelocity.y > -0.1f && this.rb2d.linearVelocity.y < 0.1f)
			{
				if (Random.value > 0.5f)
				{
					linearVelocity.y = this.speed.Value;
					this.movingUp = true;
				}
				else
				{
					linearVelocity.y = -this.speed.Value;
					this.movingUp = false;
				}
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

		// Token: 0x04005B1A RID: 23322
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005B1B RID: 23323
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005B1C RID: 23324
		public FsmFloat distance;

		// Token: 0x04005B1D RID: 23325
		public FsmFloat speed;

		// Token: 0x04005B1E RID: 23326
		public FsmFloat range;

		// Token: 0x04005B1F RID: 23327
		public bool changeAnimation;

		// Token: 0x04005B20 RID: 23328
		public bool spriteFacesRight;

		// Token: 0x04005B21 RID: 23329
		public FsmString forwardAnimation;

		// Token: 0x04005B22 RID: 23330
		public FsmString backAnimation;

		// Token: 0x04005B23 RID: 23331
		private float distanceAway;

		// Token: 0x04005B24 RID: 23332
		private FsmGameObject self;

		// Token: 0x04005B25 RID: 23333
		private tk2dSpriteAnimator animator;

		// Token: 0x04005B26 RID: 23334
		private bool movingUp;

		// Token: 0x04005B27 RID: 23335
		private float ANIM_CHANGE_TIME = 0.6f;

		// Token: 0x04005B28 RID: 23336
		private float changeTimer;
	}
}
