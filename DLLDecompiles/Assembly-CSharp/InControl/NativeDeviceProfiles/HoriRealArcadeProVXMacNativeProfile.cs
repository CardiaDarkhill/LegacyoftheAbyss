using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A26 RID: 2598
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProVXMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600568F RID: 22159 RVA: 0x001A72FC File Offset: 0x001A54FC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro VX";
			base.DeviceNotes = "Hori Real Arcade Pro VX on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 27
				}
			};
		}
	}
}
