using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000ADC RID: 2780
	[Preserve]
	[NativeInputDeviceProfile]
	public class XTR55_G2_WindowsNativeProfile : InputDeviceProfile
	{
		// Token: 0x06005820 RID: 22560 RVA: 0x001BFA3C File Offset: 0x001BDC3C
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
					DriverType = InputDeviceDriverType.DirectInput,
					VendorID = 2971,
					ProductID = 16402,
					NameLiteral = "SAILI Simulator --- XTR5.5+G2+FMS Controller"
				}
			};
		}
	}
}
