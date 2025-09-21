using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A5B RID: 2651
	[Preserve]
	[NativeInputDeviceProfile]
	public class MVCTEStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056F9 RID: 22265 RVA: 0x001A915C File Offset: 0x001A735C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "MVC TE Stick";
			base.DeviceNotes = "MVC TE Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61497
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 46904
				}
			};
		}
	}
}
