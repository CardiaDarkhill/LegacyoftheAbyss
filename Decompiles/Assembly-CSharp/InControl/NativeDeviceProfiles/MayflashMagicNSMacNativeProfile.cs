using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A52 RID: 2642
	[Preserve]
	[NativeInputDeviceProfile]
	public class MayflashMagicNSMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056E7 RID: 22247 RVA: 0x001A89C8 File Offset: 0x001A6BC8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mayflash Magic-NS";
			base.DeviceNotes = "Mayflash Magic-NS on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 121,
					ProductID = 6355
				}
			};
		}
	}
}
