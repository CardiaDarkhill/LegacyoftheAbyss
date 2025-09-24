using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A43 RID: 2627
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzMC2RacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056C9 RID: 22217 RVA: 0x001A8284 File Offset: 0x001A6484
		public override void Define()
		{
			base.Define();
			base.DeviceName = "MadCatz MC2 Racing Wheel";
			base.DeviceNotes = "MadCatz MC2 Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61472
				}
			};
		}
	}
}
