using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A6E RID: 2670
	[Preserve]
	[NativeInputDeviceProfile]
	public class ProEXXbox360ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600571F RID: 22303 RVA: 0x001AA2C8 File Offset: 0x001A84C8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Pro EX Xbox 360 Controller";
			base.DeviceNotes = "Pro EX Xbox 360 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21258
				}
			};
		}
	}
}
