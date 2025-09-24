using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A83 RID: 2691
	[Preserve]
	[NativeInputDeviceProfile]
	public class ThrustMasterFerrari458SpiderRacingWheelMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005749 RID: 22345 RVA: 0x001AAFA8 File Offset: 0x001A91A8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "ThrustMaster Ferrari 458 Spider Racing Wheel";
			base.DeviceNotes = "ThrustMaster Ferrari 458 Spider Racing Wheel on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1103,
					ProductID = 46705
				}
			};
		}
	}
}
