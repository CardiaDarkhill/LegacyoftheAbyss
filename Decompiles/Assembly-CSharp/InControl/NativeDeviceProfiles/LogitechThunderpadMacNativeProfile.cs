using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A35 RID: 2613
	[Preserve]
	[NativeInputDeviceProfile]
	public class LogitechThunderpadMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056AD RID: 22189 RVA: 0x001A7ABC File Offset: 0x001A5CBC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech Thunderpad";
			base.DeviceNotes = "Logitech Thunderpad on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1133,
					ProductID = 51848
				}
			};
		}
	}
}
