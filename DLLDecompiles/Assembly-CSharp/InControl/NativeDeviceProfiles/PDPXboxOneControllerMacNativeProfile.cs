using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A67 RID: 2663
	[Preserve]
	[NativeInputDeviceProfile]
	public class PDPXboxOneControllerMacNativeProfile : XboxOneDriverMacNativeProfile
	{
		// Token: 0x06005711 RID: 22289 RVA: 0x001A9AE8 File Offset: 0x001A7CE8
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PDP Xbox One Controller";
			base.DeviceNotes = "PDP Xbox One Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 676
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 715
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 314
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 354
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 9414,
					ProductID = 22042
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 353
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 355
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 683
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 352
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 680
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 674
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 347
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 677
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 685
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 704
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 679
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 3695,
					ProductID = 678
				}
			};
		}
	}
}
