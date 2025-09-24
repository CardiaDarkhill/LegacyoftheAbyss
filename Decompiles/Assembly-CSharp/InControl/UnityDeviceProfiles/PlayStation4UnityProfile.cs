using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009BF RID: 2495
	[Preserve]
	[UnityInputDeviceProfile]
	public class PlayStation4UnityProfile : InputDeviceProfile
	{
		// Token: 0x060055C1 RID: 21953 RVA: 0x00196968 File Offset: 0x00194B68
		public override void Define()
		{
			base.Define();
			base.DeviceName = "DUALSHOCK®4 wireless controller";
			base.DeviceNotes = "DUALSHOCK®4 wireless controller on PlayStation®4 system";
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.PlayStation4;
			base.IncludePlatforms = new string[]
			{
				"PS4",
				"ORBIS"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NamePattern = "controller"
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "cross button",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Name = "circle button",
					Target = InputControlType.Action2,
					Source = InputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Name = "square button",
					Target = InputControlType.Action3,
					Source = InputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Name = "triangle button",
					Target = InputControlType.Action4,
					Source = InputDeviceProfile.Button(3)
				},
				new InputControlMapping
				{
					Name = "L1 button",
					Target = InputControlType.LeftBumper,
					Source = InputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Name = "R1 button",
					Target = InputControlType.RightBumper,
					Source = InputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Name = "touch pad button",
					Target = InputControlType.TouchPadButton,
					Source = InputDeviceProfile.Button(6)
				},
				new InputControlMapping
				{
					Name = "OPTIONS button",
					Target = InputControlType.Options,
					Source = InputDeviceProfile.Button(7)
				},
				new InputControlMapping
				{
					Name = "L3 button",
					Target = InputControlType.LeftStickButton,
					Source = InputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Name = "R3 button",
					Target = InputControlType.RightStickButton,
					Source = InputDeviceProfile.Button(9)
				}
			};
			base.AnalogMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "left stick left",
					Target = InputControlType.LeftStickLeft,
					Source = InputDeviceProfile.Analog(0),
					SourceRange = InputRangeType.ZeroToMinusOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "left stick right",
					Target = InputControlType.LeftStickRight,
					Source = InputDeviceProfile.Analog(0),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "left stick up",
					Target = InputControlType.LeftStickUp,
					Source = InputDeviceProfile.Analog(1),
					SourceRange = InputRangeType.ZeroToMinusOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "left stick down",
					Target = InputControlType.LeftStickDown,
					Source = InputDeviceProfile.Analog(1),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "right stick left",
					Target = InputControlType.RightStickLeft,
					Source = InputDeviceProfile.Analog(3),
					SourceRange = InputRangeType.ZeroToMinusOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "right stick right",
					Target = InputControlType.RightStickRight,
					Source = InputDeviceProfile.Analog(3),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "right stick up",
					Target = InputControlType.RightStickUp,
					Source = InputDeviceProfile.Analog(4),
					SourceRange = InputRangeType.ZeroToMinusOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "right stick down",
					Target = InputControlType.RightStickDown,
					Source = InputDeviceProfile.Analog(4),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "left button",
					Target = InputControlType.DPadLeft,
					Source = InputDeviceProfile.Analog(5),
					SourceRange = InputRangeType.ZeroToMinusOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "right button",
					Target = InputControlType.DPadRight,
					Source = InputDeviceProfile.Analog(5),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "up button",
					Target = InputControlType.DPadUp,
					Source = InputDeviceProfile.Analog(6),
					SourceRange = InputRangeType.ZeroToOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "down button",
					Target = InputControlType.DPadDown,
					Source = InputDeviceProfile.Analog(6),
					SourceRange = InputRangeType.ZeroToMinusOne,
					TargetRange = InputRangeType.ZeroToOne
				},
				new InputControlMapping
				{
					Name = "L2 button",
					Target = InputControlType.LeftTrigger,
					Source = InputDeviceProfile.Analog(7)
				},
				new InputControlMapping
				{
					Name = "R2 button",
					Target = InputControlType.RightTrigger,
					Source = InputDeviceProfile.Analog(2),
					Invert = true
				}
			};
		}
	}
}
