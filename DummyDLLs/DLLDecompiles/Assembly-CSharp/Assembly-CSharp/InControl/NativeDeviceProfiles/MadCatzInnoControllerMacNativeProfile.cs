using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A41 RID: 2625
	[Preserve]
	[NativeInputDeviceProfile]
	public class MadCatzInnoControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056C5 RID: 22213 RVA: 0x001A818C File Offset: 0x001A638C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Mad Catz Inno Controller";
			base.DeviceNotes = "Mad Catz Inno Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 62465
				}
			};
		}
	}
}
