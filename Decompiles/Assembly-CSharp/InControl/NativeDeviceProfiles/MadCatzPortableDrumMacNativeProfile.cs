using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A48 RID: 2632
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzPortableDrumMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056D3 RID: 22227 RVA: 0x001A84F0 File Offset: 0x001A66F0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Portable Drum";
			base.DeviceNotes = "Mad Catz Portable Drum on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 39025
				}
			};
		}
	}
}
