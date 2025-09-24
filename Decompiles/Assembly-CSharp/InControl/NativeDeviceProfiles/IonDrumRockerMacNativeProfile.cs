using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A2B RID: 2603
	[Preserve]
	[NativeInputDeviceProfile]
	public class IonDrumRockerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005699 RID: 22169 RVA: 0x001A75A8 File Offset: 0x001A57A8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Ion Drum Rocker";
			base.DeviceNotes = "Ion Drum Rocker on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 304
				}
			};
		}
	}
}
