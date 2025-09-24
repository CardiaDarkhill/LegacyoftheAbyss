using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A71 RID: 2673
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerAtroxArcadeStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005725 RID: 22309 RVA: 0x001AA43C File Offset: 0x001A863C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Atrox Arcade Stick";
			base.DeviceNotes = "Razer Atrox Arcade Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5426,
					ProductID = 2560
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 20480
				}
			};
		}
	}
}
