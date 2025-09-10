using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A73 RID: 2675
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerOnzaTEControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005729 RID: 22313 RVA: 0x001AA5B4 File Offset: 0x001A87B4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Onza TE Controller";
			base.DeviceNotes = "Razer Onza TE Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 64768
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5769,
					ProductID = 64768
				}
			};
		}
	}
}
