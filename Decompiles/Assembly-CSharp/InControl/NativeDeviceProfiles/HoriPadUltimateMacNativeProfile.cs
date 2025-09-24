using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A1D RID: 2589
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriPadUltimateMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600567D RID: 22141 RVA: 0x001A6E64 File Offset: 0x001A5064
		public override void Define()
		{
			base.Define();
			base.DeviceName = "HoriPad Ultimate";
			base.DeviceNotes = "HoriPad Ultimate on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 144
				}
			};
		}
	}
}
