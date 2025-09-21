using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A46 RID: 2630
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzMLGFightStickTEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056CF RID: 22223 RVA: 0x001A83F8 File Offset: 0x001A65F8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz MLG Fight Stick TE";
			base.DeviceNotes = "Mad Catz MLG Fight Stick TE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61502
				}
			};
		}
	}
}
