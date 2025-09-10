using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A69 RID: 2665
	[Preserve]
	[NativeInputDeviceProfile]
	public class POWERAFUS1ONTournamentControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005715 RID: 22293 RVA: 0x001A9FDC File Offset: 0x001A81DC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "POWER A FUS1ON Tournament Controller";
			base.DeviceNotes = "POWER A FUS1ON Tournament Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 21399
				}
			};
		}
	}
}
