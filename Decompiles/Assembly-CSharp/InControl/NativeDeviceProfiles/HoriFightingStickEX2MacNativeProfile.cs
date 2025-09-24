using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A18 RID: 2584
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriFightingStickEX2MacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005673 RID: 22131 RVA: 0x001A6B40 File Offset: 0x001A4D40
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Fighting Stick EX2";
			base.DeviceNotes = "Hori Fighting Stick EX2 on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 10
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 62725
				},
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
