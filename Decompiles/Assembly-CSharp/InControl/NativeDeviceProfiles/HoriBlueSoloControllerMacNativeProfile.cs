using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A10 RID: 2576
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriBlueSoloControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005663 RID: 22115 RVA: 0x001A65A8 File Offset: 0x001A47A8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Blue Solo Controller ";
			base.DeviceNotes = "Hori Blue Solo Controller\ton Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 64001
				}
			};
		}
	}
}
