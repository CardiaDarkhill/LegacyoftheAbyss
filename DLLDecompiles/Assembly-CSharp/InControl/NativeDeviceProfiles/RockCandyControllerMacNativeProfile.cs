using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A7C RID: 2684
	[Preserve]
	[NativeInputDeviceProfile]
	public class RockCandyControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600573B RID: 22331 RVA: 0x001AAAC4 File Offset: 0x001A8CC4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Rock Candy Controller";
			base.DeviceNotes = "Rock Candy Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 287
				}
			};
		}
	}
}
