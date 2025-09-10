using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A24 RID: 2596
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProVHayabusaMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600568B RID: 22155 RVA: 0x001A71C8 File Offset: 0x001A53C8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro V Hayabusa";
			base.DeviceNotes = "Hori Real Arcade Pro V Hayabusa on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 216
				}
			};
		}
	}
}
