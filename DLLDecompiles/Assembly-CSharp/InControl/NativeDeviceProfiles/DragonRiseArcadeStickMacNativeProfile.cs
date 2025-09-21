using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A06 RID: 2566
	[Preserve]
	[NativeInputDeviceProfile]
	public class DragonRiseArcadeStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600564F RID: 22095 RVA: 0x001A5FD0 File Offset: 0x001A41D0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "DragonRise Arcade Stick";
			base.DeviceNotes = "DragonRise Arcade Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 121,
					ProductID = 6268
				}
			};
		}
	}
}
