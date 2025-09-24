using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200130B RID: 4875
	public sealed class LookAnimNpcGetFacingLeft : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		// Token: 0x06007EA6 RID: 32422 RVA: 0x002597B4 File Offset: 0x002579B4
		public override void Reset()
		{
			base.Reset();
			this.StoreValue = null;
			this.EveryFrame = null;
			this.FacingLeft = null;
			this.FacingRight = null;
		}

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x06007EA7 RID: 32423 RVA: 0x002597D8 File Offset: 0x002579D8
		protected override bool AutoFinish
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06007EA8 RID: 32424 RVA: 0x002597DB File Offset: 0x002579DB
		protected override void DoAction(LookAnimNPC lookAnim)
		{
			this.hasLookNpc = (lookAnim != null);
			this.lookAnimNpc = lookAnim;
			this.SendEvent();
			if (!this.EveryFrame.Value)
			{
				base.Finish();
				return;
			}
		}

		// Token: 0x06007EA9 RID: 32425 RVA: 0x0025980B File Offset: 0x00257A0B
		protected override void DoActionNoComponent(GameObject target)
		{
			base.Finish();
		}

		// Token: 0x06007EAA RID: 32426 RVA: 0x00259813 File Offset: 0x00257A13
		public override void OnUpdate()
		{
			if (!this.hasLookNpc)
			{
				base.Finish();
				return;
			}
			this.SendEvent();
		}

		// Token: 0x06007EAB RID: 32427 RVA: 0x0025982C File Offset: 0x00257A2C
		private void SendEvent()
		{
			if (!this.hasLookNpc)
			{
				return;
			}
			if (this.StoreValue != null)
			{
				this.StoreValue.Value = this.lookAnimNpc.WasFacingLeft;
			}
			if (this.lookAnimNpc.WasFacingLeft)
			{
				base.Fsm.Event(this.FacingLeft);
				return;
			}
			base.Fsm.Event(this.FacingRight);
		}

		// Token: 0x04007E5A RID: 32346
		public FsmEvent FacingLeft;

		// Token: 0x04007E5B RID: 32347
		public FsmEvent FacingRight;

		// Token: 0x04007E5C RID: 32348
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		// Token: 0x04007E5D RID: 32349
		public FsmBool EveryFrame;

		// Token: 0x04007E5E RID: 32350
		private int notTurningCount;

		// Token: 0x04007E5F RID: 32351
		private LookAnimNPC lookAnimNpc;

		// Token: 0x04007E60 RID: 32352
		private bool hasLookNpc;
	}
}
