using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5A RID: 3162
	[ActionCategory("Physics 2d")]
	[Tooltip("Sets the Angular Velocity of a Game Object. NOTE: Game object must have a rigidbody 2D.")]
	public class GetAngularVelocity2d : RigidBody2dActionBase
	{
		// Token: 0x06005FB4 RID: 24500 RVA: 0x001E59F0 File Offset: 0x001E3BF0
		public override void Reset()
		{
			this.angularVelocity = null;
			this.everyFrame = false;
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x001E5A00 File Offset: 0x001E3C00
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x001E5A0E File Offset: 0x001E3C0E
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x001E5A1C File Offset: 0x001E3C1C
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FB8 RID: 24504 RVA: 0x001E5A49 File Offset: 0x001E3C49
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x001E5A5F File Offset: 0x001E3C5F
		private void DoSetVelocity()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (!this.angularVelocity.IsNone)
			{
				this.angularVelocity.Value = this.rb2d.angularVelocity;
			}
		}

		// Token: 0x04005D12 RID: 23826
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D13 RID: 23827
		public FsmFloat angularVelocity;

		// Token: 0x04005D14 RID: 23828
		public bool everyFrame;
	}
}
