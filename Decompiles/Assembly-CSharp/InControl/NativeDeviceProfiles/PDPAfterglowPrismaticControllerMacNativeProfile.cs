using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A5E RID: 2654
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPAfterglowPrismaticControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056FF RID: 22271 RVA: 0x001A960C File Offset: 0x001A780C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Afterglow Prismatic Controller";
			base.DeviceNotes = "PDP Afterglow Prismatic Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 313
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 691
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 696
				}
			};
		}
	}
}
