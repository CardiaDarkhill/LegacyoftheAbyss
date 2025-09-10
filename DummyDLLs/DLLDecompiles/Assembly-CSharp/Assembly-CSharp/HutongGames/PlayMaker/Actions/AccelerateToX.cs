using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B97 RID: 2967
	[ActionCategory("Enemy AI")]
	[Tooltip("Accelerate or decelerate horizontal velocity to a target speed until it is reached.")]
	public class AccelerateToX : RigidBody2dActionBase
	{
		// Token: 0x06005BC9 RID: 23497 RVA: 0x001CE13E File Offset: 0x001CC33E
		public override void Reset()
		{
			this.gameObject = null;
			this.accelerationFactor = null;
			this.targetSpeed = null;
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x001CE155 File Offset: 0x001CC355
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x001CE164 File Offset: 0x001CC364
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
					if (component.cState.inConveyorZone)
					{
						this.velocity.x = this.velocity.x - component.conveyorSpeed;
					}
				}
				else
				{
					this.useSelfVelocity = false;
				}
			}
			this.DoAccelerate();
		}

		// Token: 0x06005BCC RID: 23500 RVA: 0x001CE1F2 File Offset: 0x001CC3F2
		public override void OnFixedUpdate()
		{
			this.DoAccelerate();
		}

		// Token: 0x06005BCD RID: 23501 RVA: 0x001CE1FC File Offset: 0x001CC3FC
		private void DoAccelerate()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (!this.useSelfVelocity)
			{
				this.velocity = this.rb2d.linearVelocity;
			}
			else
			{
				this.velocity.y = this.rb2d.linearVelocity.y;
			}
			if (this.velocity.x > this.targetSpeed.Value)
			{
				this.velocity.x = this.velocity.x - this.accelerationFactor.Value;
				if (this.velocity.x < this.targetSpeed.Value)
				{
					this.velocity.x = this.targetSpeed.Value;
				}
			}
			if (this.velocity.x < this.targetSpeed.Value)
			{
				this.velocity.x = this.velocity.x + this.accelerationFactor.Value;
				if (this.velocity.x > this.targetSpeed.Value)
				{
					this.velocity.x = this.targetSpeed.Value;
				}
			}
			this.rb2d.linearVelocity = this.velocity;
		}

		// Token: 0x04005734 RID: 22324
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005735 RID: 22325
		public FsmFloat accelerationFactor;

		// Token: 0x04005736 RID: 22326
		public FsmFloat targetSpeed;

		// Token: 0x04005737 RID: 22327
		private bool useSelfVelocity;

		// Token: 0x04005738 RID: 22328
		private Vector2 velocity;
	}
}
