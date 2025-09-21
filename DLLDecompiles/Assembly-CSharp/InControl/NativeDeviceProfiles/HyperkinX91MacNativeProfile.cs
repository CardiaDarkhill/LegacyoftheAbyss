using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A29 RID: 2601
	[Preserve]
	[NativeInputDeviceProfile]
	public class HyperkinX91MacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005695 RID: 22165 RVA: 0x001A74B0 File Offset: 0x001A56B0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Hyperkin X91";
			base.DeviceNotes = "Hyperkin X91 on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 11812,
					ProductID = 5768
				}
			};
		}
	}
}
