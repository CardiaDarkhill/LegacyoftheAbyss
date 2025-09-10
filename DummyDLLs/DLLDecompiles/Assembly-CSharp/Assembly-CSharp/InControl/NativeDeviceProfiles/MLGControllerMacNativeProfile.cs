using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A5A RID: 2650
	[Preserve]
	[NativeInputDeviceProfile]
	public class MLGControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056F7 RID: 22263 RVA: 0x001A90E0 File Offset: 0x001A72E0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "MLG Controller";
			base.DeviceNotes = "MLG Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61475
				}
			};
		}
	}
}
