using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A7A RID: 2682
	[Preserve]
	[NativeInputDeviceProfile]
	public class RockBandDrumsMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005737 RID: 22327 RVA: 0x001AA9D4 File Offset: 0x001A8BD4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Rock Band Drums";
			base.DeviceNotes = "Rock Band Drums on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 3
				}
			};
		}
	}
}
