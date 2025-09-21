using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0F RID: 3087
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y separately. Uses multiplication.")]
	public class DecelerateXY : RigidBody2dActionBase
	{
		// Token: 0x06005E23 RID: 24099 RVA: 0x001DA9D6 File Offset: 0x001D8BD6
		public override void Reset()
		{
			this.gameObject = null;
			this.decelerationX = null;
			this.decelerationY = null;
			this.brakeOnExit = false;
		}

		// Token: 0x06005E24 RID: 24100 RVA: 0x001DA9F4 File Offset: 0x001D8BF4
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E25 RID: 24101 RVA: 0x001DAA02 File Offset: 0x001D8C02
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E26 RID: 24102 RVA: 0x001DAA10 File Offset: 0x001D8C10
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DecelerateSelf();
		}

		// Token: 0x06005E27 RID: 24103 RVA: 0x001DAA30 File Offset: 0x001D8C30
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

		// Token: 0x06005E28 RID: 24104 RVA: 0x001DAAAF File Offset: 0x001D8CAF
		public override void OnFixedUpdate()
		{
			this.DecelerateSelf();
		}

		// Token: 0x06005E29 RID: 24105 RVA: 0x001DAAB8 File Offset: 0x001D8CB8
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

		// Token: 0x04005A77 RID: 23159
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A78 RID: 23160
		public FsmFloat decelerationX;

		// Token: 0x04005A79 RID: 23161
		public FsmFloat decelerationY;

		// Token: 0x04005A7A RID: 23162
		public bool brakeOnExit;
	}
}
