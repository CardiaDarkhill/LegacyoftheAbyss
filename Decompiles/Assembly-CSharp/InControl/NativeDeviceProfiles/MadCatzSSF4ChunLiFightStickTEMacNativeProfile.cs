using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A4F RID: 2639
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSSF4ChunLiFightStickTEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056E1 RID: 22241 RVA: 0x001A8854 File Offset: 0x001A6A54
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz SSF4 Chun-Li Fight Stick TE";
			base.DeviceNotes = "Mad Catz SSF4 Chun-Li Fight Stick TE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61501
				}
			};
		}
	}
}
