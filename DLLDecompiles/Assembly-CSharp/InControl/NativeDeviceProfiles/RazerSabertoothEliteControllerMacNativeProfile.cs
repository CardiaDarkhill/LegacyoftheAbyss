using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A74 RID: 2676
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerSabertoothEliteControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x0600572B RID: 22315 RVA: 0x001AA670 File Offset: 0x001A8870
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Sabertooth Elite Controller";
			base.DeviceNotes = "Razer Sabertooth Elite Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5769,
					ProductID = 65024
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 23812
				}
			};
		}
	}
}
