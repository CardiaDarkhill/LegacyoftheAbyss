using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A5D RID: 2653
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPAfterglowControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056FD RID: 22269 RVA: 0x001A9294 File Offset: 0x001A7494
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Afterglow Controller";
			base.DeviceNotes = "PDP Afterglow Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 64252
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63751
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 64253
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 742
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 768
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 22554
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 1043
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 7085,
					ProductID = 63744
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 63744
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 275
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 531
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 4779,
					ProductID = 769
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 4371
				}
			};
		}
	}
}
