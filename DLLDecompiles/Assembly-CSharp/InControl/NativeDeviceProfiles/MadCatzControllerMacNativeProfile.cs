using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A3A RID: 2618
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056B7 RID: 22199 RVA: 0x001A7D28 File Offset: 0x001A5F28
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Controller";
			base.DeviceNotes = "Mad Catz Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18198
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63746
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61642
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 672
				}
			};
		}
	}
}
