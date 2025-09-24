using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A6F RID: 2671
	[Preserve]
	[NativeInputDeviceProfile]
	public class ProEXXboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x06005721 RID: 22305 RVA: 0x001AA344 File Offset: 0x001A8544
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Pro EX Xbox One Controller";
			base.DeviceNotes = "Pro EX Xbox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21562
				}
			};
		}
	}
}
