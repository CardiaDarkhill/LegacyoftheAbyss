using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A16 RID: 2582
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriFightingCommanderMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600566F RID: 22127 RVA: 0x001A6A08 File Offset: 0x001A4C08
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Fighting Commander";
			base.DeviceNotes = "Hori Fighting Commander on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 197
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21776
				}
			};
		}
	}
}
