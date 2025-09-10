using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A37 RID: 2615
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzBeatPadMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056B1 RID: 22193 RVA: 0x001A7BB4 File Offset: 0x001A5DB4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Beat Pad";
			base.DeviceNotes = "Mad Catz Beat Pad on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18240
				}
			};
		}
	}
}
