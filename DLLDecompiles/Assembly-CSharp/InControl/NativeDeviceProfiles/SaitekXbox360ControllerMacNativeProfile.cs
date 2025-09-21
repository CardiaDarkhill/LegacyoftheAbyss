using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A80 RID: 2688
	[Preserve]
	[NativeInputDeviceProfile]
	public class SaitekXbox360ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005743 RID: 22339 RVA: 0x001AADF4 File Offset: 0x001A8FF4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Saitek Xbox 360 Controller";
			base.DeviceNotes = "Saitek Xbox 360 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 51970
				}
			};
		}
	}
}
