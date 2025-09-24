using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A2C RID: 2604
	[Preserve]
	[NativeInputDeviceProfile]
	public class JoytekXbox360ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600569B RID: 22171 RVA: 0x001A7624 File Offset: 0x001A5824
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Joytek Xbox 360 Controller";
			base.DeviceNotes = "Joytek Xbox 360 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5678,
					ProductID = 48879
				}
			};
		}
	}
}
