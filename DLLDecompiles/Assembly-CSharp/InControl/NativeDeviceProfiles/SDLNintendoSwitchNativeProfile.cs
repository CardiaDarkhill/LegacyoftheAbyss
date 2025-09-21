using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000AB0 RID: 2736
	[Preserve]
	[NativeInputDeviceProfile]
	public class SDLNintendoSwitchNativeProfile : SDLControllerNativeProfile
	{
		// Token: 0x060057C8 RID: 22472 RVA: 0x001B334C File Offset: 0x001B154C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Nintendo Switch Pro Controller";
			base.DeviceStyle = InputDeviceStyle.NintendoSwitch;
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1406
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				SDLControllerNativeProfile.Action1Mapping("B"),
				SDLControllerNativeProfile.Action2Mapping("A"),
				SDLControllerNativeProfile.Action3Mapping("Y"),
				SDLControllerNativeProfile.Action4Mapping("X"),
				SDLControllerNativeProfile.LeftCommandMapping("Minus", InputControlType.Minus),
				SDLControllerNativeProfile.RightCommandMapping("Plus", InputControlType.Plus),
				SDLControllerNativeProfile.SystemMapping("Home", InputControlType.Home),
				SDLControllerNativeProfile.LeftStickButtonMapping(),
				SDLControllerNativeProfile.RightStickButtonMapping(),
				SDLControllerNativeProfile.LeftBumperMapping("L"),
				SDLControllerNativeProfile.RightBumperMapping("R"),
				SDLControllerNativeProfile.DPadUpMapping(),
				SDLControllerNativeProfile.DPadDownMapping(),
				SDLControllerNativeProfile.DPadLeftMapping(),
				SDLControllerNativeProfile.DPadRightMapping(),
				SDLControllerNativeProfile.Misc1Mapping("Capture", InputControlType.Capture),
				SDLControllerNativeProfile.Paddle1Mapping(),
				SDLControllerNativeProfile.Paddle2Mapping(),
				SDLControllerNativeProfile.Paddle3Mapping(),
				SDLControllerNativeProfile.Paddle4Mapping()
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
				SDLControllerNativeProfile.LeftTriggerMapping("ZL"),
				SDLControllerNativeProfile.RightTriggerMapping("ZR"),
				SDLControllerNativeProfile.AccelerometerXMapping(),
				SDLControllerNativeProfile.AccelerometerYMapping(),
				SDLControllerNativeProfile.AccelerometerZMapping(),
				SDLControllerNativeProfile.GyroscopeXMapping(),
				SDLControllerNativeProfile.GyroscopeYMapping(),
				SDLControllerNativeProfile.GyroscopeZMapping()
			};
		}
	}
}
