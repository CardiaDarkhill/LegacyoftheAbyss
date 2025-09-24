using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A20 RID: 2592
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProEXPremiumVLXMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005683 RID: 22147 RVA: 0x001A6FD8 File Offset: 0x001A51D8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro EX Premium VLX";
			base.DeviceNotes = "Hori Real Arcade Pro EX Premium VLX on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 62726
				}
			};
		}
	}
}
