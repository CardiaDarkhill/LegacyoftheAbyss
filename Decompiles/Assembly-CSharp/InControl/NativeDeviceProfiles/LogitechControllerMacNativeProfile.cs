using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A2F RID: 2607
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056A1 RID: 22177 RVA: 0x001A7794 File Offset: 0x001A5994
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech Controller";
			base.DeviceNotes = "Logitech Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 62209
				}
			};
		}
	}
}
