using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A1A RID: 2586
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriFightingStickVXMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005677 RID: 22135 RVA: 0x001A6CB0 File Offset: 0x001A4EB0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Fighting Stick VX";
			base.DeviceNotes = "Hori Fighting Stick VX on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 62723
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21762
				}
			};
		}
	}
}
