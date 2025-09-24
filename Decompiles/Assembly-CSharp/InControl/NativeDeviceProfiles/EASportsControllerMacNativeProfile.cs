using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A07 RID: 2567
	[Preserve]
	[NativeInputDeviceProfile]
	public class EASportsControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005651 RID: 22097 RVA: 0x001A604C File Offset: 0x001A424C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "EA Sports Controller";
			base.DeviceNotes = "EA Sports Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 305
				}
			};
		}
	}
}
