using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A88 RID: 2696
	[Preserve]
	[NativeInputDeviceProfile]
	public class TSZPelicanControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005753 RID: 22355 RVA: 0x001AB250 File Offset: 0x001A9450
		public override void Define()
		{
			base.Define();
			base.DeviceName = "TSZ Pelican Controller";
			base.DeviceNotes = "TSZ Pelican Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 513
				}
			};
		}
	}
}
