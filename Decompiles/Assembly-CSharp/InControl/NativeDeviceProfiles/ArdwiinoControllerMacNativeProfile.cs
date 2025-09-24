using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x020009FF RID: 2559
	[Preserve]
	[NativeInputDeviceProfile]
	public class ArdwiinoControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005641 RID: 22081 RVA: 0x001A5BEC File Offset: 0x001A3DEC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Ardwiino Controller";
			base.DeviceNotes = "Ardwiino Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4617,
					ProductID = 10370
				}
			};
		}
	}
}
