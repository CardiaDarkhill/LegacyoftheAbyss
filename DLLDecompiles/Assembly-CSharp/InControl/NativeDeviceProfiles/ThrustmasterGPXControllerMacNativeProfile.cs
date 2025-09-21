using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A84 RID: 2692
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustmasterGPXControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600574B RID: 22347 RVA: 0x001AB024 File Offset: 0x001A9224
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Thrustmaster GPX Controller";
			base.DeviceNotes = "Thrustmaster GPX Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1103,
					ProductID = 45862
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 23298
				}
			};
		}
	}
}
