using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A53 RID: 2643
	[Preserve]
	[NativeInputDeviceProfile]
	public class McbazelAdapterMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056E9 RID: 22249 RVA: 0x001A8A44 File Offset: 0x001A6C44
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mcbazel Adapter";
			base.DeviceNotes = "Mcbazel Adapter on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 654
				}
			};
		}
	}
}
