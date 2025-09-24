using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A38 RID: 2616
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzBrawlStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056B3 RID: 22195 RVA: 0x001A7C30 File Offset: 0x001A5E30
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Brawl Stick";
			base.DeviceNotes = "Mad Catz Brawl Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61465
				}
			};
		}
	}
}
