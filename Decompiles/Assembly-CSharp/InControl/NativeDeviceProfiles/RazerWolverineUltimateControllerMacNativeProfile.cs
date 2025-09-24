using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A77 RID: 2679
	[Preserve]
	[NativeInputDeviceProfile]
	public class RazerWolverineUltimateControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005731 RID: 22321 RVA: 0x001AA820 File Offset: 0x001A8A20
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Razer Wolverine Ultimate Controller";
			base.DeviceNotes = "Razer Wolverine Ultimate Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 5426,
					ProductID = 2580
				}
			};
		}
	}
}
