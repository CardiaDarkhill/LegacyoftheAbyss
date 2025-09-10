using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000ADD RID: 2781
	[Preserve]
	[NativeInputDeviceProfile]
	public class XTR_G2_WindowsNativeProfile : InputDeviceProfile
	{
		// Token: 0x06005822 RID: 22562 RVA: 0x001BFAE0 File Offset: 0x001BDCE0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "KMODEL Simulator XTR G2 FMS Controller";
			base.DeviceNotes = "KMODEL Simulator XTR G2 FMS Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"Windows"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.DirectInput,
					VendorID = 2971,
					ProductID = 16402,
					NameLiteral = "KMODEL Simulator - XTR+G2+FMS Controller"
				}
			};
		}
	}
}
