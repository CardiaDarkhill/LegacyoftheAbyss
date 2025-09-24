using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A8A RID: 2698
	[Preserve]
	[NativeInputDeviceProfile]
	public class Xbox360ProEXControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005757 RID: 22359 RVA: 0x001AB348 File Offset: 0x001A9548
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Xbox 360 Pro EX Controller";
			base.DeviceNotes = "Xbox 360 Pro EX Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 8406,
					ProductID = 10271
				}
			};
		}
	}
}
