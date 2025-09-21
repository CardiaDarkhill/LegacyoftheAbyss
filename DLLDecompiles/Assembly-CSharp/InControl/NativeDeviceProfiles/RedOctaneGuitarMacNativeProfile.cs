using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A79 RID: 2681
	[Preserve]
	[NativeInputDeviceProfile]
	public class RedOctaneGuitarMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005735 RID: 22325 RVA: 0x001AA958 File Offset: 0x001A8B58
		public override void Define()
		{
			base.Define();
			base.DeviceName = "RedOctane Guitar";
			base.DeviceNotes = "RedOctane Guitar on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5168,
					ProductID = 1803
				}
			};
		}
	}
}
