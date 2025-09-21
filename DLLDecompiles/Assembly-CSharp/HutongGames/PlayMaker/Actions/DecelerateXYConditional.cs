using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C10 RID: 3088
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y separately. Uses multiplication.")]
	public class DecelerateXYConditional : RigidBody2dActionBase
	{
		// Token: 0x06005E2B RID: 24107 RVA: 0x001DAC54 File Offset: 0x001D8E54
		public override void Reset()
		{
			this.gameObject = null;
			this.decelerationX = null;
			this.decelerationY = null;
			this.brakeOnExit = false;
			this.conditionalBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06005E2C RID: 24108 RVA: 0x001DAC84 File Offset: 0x001D8E84
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E2D RID: 24109 RVA: 0x001DAC92 File Offset: 0x001D8E92
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E2E RID: 24110 RVA: 0x001DACA0 File Offset: 0x001D8EA0
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DecelerateSelf();
		}

		// Token: 0x06005E2F RID: 24111 RVA: 0x001DACC0 File Offset: 0x001D8EC0
		public override void OnExit()
		{
			base.OnExit();
			if (this.brakeOnExit)
			{
				if (!this.decelerationX.IsNone)
				{
					this.rb2d.linearVelocity = new Vector2(0f, this.rb2d.linearVelocity.y);
				}
				if (!this.decelerationY.IsNone)
				{
					this.rb2d.linearVelocity = new Vector2(this.rb2d.linearVelocity.x, 0f);
				}
			}
		}

		// Token: 0x06005E30 RID: 24112 RVA: 0x001DAD3F File Offset: 0x001D8F3F
		public override void OnFixedUpdate()
		{
			this.DecelerateSelf();
		}

		// Token: 0x06005E31 RID: 24113 RVA: 0x001DAD48 File Offset: 0x001D8F48
		private void DecelerateSelf()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (!this.conditionalBool.Value)
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
				if (linearVelocity.x < 0.001f && linearVelocity.x > -0.001f)
				{
					linearVelocity.x = 0f;
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
				if (linearVelocity.y < 0.001f && linearVelocity.y > -0.001f)
				{
					linearVelocity.y = 0f;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005A7B RID: 23163
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A7C RID: 23164
		public FsmFloat decelerationX;

		// Token: 0x04005A7D RID: 23165
		public FsmFloat decelerationY;

		// Token: 0x04005A7E RID: 23166
		public bool brakeOnExit;

		// Token: 0x04005A7F RID: 23167
		public FsmBool conditionalBool;
	}
}
