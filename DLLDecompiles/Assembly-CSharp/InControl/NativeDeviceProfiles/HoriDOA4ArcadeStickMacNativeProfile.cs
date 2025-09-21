using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A12 RID: 2578
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriDOA4ArcadeStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005667 RID: 22119 RVA: 0x001A679C File Offset: 0x001A499C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori DOA4 Arcade Stick";
			base.DeviceNotes = "Hori DOA4 Arcade Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 10
				}
			};
		}
	}
}
