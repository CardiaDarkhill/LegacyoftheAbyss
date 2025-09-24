using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A65 RID: 2661
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPXbox360ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600570D RID: 22285 RVA: 0x001A99F0 File Offset: 0x001A7BF0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Xbox 360 Controller";
			base.DeviceNotes = "PDP Xbox 360 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 1281
				}
			};
		}
	}
}
