using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A63 RID: 2659
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPTronControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005709 RID: 22281 RVA: 0x001A98F8 File Offset: 0x001A7AF8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Tron Controller";
			base.DeviceNotes = "PDP Tron Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63747
				}
			};
		}
	}
}
