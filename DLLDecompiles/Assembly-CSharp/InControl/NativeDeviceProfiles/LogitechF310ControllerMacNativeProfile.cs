using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A31 RID: 2609
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechF310ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056A5 RID: 22181 RVA: 0x001A788C File Offset: 0x001A5A8C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech F310 Controller";
			base.DeviceNotes = "Logitech F310 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 49693
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 49686
				}
			};
		}
	}
}
