using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A1C RID: 2588
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriPadEXTurboControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600567B RID: 22139 RVA: 0x001A6DE8 File Offset: 0x001A4FE8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Pad EX Turbo Controller";
			base.DeviceNotes = "Hori Pad EX Turbo Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 12
				}
			};
		}
	}
}
