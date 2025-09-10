using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E85 RID: 3717
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an Event when the mobile device is shaken.")]
	public class DeviceShakeEvent : FsmStateAction
	{
		// Token: 0x060069B3 RID: 27059 RVA: 0x00211346 File Offset: 0x0020F546
		public override void Reset()
		{
			this.shakeThreshold = 3f;
			this.sendEvent = null;
		}

		// Token: 0x060069B4 RID: 27060 RVA: 0x00211360 File Offset: 0x0020F560
		public override void OnUpdate()
		{
			if (ActionHelpers.GetDeviceAcceleration().sqrMagnitude > this.shakeThreshold.Value * this.shakeThreshold.Value)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x040068DD RID: 26845
		[RequiredField]
		[Tooltip("Amount of acceleration required to trigger the event. Higher numbers require a harder shake.")]
		public FsmFloat shakeThreshold;

		// Token: 0x040068DE RID: 26846
		[RequiredField]
		[Tooltip("Event to send when Shake Threshold is exceeded.")]
		public FsmEvent sendEvent;
	}
}
