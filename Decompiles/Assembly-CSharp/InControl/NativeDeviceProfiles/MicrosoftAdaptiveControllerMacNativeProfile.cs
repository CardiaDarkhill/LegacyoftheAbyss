using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A54 RID: 2644
	[Preserve]
	[NativeInputDeviceProfile]
	public class MicrosoftAdaptiveControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056EB RID: 22251 RVA: 0x001A8AC0 File Offset: 0x001A6CC0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Microsoft Adaptive Controller";
			base.DeviceNotes = "Microsoft Adaptive Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 2826
				}
			};
		}
	}
}
