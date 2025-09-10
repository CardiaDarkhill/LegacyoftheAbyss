using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A45 RID: 2629
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzMicroControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056CD RID: 22221 RVA: 0x001A837C File Offset: 0x001A657C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Micro Controller";
			base.DeviceNotes = "Mad Catz Micro Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18230
				}
			};
		}
	}
}
