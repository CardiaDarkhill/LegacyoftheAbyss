using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B98 RID: 2968
	[ActionCategory("Enemy AI")]
	[Tooltip("Accelerate or decelerate horizontal velocity to a target speed until it is reached.")]
	public class AccelerateToXByScale : RigidBody2dActionBase
	{
		// Token: 0x06005BCF RID: 23503 RVA: 0x001CE327 File Offset: 0x001CC527
		public override void Reset()
		{
			this.gameObject = null;
			this.accelerationFactor = null;
			this.targetSpeed = null;
		}

		// Token: 0x06005BD0 RID: 23504 RVA: 0x001CE33E File Offset: 0x001CC53E
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BD1 RID: 23505 RVA: 0x001CE34C File Offset: 0x001CC54C
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			if (this.rb2d)
			{
				HeroController component = this.rb2d.GetComponent<HeroController>();
				this.velocity = this.rb2d.linearVelocity;
				if (component)
				{
					this.useSelfVelocity = true;
					this.velocity.x = this.velocity.x - component.conveyorSpeed;
				}
				else
				{
					this.useSelfVelocity = false;
				}
			}
			this.DoAccelerate();
		}

		// Token: 0x06005BD2 RID: 23506 RVA: 0x001CE3CD File Offset: 0x001CC5CD
		public override void OnFixedUpdate()
		{
			this.DoAccelerate();
		}

		// Token: 0x06005BD3 RID: 23507 RVA: 0x001CE3D8 File Offset: 0x001CC5D8
		private void DoAccelerate()
		{
			if (this.rb2d == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!this.useSelfVelocity)
			{
				this.velocity = this.rb2d.linearVelocity;
			}
			else
			{
				this.velocity.y = this.rb2d.linearVelocity.y;
			}
			float num;
			if (ownerDefaultTarget.transform.localScale.x > 0f)
			{
				num = this.targetSpeed.Value;
			}
			else
			{
				num = -this.targetSpeed.Value;
			}
			if (this.velocity.x > num)
			{
				this.velocity.x = this.velocity.x - this.accelerationFactor.Value;
				if (this.velocity.x < num)
				{
					this.velocity.x = num;
				}
			}
			if (this.velocity.x < num)
			{
				this.velocity.x = this.velocity.x + this.accelerationFactor.Value;
				if (this.velocity.x > num)
				{
					this.velocity.x = num;
				}
			}
			this.rb2d.linearVelocity = this.velocity;
		}

		// Token: 0x04005739 RID: 22329
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400573A RID: 22330
		public FsmFloat accelerationFactor;

		// Token: 0x0400573B RID: 22331
		public FsmFloat targetSpeed;

		// Token: 0x0400573C RID: 22332
		private bool useSelfVelocity;

		// Token: 0x0400573D RID: 22333
		private Vector2 velocity;
	}
}
