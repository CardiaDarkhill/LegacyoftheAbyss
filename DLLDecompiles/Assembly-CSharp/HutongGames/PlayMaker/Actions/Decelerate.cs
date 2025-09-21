using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0D RID: 3085
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y until 0 reached.")]
	public class Decelerate : RigidBody2dActionBase
	{
		// Token: 0x06005E14 RID: 24084 RVA: 0x001DA68B File Offset: 0x001D888B
		public override void Reset()
		{
			this.gameObject = null;
			this.deceleration = 0f;
		}

		// Token: 0x06005E15 RID: 24085 RVA: 0x001DA6A4 File Offset: 0x001D88A4
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E16 RID: 24086 RVA: 0x001DA6B2 File Offset: 0x001D88B2
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E17 RID: 24087 RVA: 0x001DA6C0 File Offset: 0x001D88C0
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DecelerateSelf();
		}

		// Token: 0x06005E18 RID: 24088 RVA: 0x001DA6DF File Offset: 0x001D88DF
		public override void OnFixedUpdate()
		{
			this.DecelerateSelf();
		}

		// Token: 0x06005E19 RID: 24089 RVA: 0x001DA6E8 File Offset: 0x001D88E8
		private void DecelerateSelf()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (linearVelocity.x < 0f)
			{
				linearVelocity.x += this.deceleration.Value;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			else if (linearVelocity.x > 0f)
			{
				linearVelocity.x -= this.deceleration.Value;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.y < 0f)
			{
				linearVelocity.y += this.deceleration.Value;
				if (linearVelocity.y > 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			else if (linearVelocity.y > 0f)
			{
				linearVelocity.y -= this.deceleration.Value;
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005A72 RID: 23154
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A73 RID: 23155
		public FsmFloat deceleration;
	}
}
