using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A6A RID: 2666
	[Preserve]
	[NativeInputDeviceProfile]
	public class PowerAMiniControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005717 RID: 22295 RVA: 0x001AA058 File Offset: 0x001A8258
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PowerA Mini Controller";
			base.DeviceNotes = "PowerA Mini Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21530
				}
			};
		}
	}
}
