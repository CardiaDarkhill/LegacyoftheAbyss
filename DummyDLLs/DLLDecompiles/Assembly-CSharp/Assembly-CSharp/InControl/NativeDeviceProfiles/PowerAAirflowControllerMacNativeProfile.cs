using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A68 RID: 2664
	[Preserve]
	[NativeInputDeviceProfile]
	public class PowerAAirflowControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005713 RID: 22291 RVA: 0x001A9F60 File Offset: 0x001A8160
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PowerA Airflow Controller";
			base.DeviceNotes = "PowerA Airflow Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5604,
					ProductID = 16138
				}
			};
		}
	}
}
