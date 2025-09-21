using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A36 RID: 2614
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzArcadeStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056AF RID: 22191 RVA: 0x001A7B38 File Offset: 0x001A5D38
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Arcade Stick";
			base.DeviceNotes = "Mad Catz Arcade Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 18264
				}
			};
		}
	}
}
