using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A57 RID: 2647
	[Preserve]
	[NativeInputDeviceProfile]
	public class MicrosoftXboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x060056F1 RID: 22257 RVA: 0x001A8F2C File Offset: 0x001A712C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Microsoft Xbox One Controller";
			base.DeviceNotes = "Microsoft Xbox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 721
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 733
				}
			};
		}
	}
}
