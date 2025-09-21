using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A50 RID: 2640
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSSF4FightStickTEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056E3 RID: 22243 RVA: 0x001A88D0 File Offset: 0x001A6AD0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz SSF4 Fight Stick TE";
			base.DeviceNotes = "Mad Catz SSF4 Fight Stick TE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 63288
				}
			};
		}
	}
}
