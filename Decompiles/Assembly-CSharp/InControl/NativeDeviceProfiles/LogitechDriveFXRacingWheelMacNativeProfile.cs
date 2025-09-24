using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A30 RID: 2608
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechDriveFXRacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056A3 RID: 22179 RVA: 0x001A7810 File Offset: 0x001A5A10
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech DriveFX Racing Wheel";
			base.DeviceNotes = "Logitech DriveFX Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 51875
				}
			};
		}
	}
}
