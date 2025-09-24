using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A3B RID: 2619
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzFightPadControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056B9 RID: 22201 RVA: 0x001A7E64 File Offset: 0x001A6064
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz FightPad Controller";
			base.DeviceNotes = "Mad Catz FightPad Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61480
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18216
				}
			};
		}
	}
}
