using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A5F RID: 2655
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPBattlefieldXBoxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x06005701 RID: 22273 RVA: 0x001A9708 File Offset: 0x001A7908
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Battlefield XBox One Controller";
			base.DeviceNotes = "PDP Battlefield XBox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 356
				}
			};
		}
	}
}
