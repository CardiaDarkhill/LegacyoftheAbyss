using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A15 RID: 2581
	[Preserve]
	[NativeInputDeviceProfile]
	public class HORIFightingCommanderControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600566D RID: 22125 RVA: 0x001A698C File Offset: 0x001A4B8C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "HORI Fighting Commander Controller";
			base.DeviceNotes = "HORI Fighting Commander Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 134
				}
			};
		}
	}
}
