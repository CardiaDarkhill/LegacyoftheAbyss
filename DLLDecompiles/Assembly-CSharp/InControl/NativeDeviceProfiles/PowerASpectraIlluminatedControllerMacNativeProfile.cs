using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A6D RID: 2669
	[Preserve]
	[NativeInputDeviceProfile]
	public class PowerASpectraIlluminatedControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600571D RID: 22301 RVA: 0x001AA24C File Offset: 0x001A844C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PowerA Spectra Illuminated Controller";
			base.DeviceNotes = "PowerA Spectra Illuminated Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21546
				}
			};
		}
	}
}
