using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A55 RID: 2645
	[Preserve]
	[NativeInputDeviceProfile]
	public class MicrosoftXbox360ControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056ED RID: 22253 RVA: 0x001A8B3C File Offset: 0x001A6D3C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Microsoft Xbox 360 Controller";
			base.DeviceNotes = "Microsoft Xbox 360 Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 654
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 655
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 307
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 63233
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 672
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 62721
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 672
				}
			};
		}
	}
}
