using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A08 RID: 2568
	[Preserve]
	[NativeInputDeviceProfile]
	public class ElecomControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005653 RID: 22099 RVA: 0x001A60C8 File Offset: 0x001A42C8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Elecom Controller";
			base.DeviceNotes = "Elecom Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1390,
					ProductID = 8196
				}
			};
		}
	}
}
