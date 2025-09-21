using System;

namespace InControl.NativeDeviceProfiles
{
	// Token: 0x02000AAB RID: 2731
	[Preserve]
	[NativeInputDeviceProfile]
	public class SDLControllerNativeProfile : InputDeviceProfile
	{
		// Token: 0x06005799 RID: 22425 RVA: 0x001B25E4 File Offset: 0x001B07E4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "{NAME}";
			base.DeviceNotes = "";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"OS X",
				"Windows"
			};
		}

		// Token: 0x0600579A RID: 22426 RVA: 0x001B2630 File Offset: 0x001B0830
		protected static InputControlMapping Action1Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action1,
				Source = InputDeviceProfile.Button(0)
			};
		}

		// Token: 0x0600579B RID: 22427 RVA: 0x001B2652 File Offset: 0x001B0852
		protected static InputControlMapping Action2Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action2,
				Source = InputDeviceProfile.Button(1)
			};
		}

		// Token: 0x0600579C RID: 22428 RVA: 0x001B2674 File Offset: 0x001B0874
		protected static InputControlMapping Action3Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action3,
				Source = InputDeviceProfile.Button(2)
			};
		}

		// Token: 0x0600579D RID: 22429 RVA: 0x001B2696 File Offset: 0x001B0896
		protected static InputControlMapping Action4Mapping(string name)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.Action4,
				Source = InputDeviceProfile.Button(3)
			};
		}

		// Token: 0x0600579E RID: 22430 RVA: 0x001B26B8 File Offset: 0x001B08B8
		protected static InputControlMapping LeftCommandMapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(4)
			};
		}

		// Token: 0x0600579F RID: 22431 RVA: 0x001B26D9 File Offset: 0x001B08D9
		protected static InputControlMapping SystemMapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(5)
			};
		}

		// Token: 0x060057A0 RID: 22432 RVA: 0x001B26FA File Offset: 0x001B08FA
		protected static InputControlMapping RightCommandMapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(6)
			};
		}

		// Token: 0x060057A1 RID: 22433 RVA: 0x001B271B File Offset: 0x001B091B
		protected static InputControlMapping LeftStickButtonMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Button",
				Target = InputControlType.LeftStickButton,
				Source = InputDeviceProfile.Button(7)
			};
		}

		// Token: 0x060057A2 RID: 22434 RVA: 0x001B2740 File Offset: 0x001B0940
		protected static InputControlMapping RightStickButtonMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Button",
				Target = InputControlType.RightStickButton,
				Source = InputDeviceProfile.Button(8)
			};
		}

		// Token: 0x060057A3 RID: 22435 RVA: 0x001B2766 File Offset: 0x001B0966
		protected static InputControlMapping LeftBumperMapping(string name = "Left Bumper")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.LeftBumper,
				Source = InputDeviceProfile.Button(9)
			};
		}

		// Token: 0x060057A4 RID: 22436 RVA: 0x001B2789 File Offset: 0x001B0989
		protected static InputControlMapping RightBumperMapping(string name = "Right Bumper")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.RightBumper,
				Source = InputDeviceProfile.Button(10)
			};
		}

		// Token: 0x060057A5 RID: 22437 RVA: 0x001B27AC File Offset: 0x001B09AC
		protected static InputControlMapping DPadUpMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Up",
				Target = InputControlType.DPadUp,
				Source = InputDeviceProfile.Button(11)
			};
		}

		// Token: 0x060057A6 RID: 22438 RVA: 0x001B27D3 File Offset: 0x001B09D3
		protected static InputControlMapping DPadDownMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Down",
				Target = InputControlType.DPadDown,
				Source = InputDeviceProfile.Button(12)
			};
		}

		// Token: 0x060057A7 RID: 22439 RVA: 0x001B27FA File Offset: 0x001B09FA
		protected static InputControlMapping DPadLeftMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Left",
				Target = InputControlType.DPadLeft,
				Source = InputDeviceProfile.Button(13)
			};
		}

		// Token: 0x060057A8 RID: 22440 RVA: 0x001B2821 File Offset: 0x001B0A21
		protected static InputControlMapping DPadRightMapping()
		{
			return new InputControlMapping
			{
				Name = "DPad Right",
				Target = InputControlType.DPadRight,
				Source = InputDeviceProfile.Button(14)
			};
		}

		// Token: 0x060057A9 RID: 22441 RVA: 0x001B2848 File Offset: 0x001B0A48
		protected static InputControlMapping Misc1Mapping(string name, InputControlType target)
		{
			return new InputControlMapping
			{
				Name = name,
				Target = target,
				Source = InputDeviceProfile.Button(15)
			};
		}

		// Token: 0x060057AA RID: 22442 RVA: 0x001B286A File Offset: 0x001B0A6A
		protected static InputControlMapping Paddle1Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 1",
				Target = InputControlType.Paddle1,
				Source = InputDeviceProfile.Button(16)
			};
		}

		// Token: 0x060057AB RID: 22443 RVA: 0x001B2894 File Offset: 0x001B0A94
		protected static InputControlMapping Paddle2Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 2",
				Target = InputControlType.Paddle2,
				Source = InputDeviceProfile.Button(17)
			};
		}

		// Token: 0x060057AC RID: 22444 RVA: 0x001B28BE File Offset: 0x001B0ABE
		protected static InputControlMapping Paddle3Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 3",
				Target = InputControlType.Paddle3,
				Source = InputDeviceProfile.Button(18)
			};
		}

		// Token: 0x060057AD RID: 22445 RVA: 0x001B28E8 File Offset: 0x001B0AE8
		protected static InputControlMapping Paddle4Mapping()
		{
			return new InputControlMapping
			{
				Name = "Paddle 4",
				Target = InputControlType.Paddle4,
				Source = InputDeviceProfile.Button(19)
			};
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x001B2912 File Offset: 0x001B0B12
		protected static InputControlMapping TouchPadButtonMapping()
		{
			return new InputControlMapping
			{
				Name = "Touch Pad Button",
				Target = InputControlType.TouchPadButton,
				Source = InputDeviceProfile.Button(20)
			};
		}

		// Token: 0x060057AF RID: 22447 RVA: 0x001B293C File Offset: 0x001B0B3C
		protected static InputControlMapping LeftStickLeftMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Left",
				Target = InputControlType.LeftStickLeft,
				Source = InputDeviceProfile.Analog(0),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B0 RID: 22448 RVA: 0x001B296F File Offset: 0x001B0B6F
		protected static InputControlMapping LeftStickRightMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Right",
				Target = InputControlType.LeftStickRight,
				Source = InputDeviceProfile.Analog(0),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B1 RID: 22449 RVA: 0x001B29A2 File Offset: 0x001B0BA2
		protected static InputControlMapping LeftStickUpMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Up",
				Target = InputControlType.LeftStickUp,
				Source = InputDeviceProfile.Analog(1),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B2 RID: 22450 RVA: 0x001B29D5 File Offset: 0x001B0BD5
		protected static InputControlMapping LeftStickDownMapping()
		{
			return new InputControlMapping
			{
				Name = "Left Stick Down",
				Target = InputControlType.LeftStickDown,
				Source = InputDeviceProfile.Analog(1),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B3 RID: 22451 RVA: 0x001B2A08 File Offset: 0x001B0C08
		protected static InputControlMapping RightStickLeftMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Left",
				Target = InputControlType.RightStickLeft,
				Source = InputDeviceProfile.Analog(2),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B4 RID: 22452 RVA: 0x001B2A3B File Offset: 0x001B0C3B
		protected static InputControlMapping RightStickRightMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Right",
				Target = InputControlType.RightStickRight,
				Source = InputDeviceProfile.Analog(2),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B5 RID: 22453 RVA: 0x001B2A6F File Offset: 0x001B0C6F
		protected static InputControlMapping RightStickUpMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Up",
				Target = InputControlType.RightStickUp,
				Source = InputDeviceProfile.Analog(3),
				SourceRange = InputRangeType.ZeroToMinusOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B6 RID: 22454 RVA: 0x001B2AA2 File Offset: 0x001B0CA2
		protected static InputControlMapping RightStickDownMapping()
		{
			return new InputControlMapping
			{
				Name = "Right Stick Down",
				Target = InputControlType.RightStickDown,
				Source = InputDeviceProfile.Analog(3),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B7 RID: 22455 RVA: 0x001B2AD5 File Offset: 0x001B0CD5
		protected static InputControlMapping LeftTriggerMapping(string name = "Left Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.LeftTrigger,
				Source = InputDeviceProfile.Analog(4),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B8 RID: 22456 RVA: 0x001B2B05 File Offset: 0x001B0D05
		protected static InputControlMapping RightTriggerMapping(string name = "Right Trigger")
		{
			return new InputControlMapping
			{
				Name = name,
				Target = InputControlType.RightTrigger,
				Source = InputDeviceProfile.Analog(5),
				SourceRange = InputRangeType.ZeroToOne,
				TargetRange = InputRangeType.ZeroToOne
			};
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x001B2B35 File Offset: 0x001B0D35
		protected static InputControlMapping AccelerometerXMapping()
		{
			return new InputControlMapping
			{
				Name = "Accelerometer X",
				Target = InputControlType.AccelerometerX,
				Source = InputDeviceProfile.Analog(6),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		// Token: 0x060057BA RID: 22458 RVA: 0x001B2B73 File Offset: 0x001B0D73
		protected static InputControlMapping AccelerometerYMapping()
		{
			return new InputControlMapping
			{
				Name = "Accelerometer Y",
				Target = InputControlType.AccelerometerY,
				Source = InputDeviceProfile.Analog(7),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		// Token: 0x060057BB RID: 22459 RVA: 0x001B2BB1 File Offset: 0x001B0DB1
		protected static InputControlMapping AccelerometerZMapping()
		{
			return new InputControlMapping
			{
				Name = "Accelerometer Z",
				Target = InputControlType.AccelerometerZ,
				Source = InputDeviceProfile.Analog(8),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x001B2BEF File Offset: 0x001B0DEF
		protected static InputControlMapping GyroscopeXMapping()
		{
			return new InputControlMapping
			{
				Name = "Gyroscope X",
				Target = InputControlType.TiltX,
				Source = InputDeviceProfile.Analog(9),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x001B2C2E File Offset: 0x001B0E2E
		protected static InputControlMapping GyroscopeYMapping()
		{
			return new InputControlMapping
			{
				Name = "Gyroscope Y",
				Target = InputControlType.TiltY,
				Source = InputDeviceProfile.Analog(10),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x001B2C6D File Offset: 0x001B0E6D
		protected static InputControlMapping GyroscopeZMapping()
		{
			return new InputControlMapping
			{
				Name = "Gyroscope Z",
				Target = InputControlType.TiltZ,
				Source = InputDeviceProfile.Analog(11),
				SourceRange = InputRangeType.MinusOneToOne,
				TargetRange = InputRangeType.MinusOneToOne,
				Passive = true
			};
		}

		// Token: 0x02001B6C RID: 7020
		protected enum SDLButtonType
		{
			// Token: 0x04009CD7 RID: 40151
			SDL_CONTROLLER_BUTTON_INVALID = -1,
			// Token: 0x04009CD8 RID: 40152
			SDL_CONTROLLER_BUTTON_A,
			// Token: 0x04009CD9 RID: 40153
			SDL_CONTROLLER_BUTTON_B,
			// Token: 0x04009CDA RID: 40154
			SDL_CONTROLLER_BUTTON_X,
			// Token: 0x04009CDB RID: 40155
			SDL_CONTROLLER_BUTTON_Y,
			// Token: 0x04009CDC RID: 40156
			SDL_CONTROLLER_BUTTON_BACK,
			// Token: 0x04009CDD RID: 40157
			SDL_CONTROLLER_BUTTON_GUIDE,
			// Token: 0x04009CDE RID: 40158
			SDL_CONTROLLER_BUTTON_START,
			// Token: 0x04009CDF RID: 40159
			SDL_CONTROLLER_BUTTON_LEFTSTICK,
			// Token: 0x04009CE0 RID: 40160
			SDL_CONTROLLER_BUTTON_RIGHTSTICK,
			// Token: 0x04009CE1 RID: 40161
			SDL_CONTROLLER_BUTTON_LEFTSHOULDER,
			// Token: 0x04009CE2 RID: 40162
			SDL_CONTROLLER_BUTTON_RIGHTSHOULDER,
			// Token: 0x04009CE3 RID: 40163
			SDL_CONTROLLER_BUTTON_DPAD_UP,
			// Token: 0x04009CE4 RID: 40164
			SDL_CONTROLLER_BUTTON_DPAD_DOWN,
			// Token: 0x04009CE5 RID: 40165
			SDL_CONTROLLER_BUTTON_DPAD_LEFT,
			// Token: 0x04009CE6 RID: 40166
			SDL_CONTROLLER_BUTTON_DPAD_RIGHT,
			// Token: 0x04009CE7 RID: 40167
			SDL_CONTROLLER_BUTTON_MISC1,
			// Token: 0x04009CE8 RID: 40168
			SDL_CONTROLLER_BUTTON_PADDLE1,
			// Token: 0x04009CE9 RID: 40169
			SDL_CONTROLLER_BUTTON_PADDLE2,
			// Token: 0x04009CEA RID: 40170
			SDL_CONTROLLER_BUTTON_PADDLE3,
			// Token: 0x04009CEB RID: 40171
			SDL_CONTROLLER_BUTTON_PADDLE4,
			// Token: 0x04009CEC RID: 40172
			SDL_CONTROLLER_BUTTON_TOUCHPAD,
			// Token: 0x04009CED RID: 40173
			SDL_CONTROLLER_BUTTON_MAX
		}
	}
}
