using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A1F RID: 2591
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProEXMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005681 RID: 22145 RVA: 0x001A6F5C File Offset: 0x001A515C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro EX";
			base.DeviceNotes = "Hori Real Arcade Pro EX on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 62724
				}
			};
		}
	}
}
