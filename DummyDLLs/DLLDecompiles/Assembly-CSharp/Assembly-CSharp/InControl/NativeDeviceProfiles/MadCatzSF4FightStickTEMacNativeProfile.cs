using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A4D RID: 2637
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSF4FightStickTEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056DD RID: 22237 RVA: 0x001A875C File Offset: 0x001A695C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz SF4 Fight Stick TE";
			base.DeviceNotes = "Mad Catz SF4 Fight Stick TE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18232
				}
			};
		}
	}
}
