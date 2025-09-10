using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A89 RID: 2697
	[Preserve]
	[NativeInputDeviceProfile]
	public class Xbox360MortalKombatFightStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005755 RID: 22357 RVA: 0x001AB2CC File Offset: 0x001A94CC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Xbox 360 Mortal Kombat Fight Stick";
			base.DeviceNotes = "Xbox 360 Mortal Kombat Fight Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63750
				}
			};
		}
	}
}
