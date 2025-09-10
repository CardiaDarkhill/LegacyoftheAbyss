using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A1B RID: 2587
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriFightStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005679 RID: 22137 RVA: 0x001A6D6C File Offset: 0x001A4F6C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Fight Stick";
			base.DeviceNotes = "Hori Fight Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 13
				}
			};
		}
	}
}
