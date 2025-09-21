using System;
using System.Collections.Generic;

namespace InControl
{
	// Token: 0x0200091C RID: 2332
	public class PS4SimpleInputDeviceManager : InputDeviceManager
	{
		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x0600529F RID: 21151 RVA: 0x0017A723 File Offset: 0x00178923
		public PS4SimpleInputDevice Device
		{
			get
			{
				return this.device;
			}
		}

		// Token: 0x060052A0 RID: 21152 RVA: 0x0017A72B File Offset: 0x0017892B
		public PS4SimpleInputDeviceManager()
		{
			this.device = new PS4SimpleInputDevice();
			this.device.CustomPlayerID = 0;
			this.devices.Add(this.device);
			this.Update(0UL, 0f);
		}

		// Token: 0x060052A1 RID: 21153 RVA: 0x0017A768 File Offset: 0x00178968
		public override void Update(ulong updateTick, float deltaTime)
		{
			if (!this.isDeviceAttached)
			{
				InputManager.AttachDevice(this.device);
				this.isDeviceAttached = true;
			}
		}

		// Token: 0x060052A2 RID: 21154 RVA: 0x0017A784 File Offset: 0x00178984
		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			return false;
		}

		// Token: 0x060052A3 RID: 21155 RVA: 0x0017A788 File Offset: 0x00178988
		internal static bool Enable()
		{
			List<string> list = new List<string>();
			try
			{
				if (!PS4SimpleInputDeviceManager.CheckPlatformSupport(list))
				{
					return false;
				}
				InputManager.AddDeviceManager<PS4SimpleInputDeviceManager>();
			}
			finally
			{
				foreach (string text in list)
				{
					Logger.LogError(text);
				}
			}
			return true;
		}

		// Token: 0x040052CB RID: 21195
		private PS4SimpleInputDevice device;

		// Token: 0x040052CC RID: 21196
		private bool isDeviceAttached;
	}
}
