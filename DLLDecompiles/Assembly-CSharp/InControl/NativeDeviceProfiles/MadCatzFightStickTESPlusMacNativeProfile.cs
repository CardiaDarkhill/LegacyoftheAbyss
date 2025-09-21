using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A3E RID: 2622
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzFightStickTESPlusMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056BF RID: 22207 RVA: 0x001A8018 File Offset: 0x001A6218
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Fight Stick TES Plus";
			base.DeviceNotes = "Mad Catz Fight Stick TES Plus on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61506
				}
			};
		}
	}
}
