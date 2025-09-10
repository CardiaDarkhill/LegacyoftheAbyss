using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200129C RID: 4764
	public class CheckIsBlackThreaded : FsmStateAction
	{
		// Token: 0x06007D0C RID: 32012 RVA: 0x002554CE File Offset: 0x002536CE
		public override void Reset()
		{
			this.Target = null;
			this.TrueEvent = null;
			this.FalseEvent = null;
			this.StoreValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007D0D RID: 32013 RVA: 0x002554F4 File Offset: 0x002536F4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.blackThreadState = safe.GetComponentInParent<BlackThreadState>(true);
			}
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007D0E RID: 32014 RVA: 0x00255537 File Offset: 0x00253737
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007D0F RID: 32015 RVA: 0x00255540 File Offset: 0x00253740
		private void DoAction()
		{
			bool flag = this.blackThreadState && this.blackThreadState.IsVisiblyThreaded;
			this.StoreValue.Value = flag;
			base.Fsm.Event(flag ? this.TrueEvent : this.FalseEvent);
		}

		// Token: 0x04007D1C RID: 32028
		public FsmOwnerDefault Target;

		// Token: 0x04007D1D RID: 32029
		public FsmEvent TrueEvent;

		// Token: 0x04007D1E RID: 32030
		public FsmEvent FalseEvent;

		// Token: 0x04007D1F RID: 32031
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		// Token: 0x04007D20 RID: 32032
		public bool EveryFrame;

		// Token: 0x04007D21 RID: 32033
		private BlackThreadState blackThreadState;
	}
}
