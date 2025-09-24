using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A49 RID: 2633
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzProControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056D5 RID: 22229 RVA: 0x001A856C File Offset: 0x001A676C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Pro Controller";
			base.DeviceNotes = "Mad Catz Pro Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18214
				}
			};
		}
	}
}
