using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A3C RID: 2620
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzFightPadMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056BB RID: 22203 RVA: 0x001A7F20 File Offset: 0x001A6120
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz FightPad";
			base.DeviceNotes = "Mad Catz FightPad on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61486
				}
			};
		}
	}
}
