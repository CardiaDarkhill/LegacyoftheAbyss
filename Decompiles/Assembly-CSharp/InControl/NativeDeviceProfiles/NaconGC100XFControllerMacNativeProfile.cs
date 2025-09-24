using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A5C RID: 2652
	[Preserve]
	[NativeInputDeviceProfile]
	public class NaconGC100XFControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056FB RID: 22267 RVA: 0x001A9218 File Offset: 0x001A7418
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Nacon GC-100XF Controller";
			base.DeviceNotes = "Nacon GC-100XF Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4553,
					ProductID = 22000
				}
			};
		}
	}
}
