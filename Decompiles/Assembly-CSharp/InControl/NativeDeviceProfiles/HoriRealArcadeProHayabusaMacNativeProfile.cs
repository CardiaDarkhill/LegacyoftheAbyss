using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A22 RID: 2594
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProHayabusaMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005687 RID: 22151 RVA: 0x001A70D0 File Offset: 0x001A52D0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro Hayabusa";
			base.DeviceNotes = "Hori Real Arcade Pro Hayabusa on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 99
				}
			};
		}
	}
}
