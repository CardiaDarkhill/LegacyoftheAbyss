using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCB RID: 3019
	public class BoolKeepValue : FsmStateAction
	{
		// Token: 0x06005CB5 RID: 23733 RVA: 0x001D2BC2 File Offset: 0x001D0DC2
		public override void Reset()
		{
			this.MonitorBool = null;
			this.KeepBool = null;
			this.TargetValue = null;
			this.EveryFrame = true;
		}

		// Token: 0x06005CB6 RID: 23734 RVA: 0x001D2BE0 File Offset: 0x001D0DE0
		public override void OnEnter()
		{
			this.CheckValues();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CB7 RID: 23735 RVA: 0x001D2BF6 File Offset: 0x001D0DF6
		public override void OnUpdate()
		{
			this.CheckValues();
		}

		// Token: 0x06005CB8 RID: 23736 RVA: 0x001D2BFE File Offset: 0x001D0DFE
		private void CheckValues()
		{
			if (this.MonitorBool.Value != this.TargetValue.Value)
			{
				return;
			}
			this.KeepBool.Value = this.TargetValue.Value;
			base.Finish();
		}

		// Token: 0x0400584A RID: 22602
		[UIHint(UIHint.Variable)]
		public FsmBool MonitorBool;

		// Token: 0x0400584B RID: 22603
		[UIHint(UIHint.Variable)]
		public FsmBool KeepBool;

		// Token: 0x0400584C RID: 22604
		public FsmBool TargetValue;

		// Token: 0x0400584D RID: 22605
		public bool EveryFrame;
	}
}
