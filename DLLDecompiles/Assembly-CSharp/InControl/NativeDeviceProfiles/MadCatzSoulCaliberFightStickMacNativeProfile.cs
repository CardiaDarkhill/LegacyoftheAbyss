using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A4E RID: 2638
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSoulCaliberFightStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056DF RID: 22239 RVA: 0x001A87D8 File Offset: 0x001A69D8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Soul Caliber Fight Stick";
			base.DeviceNotes = "Mad Catz Soul Caliber Fight Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61503
				}
			};
		}
	}
}
