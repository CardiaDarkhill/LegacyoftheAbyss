using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1D RID: 3357
	[ActionCategory("Physics 2d")]
	public class SetAngularDrag2d : RigidBody2dActionBase
	{
		// Token: 0x06006309 RID: 25353 RVA: 0x001F5388 File Offset: 0x001F3588
		public override void Reset()
		{
			this.angularDrag = null;
			this.everyFrame = false;
		}

		// Token: 0x0600630A RID: 25354 RVA: 0x001F5398 File Offset: 0x001F3598
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600630B RID: 25355 RVA: 0x001F53A6 File Offset: 0x001F35A6
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600630C RID: 25356 RVA: 0x001F53B4 File Offset: 0x001F35B4
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoSetDrag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x001F53E1 File Offset: 0x001F35E1
		public override void OnFixedUpdate()
		{
			this.DoSetDrag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x001F53F7 File Offset: 0x001F35F7
		private void DoSetDrag()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (!this.angularDrag.IsNone)
			{
				this.rb2d.angularDamping = this.angularDrag.Value;
			}
		}

		// Token: 0x04006180 RID: 24960
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006181 RID: 24961
		public FsmFloat angularDrag;

		// Token: 0x04006182 RID: 24962
		public bool everyFrame;
	}
}
