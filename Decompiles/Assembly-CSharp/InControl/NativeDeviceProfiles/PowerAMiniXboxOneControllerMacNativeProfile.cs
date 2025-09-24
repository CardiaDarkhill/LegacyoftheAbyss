using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A6C RID: 2668
	[Preserve]
	[NativeInputDeviceProfile]
	public class PowerAMiniXboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x0600571B RID: 22299 RVA: 0x001AA1D0 File Offset: 0x001A83D0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Power A Mini Xbox One Controller";
			base.DeviceNotes = "Power A Mini Xbox One Controller on Mac";
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
