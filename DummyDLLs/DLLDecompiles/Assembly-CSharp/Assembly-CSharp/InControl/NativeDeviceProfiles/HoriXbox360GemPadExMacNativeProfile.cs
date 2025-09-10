using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A28 RID: 2600
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriXbox360GemPadExMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005693 RID: 22163 RVA: 0x001A7434 File Offset: 0x001A5634
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Xbox 360 Gem Pad Ex";
			base.DeviceNotes = "Hori Xbox 360 Gem Pad Ex on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21773
				}
			};
		}
	}
}
