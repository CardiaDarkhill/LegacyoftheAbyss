using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C26 RID: 3110
	public class EnableFsmSelf : FsmStateAction
	{
		// Token: 0x06005EAB RID: 24235 RVA: 0x001DF2E4 File Offset: 0x001DD4E4
		public override void Reset()
		{
			this.SetEnabled = null;
		}

		// Token: 0x06005EAC RID: 24236 RVA: 0x001DF2ED File Offset: 0x001DD4ED
		public override void OnEnter()
		{
			if (!this.SetEnabled.IsNone)
			{
				base.Fsm.Owner.enabled = this.SetEnabled.Value;
			}
			base.Finish();
		}

		// Token: 0x04005B3C RID: 23356
		public FsmBool SetEnabled;
	}
}
