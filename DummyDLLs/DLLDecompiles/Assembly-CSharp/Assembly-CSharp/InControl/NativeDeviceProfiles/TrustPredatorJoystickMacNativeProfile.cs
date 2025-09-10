using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A87 RID: 2695
	[Preserve]
	[NativeInputDeviceProfile]
	public class TrustPredatorJoystickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005751 RID: 22353 RVA: 0x001AB1D8 File Offset: 0x001A93D8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Trust Predator Joystick";
			base.DeviceNotes = "Trust Predator Joystick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 2064,
					ProductID = 3
				}
			};
		}
	}
}
