using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A58 RID: 2648
	[Preserve]
	[NativeInputDeviceProfile]
	public class MicrosoftXboxOneEliteControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x060056F3 RID: 22259 RVA: 0x001A8FE8 File Offset: 0x001A71E8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Microsoft Xbox One Elite Controller";
			base.DeviceNotes = "Microsoft Xbox One Elite Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 739
				}
			};
		}
	}
}
