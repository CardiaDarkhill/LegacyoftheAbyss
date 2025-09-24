using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A2D RID: 2605
	[Preserve]
	[NativeInputDeviceProfile]
	public class KonamiDancePadMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600569D RID: 22173 RVA: 0x001A76A0 File Offset: 0x001A58A0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Konami Dance Pad";
			base.DeviceNotes = "Konami Dance Pad on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4779,
					ProductID = 4
				}
			};
		}
	}
}
