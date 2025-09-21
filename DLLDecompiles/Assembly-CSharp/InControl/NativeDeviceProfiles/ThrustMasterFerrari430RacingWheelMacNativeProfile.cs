using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A81 RID: 2689
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustMasterFerrari430RacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005745 RID: 22341 RVA: 0x001AAE70 File Offset: 0x001A9070
		public override void Define()
		{
			base.Define();
			base.DeviceName = "ThrustMaster Ferrari 430 Racing Wheel";
			base.DeviceNotes = "ThrustMaster Ferrari 430 Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1103,
					ProductID = 46683
				}
			};
		}
	}
}
