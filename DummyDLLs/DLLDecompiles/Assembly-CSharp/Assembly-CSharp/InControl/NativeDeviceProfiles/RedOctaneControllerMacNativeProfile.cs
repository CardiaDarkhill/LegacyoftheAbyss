using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A78 RID: 2680
	[Preserve]
	[NativeInputDeviceProfile]
	public class RedOctaneControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005733 RID: 22323 RVA: 0x001AA89C File Offset: 0x001A8A9C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Red Octane Controller";
			base.DeviceNotes = "Red Octane Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5168,
					ProductID = 63489
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5168,
					ProductID = 672
				}
			};
		}
	}
}
