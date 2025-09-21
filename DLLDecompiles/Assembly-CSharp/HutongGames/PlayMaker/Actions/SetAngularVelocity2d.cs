using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1E RID: 3358
	[ActionCategory("Physics 2d")]
	[Tooltip("Sets the Angular Velocity of a Game Object. NOTE: Game object must have a rigidbody 2D.")]
	public class SetAngularVelocity2d : RigidBody2dActionBase
	{
		// Token: 0x06006310 RID: 25360 RVA: 0x001F5433 File Offset: 0x001F3633
		public override void Reset()
		{
			this.angularVelocity = null;
			this.everyFrame = false;
		}

		// Token: 0x06006311 RID: 25361 RVA: 0x001F5443 File Offset: 0x001F3643
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x001F5451 File Offset: 0x001F3651
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006313 RID: 25363 RVA: 0x001F545F File Offset: 0x001F365F
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x001F548C File Offset: 0x001F368C
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006315 RID: 25365 RVA: 0x001F54A2 File Offset: 0x001F36A2
		private void DoSetVelocity()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (!this.angularVelocity.IsNone)
			{
				this.rb2d.angularVelocity = this.angularVelocity.Value;
			}
		}

		// Token: 0x04006183 RID: 24963
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006184 RID: 24964
		public FsmFloat angularVelocity;

		// Token: 0x04006185 RID: 24965
		public bool everyFrame;
	}
}
