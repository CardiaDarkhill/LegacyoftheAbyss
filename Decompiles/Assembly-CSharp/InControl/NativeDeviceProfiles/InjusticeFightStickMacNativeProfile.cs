using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A2A RID: 2602
	[Preserve]
	[NativeInputDeviceProfile]
	public class InjusticeFightStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005697 RID: 22167 RVA: 0x001A752C File Offset: 0x001A572C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Injustice Fight Stick";
			base.DeviceNotes = "Injustice Fight Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 293
				}
			};
		}
	}
}
