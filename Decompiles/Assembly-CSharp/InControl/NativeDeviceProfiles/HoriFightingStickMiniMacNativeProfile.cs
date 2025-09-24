using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A19 RID: 2585
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriFightingStickMiniMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005675 RID: 22133 RVA: 0x001A6C34 File Offset: 0x001A4E34
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Fighting Stick Mini";
			base.DeviceNotes = "Hori Fighting Stick Mini on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 237
				}
			};
		}
	}
}
