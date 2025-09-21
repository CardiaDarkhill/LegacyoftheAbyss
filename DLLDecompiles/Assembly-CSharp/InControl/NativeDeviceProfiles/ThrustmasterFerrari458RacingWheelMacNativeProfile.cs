using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A82 RID: 2690
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustmasterFerrari458RacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005747 RID: 22343 RVA: 0x001AAEEC File Offset: 0x001A90EC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Thrustmaster Ferrari 458 Racing Wheel";
			base.DeviceNotes = "Thrustmaster Ferrari 458 Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 23296
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 23299
				}
			};
		}
	}
}
