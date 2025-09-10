using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000AB5 RID: 2741
	[Preserve]
	[NativeInputDeviceProfile]
	public class SDLXboxOneNativeProfile : SDLControllerNativeProfile
	{
		// Token: 0x060057D2 RID: 22482 RVA: 0x001B3D8C File Offset: 0x001B1F8C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Xbox One Controller";
			base.DeviceStyle = InputDeviceStyle.XboxOne;
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1118,
					ProductID = 746
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1118,
					ProductID = 736
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1118,
					ProductID = 765
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1118,
					ProductID = 767
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1118,
					ProductID = 766
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				SDLControllerNativeProfile.Action1Mapping("A"),
				SDLControllerNativeProfile.Action2Mapping("B"),
				SDLControllerNativeProfile.Action3Mapping("X"),
				SDLControllerNativeProfile.Action4Mapping("Y"),
				SDLControllerNativeProfile.LeftCommandMapping("View", InputControlType.View),
				SDLControllerNativeProfile.RightCommandMapping("Menu", InputControlType.Menu),
				SDLControllerNativeProfile.SystemMapping("Guide", InputControlType.Guide),
				SDLControllerNativeProfile.LeftStickButtonMapping(),
				SDLControllerNativeProfile.RightStickButtonMapping(),
				SDLControllerNativeProfile.LeftBumperMapping("Left Bumper"),
				SDLControllerNativeProfile.RightBumperMapping("Right Bumper"),
				SDLControllerNativeProfile.DPadUpMapping(),
				SDLControllerNativeProfile.DPadDownMapping(),
				SDLControllerNativeProfile.DPadLeftMapping(),
				SDLControllerNativeProfile.DPadRightMapping()
			};
			base.AnalogMappings = new InputControlMapping[]
			{
				SDLControllerNativeProfile.LeftStickLeftMapping(),
				SDLControllerNativeProfile.LeftStickRightMapping(),
				SDLControllerNativeProfile.LeftStickUpMapping(),
				SDLControllerNativeProfile.LeftStickDownMapping(),
				SDLControllerNativeProfile.RightStickLeftMapping(),
				SDLControllerNativeProfile.RightStickRightMapping(),
				SDLControllerNativeProfile.RightStickUpMapping(),
				SDLControllerNativeProfile.RightStickDownMapping(),
				SDLControllerNativeProfile.LeftTriggerMapping("Left Trigger"),
				SDLControllerNativeProfile.RightTriggerMapping("Right Trigger")
			};
		}

		// Token: 0x02001B6F RID: 7023
		private enum ProductId : ushort
		{
			// Token: 0x04009CF5 RID: 40181
			XBOX_ONE_S = 746,
			// Token: 0x04009CF6 RID: 40182
			XBOX_ONE_S_REV1_BLUETOOTH = 736,
			// Token: 0x04009CF7 RID: 40183
			XBOX_ONE_S_REV2_BLUETOOTH = 765,
			// Token: 0x04009CF8 RID: 40184
			XBOX_ONE_RAW_INPUT_CONTROLLER = 767,
			// Token: 0x04009CF9 RID: 40185
			XBOX_ONE_XINPUT_CONTROLLER = 766
		}
	}
}
