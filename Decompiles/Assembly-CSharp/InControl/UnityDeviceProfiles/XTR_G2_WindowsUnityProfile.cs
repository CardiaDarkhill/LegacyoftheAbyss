using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009FD RID: 2557
	[Preserve]
	[UnityInputDeviceProfile]
	public class XTR_G2_WindowsUnityProfile : InputDeviceProfile
	{
		// Token: 0x0600563D RID: 22077 RVA: 0x001A5AFC File Offset: 0x001A3CFC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "KMODEL Simulator XTR G2 FMS Controller";
			base.DeviceNotes = "KMODEL Simulator XTR G2 FMS Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"Windows"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "KMODEL Simulator - XTR+G2+FMS Controller"
				}
			};
		}
	}
}
