using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A4B RID: 2635
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzSF4FightStickRound2TEMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056D9 RID: 22233 RVA: 0x001A8664 File Offset: 0x001A6864
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz SF4 Fight Stick Round 2 TE";
			base.DeviceNotes = "Mad Catz SF4 Fight Stick Round 2 TE on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61496
				}
			};
		}
	}
}
