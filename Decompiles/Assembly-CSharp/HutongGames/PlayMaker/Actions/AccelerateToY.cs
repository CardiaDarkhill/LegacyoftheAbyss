using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B99 RID: 2969
	[ActionCategory("Enemy AI")]
	[Tooltip("Accelerate or decelerate horizontal velocity to a target speed until it is reached.")]
	public class AccelerateToY : RigidBody2dActionBase
	{
		// Token: 0x06005BD5 RID: 23509 RVA: 0x001CE509 File Offset: 0x001CC709
		public override void Reset()
		{
			this.gameObject = null;
			this.accelerationFactor = null;
			this.targetSpeed = null;
		}

		// Token: 0x06005BD6 RID: 23510 RVA: 0x001CE520 File Offset: 0x001CC720
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BD7 RID: 23511 RVA: 0x001CE52E File Offset: 0x001CC72E
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoAccelerate();
		}

		// Token: 0x06005BD8 RID: 23512 RVA: 0x001CE54D File Offset: 0x001CC74D
		public override void OnFixedUpdate()
		{
			this.DoAccelerate();
		}

		// Token: 0x06005BD9 RID: 23513 RVA: 0x001CE558 File Offset: 0x001CC758
		private void DoAccelerate()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (linearVelocity.y > this.targetSpeed.Value)
			{
				linearVelocity.y -= this.accelerationFactor.Value;
				if (linearVelocity.y < this.targetSpeed.Value)
				{
					linearVelocity.y = this.targetSpeed.Value;
				}
			}
			if (linearVelocity.y < this.targetSpeed.Value)
			{
				linearVelocity.y += this.accelerationFactor.Value;
				if (linearVelocity.y > this.targetSpeed.Value)
				{
					linearVelocity.y = this.targetSpeed.Value;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x0400573E RID: 22334
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400573F RID: 22335
		public FsmFloat accelerationFactor;

		// Token: 0x04005740 RID: 22336
		public FsmFloat targetSpeed;
	}
}
