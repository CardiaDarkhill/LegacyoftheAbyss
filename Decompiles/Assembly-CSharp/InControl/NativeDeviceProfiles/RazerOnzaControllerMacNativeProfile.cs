using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A72 RID: 2674
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerOnzaControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005727 RID: 22311 RVA: 0x001AA4F8 File Offset: 0x001A86F8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Onza Controller";
			base.DeviceNotes = "Razer Onza Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 64769
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5769,
					ProductID = 64769
				}
			};
		}
	}
}
