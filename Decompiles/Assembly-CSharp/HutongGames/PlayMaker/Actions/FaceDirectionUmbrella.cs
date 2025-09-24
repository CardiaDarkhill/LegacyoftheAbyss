using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2F RID: 3119
	[ActionCategory("Enemy AI")]
	[Tooltip("Object will flip to face the direction it is moving on X Axis.")]
	public class FaceDirectionUmbrella : RigidBody2dActionBase
	{
		// Token: 0x06005EE1 RID: 24289 RVA: 0x001E07ED File Offset: 0x001DE9ED
		public override void Reset()
		{
			this.gameObject = null;
			this.everyFrame = false;
			this.playNewAnimation = false;
			this.newAnimationClip = null;
		}

		// Token: 0x06005EE2 RID: 24290 RVA: 0x001E080C File Offset: 0x001DEA0C
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.heroCtrl = this.target.Value.GetComponent<HeroController>();
			this._sprite = this.target.Value.GetComponent<tk2dSpriteAnimator>();
			this.xScale = this.target.Value.transform.localScale.x;
			if (this.xScale < 0f)
			{
				this.xScale *= -1f;
			}
			this.DoFace();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005EE3 RID: 24291 RVA: 0x001E08CB File Offset: 0x001DEACB
		public override void OnUpdate()
		{
			this.DoFace();
		}

		// Token: 0x06005EE4 RID: 24292 RVA: 0x001E08D4 File Offset: 0x001DEAD4
		private void DoFace()
		{
			if (this.rb2d == null)
			{
				return;
			}
			ref Vector2 linearVelocity = this.rb2d.linearVelocity;
			Vector3 localScale = this.target.Value.transform.localScale;
			float x = linearVelocity.x;
			if (this.pauseTimer <= 0f || !this.pauseBetweenTurns)
			{
				if (x > 0f)
				{
					if (localScale.x != -this.xScale)
					{
						this.pauseTimer = this.pauseTime.Value;
						this.heroCtrl.FaceRight();
						if (this.playNewAnimation)
						{
							this._sprite.Play(this.newAnimationClip.Value);
							this._sprite.PlayFromFrame(0);
							return;
						}
					}
				}
				else if (x < 0f && localScale.x != this.xScale)
				{
					this.pauseTimer = this.pauseTime.Value;
					this.heroCtrl.FaceLeft();
					if (this.playNewAnimation)
					{
						this._sprite.Play(this.newAnimationClip.Value);
						this._sprite.PlayFromFrame(0);
						return;
					}
				}
			}
			else
			{
				this.pauseTimer -= Time.deltaTime;
			}
		}

		// Token: 0x04005B8A RID: 23434
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005B8B RID: 23435
		public bool playNewAnimation;

		// Token: 0x04005B8C RID: 23436
		public FsmString newAnimationClip;

		// Token: 0x04005B8D RID: 23437
		public bool everyFrame;

		// Token: 0x04005B8E RID: 23438
		public bool pauseBetweenTurns;

		// Token: 0x04005B8F RID: 23439
		public FsmFloat pauseTime;

		// Token: 0x04005B90 RID: 23440
		private HeroController heroCtrl;

		// Token: 0x04005B91 RID: 23441
		private FsmGameObject target;

		// Token: 0x04005B92 RID: 23442
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005B93 RID: 23443
		private float xScale;

		// Token: 0x04005B94 RID: 23444
		private float pauseTimer;
	}
}
