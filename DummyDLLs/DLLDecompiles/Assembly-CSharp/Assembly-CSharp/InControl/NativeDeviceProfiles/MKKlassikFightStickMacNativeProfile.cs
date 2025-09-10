using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A59 RID: 2649
	[Preserve]
	[NativeInputDeviceProfile]
	public class MKKlassikFightStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056F5 RID: 22261 RVA: 0x001A9064 File Offset: 0x001A7264
		public override void Define()
		{
			base.Define();
			base.DeviceName = "MK Klassik Fight Stick";
			base.DeviceNotes = "MK Klassik Fight Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4779,
					ProductID = 771
				}
			};
		}
	}
}
