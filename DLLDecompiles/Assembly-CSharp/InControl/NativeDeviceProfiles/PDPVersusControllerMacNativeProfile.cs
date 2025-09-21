using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A64 RID: 2660
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPVersusControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600570B RID: 22283 RVA: 0x001A9974 File Offset: 0x001A7B74
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Versus Controller";
			base.DeviceNotes = "PDP Versus Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63748
				}
			};
		}
	}
}
