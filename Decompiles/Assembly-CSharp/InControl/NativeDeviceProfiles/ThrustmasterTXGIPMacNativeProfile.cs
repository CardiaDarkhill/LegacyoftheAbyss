using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A86 RID: 2694
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustmasterTXGIPMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600574F RID: 22351 RVA: 0x001AB15C File Offset: 0x001A935C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Thrustmaster TX GIP";
			base.DeviceNotes = "Thrustmaster TX GIP on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1103,
					ProductID = 46692
				}
			};
		}
	}
}
