using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A39 RID: 2617
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzCODControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056B5 RID: 22197 RVA: 0x001A7CAC File Offset: 0x001A5EAC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz COD Controller";
			base.DeviceNotes = "Mad Catz COD Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61477
				}
			};
		}
	}
}
