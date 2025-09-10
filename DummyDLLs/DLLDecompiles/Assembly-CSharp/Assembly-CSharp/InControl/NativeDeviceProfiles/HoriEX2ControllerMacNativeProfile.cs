using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A14 RID: 2580
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriEX2ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600566B RID: 22123 RVA: 0x001A6894 File Offset: 0x001A4A94
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori EX2 Controller";
			base.DeviceNotes = "Hori EX2 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 13
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 62721
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21760
				}
			};
		}
	}
}
