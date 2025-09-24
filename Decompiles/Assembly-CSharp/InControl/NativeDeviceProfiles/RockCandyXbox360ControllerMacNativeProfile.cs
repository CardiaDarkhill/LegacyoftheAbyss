using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A7E RID: 2686
	[Preserve]
	[NativeInputDeviceProfile]
	public class RockCandyXbox360ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600573F RID: 22335 RVA: 0x001AABBC File Offset: 0x001A8DBC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Rock Candy Xbox 360 Controller";
			base.DeviceNotes = "Rock Candy Xbox 360 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 543
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 64254
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 338
				}
			};
		}
	}
}
