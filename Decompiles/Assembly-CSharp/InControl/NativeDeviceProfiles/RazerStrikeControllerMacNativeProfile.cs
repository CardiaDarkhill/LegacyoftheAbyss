using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A75 RID: 2677
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerStrikeControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600572D RID: 22317 RVA: 0x001AA72C File Offset: 0x001A892C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Strike Controller";
			base.DeviceNotes = "Razer Strike Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5769,
					ProductID = 1
				}
			};
		}
	}
}
