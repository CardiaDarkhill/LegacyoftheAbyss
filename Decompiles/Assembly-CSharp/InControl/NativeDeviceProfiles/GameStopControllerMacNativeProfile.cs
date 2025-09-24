using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A0A RID: 2570
	[Preserve]
	[NativeInputDeviceProfile]
	public class GameStopControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005657 RID: 22103 RVA: 0x001A6200 File Offset: 0x001A4400
		public override void Define()
		{
			base.Define();
			base.DeviceName = "GameStop Controller";
			base.DeviceNotes = "GameStop Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 1025
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 769
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4779,
					ProductID = 770
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63745
				}
			};
		}
	}
}
