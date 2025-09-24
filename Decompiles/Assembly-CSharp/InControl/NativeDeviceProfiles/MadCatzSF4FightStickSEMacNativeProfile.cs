using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A4C RID: 2636
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSF4FightStickSEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056DB RID: 22235 RVA: 0x001A86E0 File Offset: 0x001A68E0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz SF4 Fight Stick SE";
			base.DeviceNotes = "Mad Catz SF4 Fight Stick SE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18200
				}
			};
		}
	}
}
