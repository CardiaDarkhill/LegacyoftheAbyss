using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A17 RID: 2583
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriFightingEdgeArcadeStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005671 RID: 22129 RVA: 0x001A6AC4 File Offset: 0x001A4CC4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Fighting Edge Arcade Stick";
			base.DeviceNotes = "Hori Fighting Edge Arcade Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21763
				}
			};
		}
	}
}
