using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A00 RID: 2560
	[Preserve]
	[NativeInputDeviceProfile]
	public class AtPlayControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005643 RID: 22083 RVA: 0x001A5C68 File Offset: 0x001A3E68
		public override void Define()
		{
			base.Define();
			base.DeviceName = "At Play Controller";
			base.DeviceNotes = "At Play Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 64250
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 64251
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 690
				}
			};
		}
	}
}
