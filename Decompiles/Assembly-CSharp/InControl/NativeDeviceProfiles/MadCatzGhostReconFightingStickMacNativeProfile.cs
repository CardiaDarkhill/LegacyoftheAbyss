using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A40 RID: 2624
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzGhostReconFightingStickMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056C3 RID: 22211 RVA: 0x001A8110 File Offset: 0x001A6310
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Ghost Recon Fighting Stick";
			base.DeviceNotes = "Mad Catz Ghost Recon Fighting Stick on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61473
				}
			};
		}
	}
}
