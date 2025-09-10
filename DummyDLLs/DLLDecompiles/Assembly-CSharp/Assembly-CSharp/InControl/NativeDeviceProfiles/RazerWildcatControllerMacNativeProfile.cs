using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A76 RID: 2678
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerWildcatControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600572F RID: 22319 RVA: 0x001AA7A4 File Offset: 0x001A89A4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Wildcat Controller";
			base.DeviceNotes = "Razer Wildcat Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5426,
					ProductID = 2563
				}
			};
		}
	}
}
