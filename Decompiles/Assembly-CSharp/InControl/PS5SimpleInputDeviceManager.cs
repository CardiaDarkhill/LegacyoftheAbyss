using System;
using System.Collections.Generic;

namespace InControl
{
	// Token: 0x0200091E RID: 2334
	public class PS5SimpleInputDeviceManager : InputDeviceManager
	{
		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x060052A8 RID: 21160 RVA: 0x0017AA1C File Offset: 0x00178C1C
		public PS5SimpleInputDevice Device
		{
			get
			{
				return this.device;
			}
		}

		// Token: 0x060052A9 RID: 21161 RVA: 0x0017AA24 File Offset: 0x00178C24
		public PS5SimpleInputDeviceManager()
		{
			this.device = new PS5SimpleInputDevice();
			this.device.CustomPlayerID = 0;
			this.devices.Add(this.device);
			this.Update(0UL, 0f);
		}

		// Token: 0x060052AA RID: 21162 RVA: 0x0017AA61 File Offset: 0x00178C61
		public override void Update(ulong updateTick, float deltaTime)
		{
			if (!this.isDeviceAttached)
			{
				InputManager.AttachDevice(this.device);
				this.isDeviceAttached = true;
			}
		}

		// Token: 0x060052AB RID: 21163 RVA: 0x0017AA7D File Offset: 0x00178C7D
		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			return false;
		}

		// Token: 0x060052AC RID: 21164 RVA: 0x0017AA80 File Offset: 0x00178C80
		internal static bool Enable()
		{
			List<string> list = new List<string>();
			try
			{
				if (!PS5SimpleInputDeviceManager.CheckPlatformSupport(list))
				{
					return false;
				}
				InputManager.AddDeviceManager<PS5SimpleInputDeviceManager>();
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

		// Token: 0x040052D0 RID: 21200
		private PS5SimpleInputDevice device;

		// Token: 0x040052D1 RID: 21201
		private bool isDeviceAttached;
	}
}
