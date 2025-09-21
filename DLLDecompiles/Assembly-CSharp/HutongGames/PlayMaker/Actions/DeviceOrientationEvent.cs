using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E84 RID: 3716
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an Event based on the Orientation of the mobile device.")]
	public class DeviceOrientationEvent : FsmStateAction
	{
		// Token: 0x060069AE RID: 27054 RVA: 0x002112E9 File Offset: 0x0020F4E9
		public override void Reset()
		{
			this.orientation = DeviceOrientation.Portrait;
			this.sendEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x060069AF RID: 27055 RVA: 0x00211300 File Offset: 0x0020F500
		public override void OnEnter()
		{
			this.DoDetectDeviceOrientation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069B0 RID: 27056 RVA: 0x00211316 File Offset: 0x0020F516
		public override void OnUpdate()
		{
			this.DoDetectDeviceOrientation();
		}

		// Token: 0x060069B1 RID: 27057 RVA: 0x0021131E File Offset: 0x0020F51E
		private void DoDetectDeviceOrientation()
		{
			if (Input.deviceOrientation == this.orientation)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x040068DA RID: 26842
		[Tooltip("Note: If device is physically situated between discrete positions, as when (for example) rotated diagonally, system will report Unknown orientation.")]
		public DeviceOrientation orientation;

		// Token: 0x040068DB RID: 26843
		[Tooltip("The event to send if the device orientation matches Orientation.")]
		public FsmEvent sendEvent;

		// Token: 0x040068DC RID: 26844
		[Tooltip("Repeat every frame. Useful if you want to wait for the orientation to be true.")]
		public bool everyFrame;
	}
}
