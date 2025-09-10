using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A2E RID: 2606
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechChillStreamControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600569F RID: 22175 RVA: 0x001A7718 File Offset: 0x001A5918
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech Chill Stream Controller";
			base.DeviceNotes = "Logitech Chill Stream Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 49730
				}
			};
		}
	}
}
