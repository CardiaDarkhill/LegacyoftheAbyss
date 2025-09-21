using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009FC RID: 2556
	[Preserve]
	[UnityInputDeviceProfile]
	public class XTR55_G2_WindowsUnityProfile : InputDeviceProfile
	{
		// Token: 0x0600563B RID: 22075 RVA: 0x001A5A88 File Offset: 0x001A3C88
		public override void Define()
		{
			base.Define();
			base.DeviceName = "SAILI Simulator XTR5.5 G2 FMS Controller";
			base.DeviceNotes = "SAILI Simulator XTR5.5 G2 FMS Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"Windows"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "SAILI Simulator --- XTR5.5+G2+FMS Controller"
				}
			};
		}
	}
}
