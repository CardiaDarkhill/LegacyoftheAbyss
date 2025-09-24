using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001004 RID: 4100
	public abstract class QuaternionBaseAction : FsmStateAction
	{
		// Token: 0x060070CA RID: 28874 RVA: 0x0022C4CC File Offset: 0x0022A6CC
		public override void Awake()
		{
			if (this.everyFrame)
			{
				QuaternionBaseAction.everyFrameOptions everyFrameOptions = this.everyFrameOption;
				if (everyFrameOptions == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
				{
					base.Fsm.HandleFixedUpdate = true;
					return;
				}
				if (everyFrameOptions != QuaternionBaseAction.everyFrameOptions.LateUpdate)
				{
					return;
				}
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x0400707E RID: 28798
		[Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;

		// Token: 0x0400707F RID: 28799
		[Tooltip("Defines how to perform the action when 'every Frame' is enabled.")]
		public QuaternionBaseAction.everyFrameOptions everyFrameOption;

		// Token: 0x02001BB8 RID: 7096
		public enum everyFrameOptions
		{
			// Token: 0x04009E61 RID: 40545
			Update,
			// Token: 0x04009E62 RID: 40546
			FixedUpdate,
			// Token: 0x04009E63 RID: 40547
			LateUpdate
		}
	}
}
