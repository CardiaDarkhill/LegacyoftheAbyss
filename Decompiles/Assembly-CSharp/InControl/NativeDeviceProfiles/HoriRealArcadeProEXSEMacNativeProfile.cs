using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A21 RID: 2593
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProEXSEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005685 RID: 22149 RVA: 0x001A7054 File Offset: 0x001A5254
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro EX SE";
			base.DeviceNotes = "Hori Real Arcade Pro EX SE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 22
				}
			};
		}
	}
}
