using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A04 RID: 2564
	[Preserve]
	[NativeInputDeviceProfile]
	public class BrookNeoGeoConverterMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600564B RID: 22091 RVA: 0x001A5ED8 File Offset: 0x001A40D8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Brook NeoGeo Converter";
			base.DeviceNotes = "Brook NeoGeo Converter on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3090,
					ProductID = 2036
				}
			};
		}
	}
}
