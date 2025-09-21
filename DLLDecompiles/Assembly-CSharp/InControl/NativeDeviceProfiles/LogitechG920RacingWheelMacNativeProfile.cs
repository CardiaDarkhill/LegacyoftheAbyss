using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A34 RID: 2612
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechG920RacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056AB RID: 22187 RVA: 0x001A7A40 File Offset: 0x001A5C40
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech G920 Racing Wheel";
			base.DeviceNotes = "Logitech G920 Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 49761
				}
			};
		}
	}
}
