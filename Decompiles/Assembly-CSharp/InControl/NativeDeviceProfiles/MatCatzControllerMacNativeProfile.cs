using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A51 RID: 2641
	[Preserve]
	[NativeInputDeviceProfile]
	public class MatCatzControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056E5 RID: 22245 RVA: 0x001A894C File Offset: 0x001A6B4C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mat Catz Controller";
			base.DeviceNotes = "Mat Catz Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61462
				}
			};
		}
	}
}
