using System;
using System.Collections.Generic;
using SharpDX.DirectInput;

namespace InControl
{
	// Token: 0x02000913 RID: 2323
	public static class DirectInputManager
	{
		// Token: 0x0600525F RID: 21087 RVA: 0x00178A94 File Offset: 0x00176C94
		public static void EnumerateDevices()
		{
			DirectInputManager.devices.Clear();
			foreach (DeviceInstance item in DirectInputManager.directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly))
			{
				DirectInputManager.devices.Add(item);
			}
		}

		// Token: 0x06005260 RID: 21088 RVA: 0x00178AF8 File Offset: 0x00176CF8
		public static List<DeviceInstance> GetDevices()
		{
			DirectInputManager.EnumerateDevices();
			return new List<DeviceInstance>(DirectInputManager.devices);
		}

		// Token: 0x06005261 RID: 21089 RVA: 0x00178B0C File Offset: 0x00176D0C
		public static DeviceInstance GetDevice(Guid deviceGuid)
		{
			DirectInputManager.EnumerateDevices();
			return DirectInputManager.devices.Find((DeviceInstance device) => device.InstanceGuid == deviceGuid);
		}

		// Token: 0x0400529A RID: 21146
		private static DirectInput directInput = new DirectInput();

		// Token: 0x0400529B RID: 21147
		private static List<DeviceInstance> devices = new List<DeviceInstance>();

		// Token: 0x0400529C RID: 21148
		private static bool init;
	}
}
