using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A61 RID: 2657
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPMetallicsLEControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005705 RID: 22277 RVA: 0x001A9800 File Offset: 0x001A7A00
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Metallics LE Controller";
			base.DeviceNotes = "PDP Metallics LE Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 345
				}
			};
		}
	}
}
