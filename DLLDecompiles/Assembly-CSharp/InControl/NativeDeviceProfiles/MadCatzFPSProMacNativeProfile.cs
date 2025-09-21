using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A3F RID: 2623
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzFPSProMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056C1 RID: 22209 RVA: 0x001A8094 File Offset: 0x001A6294
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz FPS Pro";
			base.DeviceNotes = "Mad Catz FPS Pro on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61479
				}
			};
		}
	}
}
