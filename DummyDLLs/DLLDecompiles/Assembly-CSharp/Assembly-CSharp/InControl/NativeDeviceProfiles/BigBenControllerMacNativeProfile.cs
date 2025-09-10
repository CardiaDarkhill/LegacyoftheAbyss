using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A03 RID: 2563
	[Preserve]
	[NativeInputDeviceProfile]
	public class BigBenControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005649 RID: 22089 RVA: 0x001A5E5C File Offset: 0x001A405C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Big Ben Controller";
			base.DeviceNotes = "Big Ben Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5227,
					ProductID = 1537
				}
			};
		}
	}
}
