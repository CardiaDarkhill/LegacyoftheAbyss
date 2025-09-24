using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A0C RID: 2572
	[Preserve]
	[NativeInputDeviceProfile]
	public class HarmonixDrumKitMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600565B RID: 22107 RVA: 0x001A63B8 File Offset: 0x001A45B8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Harmonix Drum Kit";
			base.DeviceNotes = "Harmonix Drum Kit on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 4408
				}
			};
		}
	}
}
