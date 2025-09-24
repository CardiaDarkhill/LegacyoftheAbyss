using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x020009FE RID: 2558
	[Preserve]
	[NativeInputDeviceProfile]
	public class AirFloControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600563F RID: 22079 RVA: 0x001A5B70 File Offset: 0x001A3D70
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Air Flo Controller";
			base.DeviceNotes = "Air Flo Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21251
				}
			};
		}
	}
}
