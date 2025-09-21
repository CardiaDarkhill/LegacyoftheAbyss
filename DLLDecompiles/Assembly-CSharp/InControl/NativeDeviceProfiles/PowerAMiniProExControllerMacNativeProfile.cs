using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A6B RID: 2667
	[Preserve]
	[NativeInputDeviceProfile]
	public class PowerAMiniProExControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005719 RID: 22297 RVA: 0x001AA0D4 File Offset: 0x001A82D4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PowerA Mini Pro Ex Controller";
			base.DeviceNotes = "PowerA Mini Pro Ex Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5604,
					ProductID = 16128
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21274
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21248
				}
			};
		}
	}
}
