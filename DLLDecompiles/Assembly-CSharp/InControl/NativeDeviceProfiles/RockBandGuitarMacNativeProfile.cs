using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A7B RID: 2683
	[Preserve]
	[NativeInputDeviceProfile]
	public class RockBandGuitarMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005739 RID: 22329 RVA: 0x001AAA4C File Offset: 0x001A8C4C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Rock Band Guitar";
			base.DeviceNotes = "Rock Band Guitar on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 2
				}
			};
		}
	}
}
