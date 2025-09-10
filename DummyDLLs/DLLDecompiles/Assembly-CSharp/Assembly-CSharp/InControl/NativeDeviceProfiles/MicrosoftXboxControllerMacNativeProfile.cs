using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000A56 RID: 2646
	[Preserve]
	[NativeInputDeviceProfile]
	public class MicrosoftXboxControllerMacNativeProfile : Xbox360DriverMacNativeProfile
	{
		// Token: 0x060056EF RID: 22255 RVA: 0x001A8D34 File Offset: 0x001A6F34
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Microsoft Xbox Controller";
			base.DeviceNotes = "Microsoft Xbox Controller on Mac";
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = ushort.MaxValue,
					ProductID = ushort.MaxValue
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 649
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 648
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 645
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 514
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 647
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.HID,
					VendorID = 1118,
					ProductID = 648
				}
			};
		}
	}
}
