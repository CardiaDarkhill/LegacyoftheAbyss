using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A02 RID: 2562
	[Preserve]
	[NativeInputDeviceProfile]
	public class BETAOPControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005647 RID: 22087 RVA: 0x001A5DE0 File Offset: 0x001A3FE0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "BETAOP Controller";
			base.DeviceNotes = "BETAOP Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4544,
					ProductID = 21766
				}
			};
		}
	}
}
