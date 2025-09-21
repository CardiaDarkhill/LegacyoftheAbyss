using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A1E RID: 2590
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRAPNFightingStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600567F RID: 22143 RVA: 0x001A6EE0 File Offset: 0x001A50E0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori RAP N Fighting Stick";
			base.DeviceNotes = "Hori RAP N Fighting Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 174
				}
			};
		}
	}
}
