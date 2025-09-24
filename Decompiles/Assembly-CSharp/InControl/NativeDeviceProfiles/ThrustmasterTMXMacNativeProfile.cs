using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A85 RID: 2693
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustmasterTMXMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600574D RID: 22349 RVA: 0x001AB0E0 File Offset: 0x001A92E0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Thrustmaster TMX";
			base.DeviceNotes = "Thrustmaster TMX on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1103,
					ProductID = 46718
				}
			};
		}
	}
}
