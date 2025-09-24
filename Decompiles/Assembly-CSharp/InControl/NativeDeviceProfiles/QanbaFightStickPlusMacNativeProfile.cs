using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A70 RID: 2672
	[Preserve]
	[NativeInputDeviceProfile]
	public class QanbaFightStickPlusMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x06005723 RID: 22307 RVA: 0x001AA3C0 File Offset: 0x001A85C0
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Qanba Fight Stick Plus";
			base.DeviceNotes = "Qanba Fight Stick Plus on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1848,
					ProductID = 48879
				}
			};
		}
	}
}
