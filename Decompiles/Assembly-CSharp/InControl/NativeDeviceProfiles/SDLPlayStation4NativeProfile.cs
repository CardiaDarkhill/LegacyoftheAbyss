using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000AB2 RID: 2738
	[Preserve]
	[NativeInputDeviceProfile]
	public class SDLPlayStation4NativeProfile : SDLControllerNativeProfile
	{
		// Token: 0x060057CC RID: 22476 RVA: 0x001B375C File Offset: 0x001B195C
		public override void Define()
		{
			base.Define();
			base.DeviceName = "PlayStation 4 Controller";
			base.DeviceStyle = InputDeviceStyle.PlayStation4;
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1356,
					ProductID = 1476
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1356,
					ProductID = 2976
				},
				new InputDeviceMatcher
				{
					DriverType = InputDeviceDriverType.SDLController,
					VendorID = 1356,
					ProductID = 2508
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				SDLControllerNativeProfile.Action1Mapping("Cross"),
				SDLControllerNativeProfile.Action2Mapping("Circle"),
				SDLControllerNativeProfile.Action3Mapping("Square"),
				SDLControllerNativeProfile.Action4Mapping("Triangle"),
				SDLControllerNativeProfile.LeftCommandMapping("Share", InputControlType.Share),
				SDLControllerNativeProfile.RightCommandMapping("Options", InputControlType.Options),
				SDLControllerNativeProfile.SystemMapping("System", InputControlType.System),
				SDLControllerNativeProfile.LeftStickButtonMapping(),
				SDLControllerNativeProfile.RightStickButtonMapping(),
				SDLControllerNativeProfile.LeftBumperMapping("L1"),
				SDLControllerNativeProfile.RightBumperMapping("R1"),
				SDLControllerNativeProfile.DPadUpMapping(),
				SDLControllerNativeProfile.DPadDownMapping(),
				SDLControllerNativeProfile.DPadLeftMapping(),
				SDLControllerNativeProfile.DPadRightMapping(),
				SDLControllerNativeProfile.TouchPadButtonMapping()
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
				SDLControllerNativeProfile.LeftTriggerMapping("L2"),
				SDLControllerNativeProfile.RightTriggerMapping("R2"),
				SDLControllerNativeProfile.AccelerometerXMapping(),
				SDLControllerNativeProfile.AccelerometerYMapping(),
				SDLControllerNativeProfile.AccelerometerZMapping(),
				SDLControllerNativeProfile.GyroscopeXMapping(),
				SDLControllerNativeProfile.GyroscopeYMapping(),
				SDLControllerNativeProfile.GyroscopeZMapping()
			};
		}

		// Token: 0x02001B6D RID: 7021
		private enum ProductId : ushort
		{
			// Token: 0x04009CEF RID: 40175
			SONY_DS4 = 1476,
			// Token: 0x04009CF0 RID: 40176
			SONY_DS4_DONGLE = 2976,
			// Token: 0x04009CF1 RID: 40177
			SONY_DS4_SLIM = 2508
		}
	}
}
