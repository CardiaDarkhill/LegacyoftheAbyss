using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A47 RID: 2631
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzNeoFightStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056D1 RID: 22225 RVA: 0x001A8474 File Offset: 0x001A6674
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Neo Fight Stick";
			base.DeviceNotes = "Mad Catz Neo Fight Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61498
				}
			};
		}
	}
}
