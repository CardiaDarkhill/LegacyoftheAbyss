using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A62 RID: 2658
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPTitanfall2XboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x06005707 RID: 22279 RVA: 0x001A987C File Offset: 0x001A7A7C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Titanfall 2 Xbox One Controller";
			base.DeviceNotes = "PDP Titanfall 2 Xbox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 357
				}
			};
		}
	}
}
