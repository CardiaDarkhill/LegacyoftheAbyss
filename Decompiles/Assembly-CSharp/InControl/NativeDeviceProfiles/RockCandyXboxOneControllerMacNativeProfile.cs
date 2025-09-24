using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A7F RID: 2687
	[Preserve]
	[NativeInputDeviceProfile]
	public class RockCandyXboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x06005741 RID: 22337 RVA: 0x001AACB8 File Offset: 0x001A8EB8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Rock Candy Xbox One Controller";
			base.DeviceNotes = "Rock Candy Xbox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 326
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 582
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 838
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 719
				}
			};
		}
	}
}
