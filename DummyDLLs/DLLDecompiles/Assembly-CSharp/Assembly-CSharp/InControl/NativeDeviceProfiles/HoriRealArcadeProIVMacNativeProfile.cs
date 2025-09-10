using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A23 RID: 2595
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoriRealArcadeProIVMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005689 RID: 22153 RVA: 0x001A714C File Offset: 0x001A534C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hori Real Arcade Pro IV";
			base.DeviceNotes = "Hori Real Arcade Pro IV on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3853,
					ProductID = 140
				}
			};
		}
	}
}
