using System;
using JetBrains.Annotations;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C85 RID: 3205
	[Tooltip("Relinquishes hero control, gracefully ending current actions (e.g., sprinting)")]
	[UsedImplicitly]
	public class HeroRelinquishControlDynamic : FsmStateAction
	{
		// Token: 0x0600606F RID: 24687 RVA: 0x001E86E4 File Offset: 0x001E68E4
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
			if (this.hc.cState.isSprinting)
			{
				this.hc.sprintFSM.SendEvent("SKID END");
				return;
			}
			this.hc.RelinquishControl();
			base.Finish();
		}

		// Token: 0x06006070 RID: 24688 RVA: 0x001E8735 File Offset: 0x001E6935
		public override void OnUpdate()
		{
			if (this.hc.controlReqlinquished)
			{
				return;
			}
			this.hc.RelinquishControl();
			base.Finish();
		}

		// Token: 0x06006071 RID: 24689 RVA: 0x001E8756 File Offset: 0x001E6956
		public override void OnExit()
		{
			if (!this.hc.controlReqlinquished)
			{
				this.hc.RelinquishControl();
			}
		}

		// Token: 0x04005DD4 RID: 24020
		private HeroController hc;
	}
}
