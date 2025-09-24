using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A7D RID: 2685
	[Preserve]
	[NativeInputDeviceProfile]
	public class RockCandyPS3ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600573D RID: 22333 RVA: 0x001AAB40 File Offset: 0x001A8D40
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Rock Candy PS3 Controller";
			base.DeviceNotes = "Rock Candy PS3 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 286
				}
			};
		}
	}
}
