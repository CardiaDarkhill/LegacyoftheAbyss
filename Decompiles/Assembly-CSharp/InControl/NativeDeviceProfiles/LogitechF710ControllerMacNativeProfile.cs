using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A33 RID: 2611
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechF710ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056A9 RID: 22185 RVA: 0x001A79C4 File Offset: 0x001A5BC4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech F710 Controller";
			base.DeviceNotes = "Logitech F710 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 49695
				}
			};
		}
	}
}
