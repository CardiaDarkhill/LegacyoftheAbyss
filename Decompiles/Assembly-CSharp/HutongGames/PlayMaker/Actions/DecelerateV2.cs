using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0E RID: 3086
	[ActionCategory("Enemy AI")]
	[Tooltip("Decelerate X and Y until 0 reached. Multiplies instead of adds.")]
	public class DecelerateV2 : RigidBody2dActionBase
	{
		// Token: 0x06005E1B RID: 24091 RVA: 0x001DA818 File Offset: 0x001D8A18
		public override void Reset()
		{
			this.gameObject = null;
			this.deceleration = 0f;
			this.brakeOnExit = false;
		}

		// Token: 0x06005E1C RID: 24092 RVA: 0x001DA838 File Offset: 0x001D8A38
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E1D RID: 24093 RVA: 0x001DA846 File Offset: 0x001D8A46
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E1E RID: 24094 RVA: 0x001DA854 File Offset: 0x001D8A54
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DecelerateSelf();
		}

		// Token: 0x06005E1F RID: 24095 RVA: 0x001DA873 File Offset: 0x001D8A73
		public override void OnFixedUpdate()
		{
			this.DecelerateSelf();
		}

		// Token: 0x06005E20 RID: 24096 RVA: 0x001DA87C File Offset: 0x001D8A7C
		private void DecelerateSelf()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (linearVelocity.x < 0f)
			{
				linearVelocity.x *= this.deceleration.Value;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			else if (linearVelocity.x > 0f)
			{
				linearVelocity.x *= this.deceleration.Value;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.y < 0f)
			{
				linearVelocity.y *= this.deceleration.Value;
				if (linearVelocity.y > 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			else if (linearVelocity.y > 0f)
			{
				linearVelocity.y *= this.deceleration.Value;
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x06005E21 RID: 24097 RVA: 0x001DA9A4 File Offset: 0x001D8BA4
		public override void OnExit()
		{
			base.OnExit();
			if (this.brakeOnExit)
			{
				this.rb2d.linearVelocity = new Vector2(0f, 0f);
			}
		}

		// Token: 0x04005A74 RID: 23156
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A75 RID: 23157
		public FsmFloat deceleration;

		// Token: 0x04005A76 RID: 23158
		public bool brakeOnExit;
	}
}
