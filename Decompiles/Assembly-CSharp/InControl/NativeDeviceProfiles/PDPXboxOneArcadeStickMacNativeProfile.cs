using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A66 RID: 2662
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPXboxOneArcadeStickMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x0600570F RID: 22287 RVA: 0x001A9A6C File Offset: 0x001A7C6C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Xbox One Arcade Stick";
			base.DeviceNotes = "PDP Xbox One Arcade Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 348
				}
			};
		}
	}
}
