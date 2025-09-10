using System;

namespace InControl
{
	// Token: 0x020008EC RID: 2284
	public class UnknownDeviceBindingSourceListener : BindingSourceListener
	{
		// Token: 0x0600500D RID: 20493 RVA: 0x0017216C File Offset: 0x0017036C
		public void Reset()
		{
			this.detectFound = UnknownDeviceControl.None;
			this.detectPhase = UnknownDeviceBindingSourceListener.DetectPhase.WaitForInitialRelease;
			this.TakeSnapshotOnUnknownDevices();
		}

		// Token: 0x0600500E RID: 20494 RVA: 0x00172188 File Offset: 0x00170388
		private void TakeSnapshotOnUnknownDevices()
		{
			int count = InputManager.Devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputDevice inputDevice = InputManager.Devices[i];
				if (inputDevice.IsUnknown)
				{
					inputDevice.TakeSnapshot();
				}
			}
		}

		// Token: 0x0600500F RID: 20495 RVA: 0x001721C8 File Offset: 0x001703C8
		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (!listenOptions.IncludeUnknownControllers || device.IsKnown)
			{
				return null;
			}
			if (this.detectPhase == UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlRelease && this.detectFound && !this.IsPressed(this.detectFound, device))
			{
				BindingSource result = new UnknownDeviceBindingSource(this.detectFound);
				this.Reset();
				return result;
			}
			UnknownDeviceControl control = this.ListenForControl(listenOptions, device);
			if (control)
			{
				if (this.detectPhase == UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlPress)
				{
					this.detectFound = control;
					this.detectPhase = UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlRelease;
				}
			}
			else if (this.detectPhase == UnknownDeviceBindingSourceListener.DetectPhase.WaitForInitialRelease)
			{
				this.detectPhase = UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlPress;
			}
			return null;
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x00172258 File Offset: 0x00170458
		private bool IsPressed(UnknownDeviceControl control, InputDevice device)
		{
			return Utility.AbsoluteIsOverThreshold(control.GetValue(device), 0.5f);
		}

		// Token: 0x06005011 RID: 20497 RVA: 0x0017226C File Offset: 0x0017046C
		private UnknownDeviceControl ListenForControl(BindingListenOptions listenOptions, InputDevice device)
		{
			if (device.IsUnknown)
			{
				UnknownDeviceControl firstPressedButton = device.GetFirstPressedButton();
				if (firstPressedButton)
				{
					return firstPressedButton;
				}
				UnknownDeviceControl firstPressedAnalog = device.GetFirstPressedAnalog();
				if (firstPressedAnalog)
				{
					return firstPressedAnalog;
				}
			}
			return UnknownDeviceControl.None;
		}

		// Token: 0x0400509A RID: 20634
		private UnknownDeviceControl detectFound;

		// Token: 0x0400509B RID: 20635
		private UnknownDeviceBindingSourceListener.DetectPhase detectPhase;

		// Token: 0x02001B5B RID: 7003
		private enum DetectPhase
		{
			// Token: 0x04009C77 RID: 40055
			WaitForInitialRelease,
			// Token: 0x04009C78 RID: 40056
			WaitForControlPress,
			// Token: 0x04009C79 RID: 40057
			WaitForControlRelease
		}
	}
}
