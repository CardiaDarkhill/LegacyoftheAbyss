using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A13 RID: 2579
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriEdgeFightingStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005669 RID: 22121 RVA: 0x001A6818 File Offset: 0x001A4A18
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Edge Fighting Stick";
			base.DeviceNotes = "Hori Edge Fighting Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 109
				}
			};
		}
	}
}
