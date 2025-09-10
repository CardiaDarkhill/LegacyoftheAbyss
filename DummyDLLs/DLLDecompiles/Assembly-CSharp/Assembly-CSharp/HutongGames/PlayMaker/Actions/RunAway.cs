using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D02 RID: 3330
	[ActionCategory("Enemy AI")]
	[Tooltip("Object runs away from target")]
	public class RunAway : RigidBody2dActionBase
	{
		// Token: 0x0600629B RID: 25243 RVA: 0x001F2CFC File Offset: 0x001F0EFC
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x001F2D2C File Offset: 0x001F0F2C
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x001F2D3A File Offset: 0x001F0F3A
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x001F2D48 File Offset: 0x001F0F48
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.animator = this.self.Value.GetComponent<tk2dSpriteAnimator>();
			this.DoChase();
		}

		// Token: 0x0600629F RID: 25247 RVA: 0x001F2DA4 File Offset: 0x001F0FA4
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x060062A0 RID: 25248 RVA: 0x001F2DAC File Offset: 0x001F0FAC
		private void DoChase()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
			{
				linearVelocity.x -= this.acceleration.Value;
				if (this.animateTurnAndRun)
				{
					if (linearVelocity.x < 0f && !this.turning)
					{
						this.animator.Play(this.turnAnimation.Value);
						this.turning = true;
					}
					if (linearVelocity.x > 0f && this.turning)
					{
						this.animator.Play(this.runAnimation.Value);
						this.turning = false;
					}
				}
			}
			else
			{
				linearVelocity.x += this.acceleration.Value;
				if (this.animateTurnAndRun)
				{
					if (linearVelocity.x > 0f && !this.turning)
					{
						this.animator.Play(this.turnAnimation.Value);
						this.turning = true;
					}
					if (linearVelocity.x < 0f && this.turning)
					{
						this.animator.Play(this.runAnimation.Value);
						this.turning = false;
					}
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
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04006102 RID: 24834
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006103 RID: 24835
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04006104 RID: 24836
		public FsmFloat speedMax;

		// Token: 0x04006105 RID: 24837
		public FsmFloat acceleration;

		// Token: 0x04006106 RID: 24838
		public bool animateTurnAndRun;

		// Token: 0x04006107 RID: 24839
		public FsmString runAnimation;

		// Token: 0x04006108 RID: 24840
		public FsmString turnAnimation;

		// Token: 0x04006109 RID: 24841
		private FsmGameObject self;

		// Token: 0x0400610A RID: 24842
		private tk2dSpriteAnimator animator;

		// Token: 0x0400610B RID: 24843
		private bool turning;
	}
}
