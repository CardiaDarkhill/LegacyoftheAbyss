using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A05 RID: 2565
	[Preserve]
	[NativeInputDeviceProfile]
	public class BrookPS2ConverterMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600564D RID: 22093 RVA: 0x001A5F54 File Offset: 0x001A4154
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Brook PS2 Converter";
			base.DeviceNotes = "Brook PS2 Converter on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3090,
					ProductID = 2289
				}
			};
		}
	}
}
