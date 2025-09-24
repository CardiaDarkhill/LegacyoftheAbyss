using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A01 RID: 2561
	[Preserve]
	[NativeInputDeviceProfile]
	public class BatarangControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005645 RID: 22085 RVA: 0x001A5D64 File Offset: 0x001A3F64
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Batarang Controller";
			base.DeviceNotes = "Batarang Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5604,
					ProductID = 16144
				}
			};
		}
	}
}
