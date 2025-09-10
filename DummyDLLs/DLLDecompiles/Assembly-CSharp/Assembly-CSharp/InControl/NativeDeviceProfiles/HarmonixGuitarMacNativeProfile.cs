using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A0D RID: 2573
	[Preserve]
	[NativeInputDeviceProfile]
	public class HarmonixGuitarMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600565D RID: 22109 RVA: 0x001A6434 File Offset: 0x001A4634
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Harmonix Guitar";
			base.DeviceNotes = "Harmonix Guitar on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 5432
				}
			};
		}
	}
}
