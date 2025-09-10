using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B96 RID: 2966
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y separately. Uses multiplication.")]
	public class AccelerateTo : RigidBody2dActionBase
	{
		// Token: 0x06005BC3 RID: 23491 RVA: 0x001CDFA6 File Offset: 0x001CC1A6
		public override void Reset()
		{
			this.gameObject = null;
			this.decelerationX = null;
			this.decelerationY = null;
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x001CDFBD File Offset: 0x001CC1BD
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BC5 RID: 23493 RVA: 0x001CDFCB File Offset: 0x001CC1CB
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DecelerateSelf();
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x001CDFEA File Offset: 0x001CC1EA
		public override void OnFixedUpdate()
		{
			this.DecelerateSelf();
		}

		// Token: 0x06005BC7 RID: 23495 RVA: 0x001CDFF4 File Offset: 0x001CC1F4
		private void DecelerateSelf()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (!this.decelerationX.IsNone)
			{
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x *= this.decelerationX.Value;
					if (linearVelocity.x > 0f)
					{
						linearVelocity.x = 0f;
					}
				}
				else if (linearVelocity.x > 0f)
				{
					linearVelocity.x *= this.decelerationX.Value;
					if (linearVelocity.x < 0f)
					{
						linearVelocity.x = 0f;
					}
				}
			}
			if (!this.decelerationY.IsNone)
			{
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y *= this.decelerationY.Value;
					if (linearVelocity.y > 0f)
					{
						linearVelocity.y = 0f;
					}
				}
				else if (linearVelocity.y > 0f)
				{
					linearVelocity.y *= this.decelerationY.Value;
					if (linearVelocity.y < 0f)
					{
						linearVelocity.y = 0f;
					}
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005731 RID: 22321
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005732 RID: 22322
		public FsmFloat decelerationX;

		// Token: 0x04005733 RID: 22323
		public FsmFloat decelerationY;
	}
}
