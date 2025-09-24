using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A44 RID: 2628
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzMicroConControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056CB RID: 22219 RVA: 0x001A8300 File Offset: 0x001A6500
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz MicroCon Controller";
			base.DeviceNotes = "Mad Catz MicroCon Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18230
				}
			};
		}
	}
}
