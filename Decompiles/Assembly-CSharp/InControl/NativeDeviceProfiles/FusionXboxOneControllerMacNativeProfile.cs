using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A09 RID: 2569
	[Preserve]
	[NativeInputDeviceProfile]
	public class FusionXboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x06005655 RID: 22101 RVA: 0x001A6144 File Offset: 0x001A4344
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Fusion Xbox One Controller";
			base.DeviceNotes = "Fusion Xbox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21786
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 22042
				}
			};
		}
	}
}
