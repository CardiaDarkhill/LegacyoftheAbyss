using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A27 RID: 2599
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProVXSAMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005691 RID: 22161 RVA: 0x001A7378 File Offset: 0x001A5578
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro VX SA";
			base.DeviceNotes = "Hori Real Arcade Pro VX SA on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 62722
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21761
				}
			};
		}
	}
}
