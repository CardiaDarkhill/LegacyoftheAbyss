using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDE RID: 3038
	[ActionCategory("Enemy AI")]
	[Tooltip("Object runs towards target")]
	public class ChaseObjectGround : RigidBody2dActionBase
	{
		// Token: 0x06005D16 RID: 23830 RVA: 0x001D4255 File Offset: 0x001D2455
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.xOffset = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
			this.snapTo = false;
		}

		// Token: 0x06005D17 RID: 23831 RVA: 0x001D4293 File Offset: 0x001D2493
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D18 RID: 23832 RVA: 0x001D42A4 File Offset: 0x001D24A4
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.animator = this.self.Value.GetComponent<tk2dSpriteAnimator>();
			this.DoChase();
			if (this.onlyOnStateEntry)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D19 RID: 23833 RVA: 0x001D430E File Offset: 0x001D250E
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D1A RID: 23834 RVA: 0x001D431C File Offset: 0x001D251C
		public override void OnFixedUpdate()
		{
			if (!this.snapFrame)
			{
				this.DoChase();
				return;
			}
			this.snapFrame = false;
		}

		// Token: 0x06005D1B RID: 23835 RVA: 0x001D4334 File Offset: 0x001D2534
		private void DoChase()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			float num = this.target.Value.transform.position.x + this.xOffset.Value;
			Vector3 position = this.self.Value.transform.position;
			if (position.x < num - this.turnRange.Value)
			{
				linearVelocity.x += this.acceleration.Value;
				this.movingRight = true;
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
			else if (position.x > num + this.turnRange.Value)
			{
				linearVelocity.x -= this.acceleration.Value;
				this.movingRight = false;
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
			else if (!this.snapTo)
			{
				if (this.movingRight)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else
				{
					linearVelocity.x -= this.acceleration.Value;
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
			if (this.snapTo && ((this.rb2d.linearVelocity.x < 0f && position.x < num) || (this.rb2d.linearVelocity.x > 0f && position.x > num)))
			{
				this.DoSnap();
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x06005D1C RID: 23836 RVA: 0x001D45D4 File Offset: 0x001D27D4
		private void DoSnap()
		{
			if (!this.snapSpeedOnly)
			{
				Vector3 position = this.self.Value.transform.position;
				this.self.Value.transform.position = new Vector3(this.target.Value.transform.position.x + this.xOffset.Value, position.y, position.z);
			}
			this.rb2d.linearVelocity = new Vector3(0f, this.rb2d.linearVelocity.y);
			this.snapFrame = true;
		}

		// Token: 0x040058CB RID: 22731
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058CC RID: 22732
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040058CD RID: 22733
		public FsmFloat xOffset;

		// Token: 0x040058CE RID: 22734
		public FsmFloat speedMax;

		// Token: 0x040058CF RID: 22735
		public FsmFloat acceleration;

		// Token: 0x040058D0 RID: 22736
		public bool animateTurnAndRun;

		// Token: 0x040058D1 RID: 22737
		public FsmString runAnimation;

		// Token: 0x040058D2 RID: 22738
		public FsmString turnAnimation;

		// Token: 0x040058D3 RID: 22739
		public FsmFloat turnRange;

		// Token: 0x040058D4 RID: 22740
		public bool snapTo;

		// Token: 0x040058D5 RID: 22741
		public bool snapSpeedOnly;

		// Token: 0x040058D6 RID: 22742
		public bool onlyOnStateEntry;

		// Token: 0x040058D7 RID: 22743
		private FsmGameObject self;

		// Token: 0x040058D8 RID: 22744
		private tk2dSpriteAnimator animator;

		// Token: 0x040058D9 RID: 22745
		private bool turning;

		// Token: 0x040058DA RID: 22746
		private bool movingRight;

		// Token: 0x040058DB RID: 22747
		private bool snapFrame;
	}
}
