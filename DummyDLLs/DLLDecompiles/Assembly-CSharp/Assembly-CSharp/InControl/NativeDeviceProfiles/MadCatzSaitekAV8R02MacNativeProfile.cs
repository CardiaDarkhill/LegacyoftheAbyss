using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A4A RID: 2634
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSaitekAV8R02MacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056D7 RID: 22231 RVA: 0x001A85E8 File Offset: 0x001A67E8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Saitek AV8R02";
			base.DeviceNotes = "Mad Catz Saitek AV8R02 on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 52009
				}
			};
		}
	}
}
