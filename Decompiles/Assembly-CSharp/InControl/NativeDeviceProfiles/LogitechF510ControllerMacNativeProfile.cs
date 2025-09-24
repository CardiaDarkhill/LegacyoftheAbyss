using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A32 RID: 2610
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechF510ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056A7 RID: 22183 RVA: 0x001A7948 File Offset: 0x001A5B48
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech F510 Controller";
			base.DeviceNotes = "Logitech F510 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 49694
				}
			};
		}
	}
}
