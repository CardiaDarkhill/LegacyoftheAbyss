using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A0F RID: 2575
	[Preserve]
	[NativeInputDeviceProfile]
	public class HoneyBeeControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005661 RID: 22113 RVA: 0x001A652C File Offset: 0x001A472C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Honey Bee Controller";
			base.DeviceNotes = "Honey Bee Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4779,
					ProductID = 21760
				}
			};
		}
	}
}
