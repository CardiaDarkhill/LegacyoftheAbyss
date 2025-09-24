using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A60 RID: 2656
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPMarvelControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005703 RID: 22275 RVA: 0x001A9784 File Offset: 0x001A7984
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Marvel Controller";
			base.DeviceNotes = "PDP Marvel Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 327
				}
			};
		}
	}
}
