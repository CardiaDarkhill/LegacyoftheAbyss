using System;
using System.Collections.Generic;

namespace InControl
{
	// Token: 0x02000920 RID: 2336
	public class SwitchSimpleInputDeviceManager : InputDeviceManager
	{
		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x060052B2 RID: 21170 RVA: 0x0017AD24 File Offset: 0x00178F24
		public SwitchSimpleInputDevice Device
		{
			get
			{
				return this.device;
			}
		}

		// Token: 0x060052B3 RID: 21171 RVA: 0x0017AD2C File Offset: 0x00178F2C
		public SwitchSimpleInputDeviceManager()
		{
			this.device = new SwitchSimpleInputDevice();
			this.devices.Add(this.device);
			this.Update(0UL, 0f);
		}

		// Token: 0x060052B4 RID: 21172 RVA: 0x0017AD60 File Offset: 0x00178F60
		public override void Update(ulong updateTick, float deltaTime)
		{
			if (this.device.IsConnected != this.isDeviceAttached)
			{
				if (this.device.IsConnected)
				{
					InputManager.AttachDevice(this.device);
				}
				else
				{
					InputManager.DetachDevice(this.device);
				}
				this.isDeviceAttached = this.device.IsConnected;
			}
		}

		// Token: 0x060052B5 RID: 21173 RVA: 0x0017ADB6 File Offset: 0x00178FB6
		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			return false;
		}

		// Token: 0x060052B6 RID: 21174 RVA: 0x0017ADBC File Offset: 0x00178FBC
		internal static bool Enable()
		{
			List<string> list = new List<string>();
			try
			{
				if (!SwitchSimpleInputDeviceManager.CheckPlatformSupport(list))
				{
					return false;
				}
				InputManager.AddDeviceManager<SwitchSimpleInputDeviceManager>();
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

		// Token: 0x040052D5 RID: 21205
		private SwitchSimpleInputDevice device;

		// Token: 0x040052D6 RID: 21206
		private bool isDeviceAttached;
	}
}
