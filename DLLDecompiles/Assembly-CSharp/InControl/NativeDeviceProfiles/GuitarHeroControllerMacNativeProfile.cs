using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A0B RID: 2571
	[Preserve]
	[NativeInputDeviceProfile]
	public class GuitarHeroControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005659 RID: 22105 RVA: 0x001A633C File Offset: 0x001A453C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Guitar Hero Controller";
			base.DeviceNotes = "Guitar Hero Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5168,
					ProductID = 18248
				}
			};
		}
	}
}
