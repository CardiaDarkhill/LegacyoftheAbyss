using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A0E RID: 2574
	[Preserve]
	[NativeInputDeviceProfile]
	public class HarmonixKeyboardMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600565F RID: 22111 RVA: 0x001A64B0 File Offset: 0x001A46B0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Harmonix Keyboard";
			base.DeviceNotes = "Harmonix Keyboard on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 4920
				}
			};
		}
	}
}
