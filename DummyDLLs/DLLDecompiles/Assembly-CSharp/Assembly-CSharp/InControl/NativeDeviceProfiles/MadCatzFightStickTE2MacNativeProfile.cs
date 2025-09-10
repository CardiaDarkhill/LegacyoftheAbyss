using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A3D RID: 2621
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzFightStickTE2MacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056BD RID: 22205 RVA: 0x001A7F9C File Offset: 0x001A619C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Fight Stick TE2";
			base.DeviceNotes = "Mad Catz Fight Stick TE2 on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 61568
				}
			};
		}
	}
}
