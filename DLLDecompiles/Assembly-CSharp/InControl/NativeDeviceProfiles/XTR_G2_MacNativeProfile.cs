using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000AA3 RID: 2723
	[Preserve]
	[NativeInputDeviceProfile]
	public class XTR_G2_MacNativeProfile : InputDeviceProfile
	{
		// Token: 0x06005789 RID: 22409 RVA: 0x001B0BA8 File Offset: 0x001AEDA8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "KMODEL Simulator XTR G2 FMS Controller";
			base.DeviceNotes = "KMODEL Simulator XTR G2 FMS Controller on OS X";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"OS X"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 2971,
					ProductID = 16402,
					NameLiteral = "KMODEL Simulator - XTR+G2+FMS Controller"
				}
			};
		}
	}
}
