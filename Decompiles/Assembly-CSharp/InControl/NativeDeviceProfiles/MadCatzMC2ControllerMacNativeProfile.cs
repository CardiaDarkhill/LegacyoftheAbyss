using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A42 RID: 2626
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzMC2ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056C7 RID: 22215 RVA: 0x001A8208 File Offset: 0x001A6408
		public override void Define()
		{
			base.Define();
			base.DeviceName = "MadCatz MC2 Controller";
			base.DeviceNotes = "MadCatz MC2 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18208
				}
			};
		}
	}
}
