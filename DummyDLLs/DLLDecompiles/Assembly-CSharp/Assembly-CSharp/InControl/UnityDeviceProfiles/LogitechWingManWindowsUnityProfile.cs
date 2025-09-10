using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009E4 RID: 2532
	[Preserve]
	[UnityInputDeviceProfile]
	public class LogitechWingManWindowsUnityProfile : InputDeviceProfile
	{
		// Token: 0x0600560B RID: 22027 RVA: 0x001A05CC File Offset: 0x0019E7CC
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Logitech WingMan Controller";
			base.DeviceNotes = "Logitech WingMan Controller on Windows";
			base.DeviceClass = InputDeviceClass.FlightStick;
			base.IncludePlatforms = new string[]
			{
				"Windows"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "WingMan Cordless Gamepad"
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "A",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Name = "B",
					Target = InputControlType.Action2,
					Source = InputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Name = "C",
					Target = InputControlType.Action5,
					Source = InputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Name = "X",
					Target = InputControlType.Action3,
					Source = InputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Name = "Y",
					Target = InputControlType.Action4,
					Source = InputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Name = "Z",
					Target = InputControlType.Action6,
					Source = InputDeviceProfile.Button(6)
				},
				new InputControlMapping
				{
					Name = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = InputDeviceProfile.Button(7)
				},
				new InputControlMapping
				{
					Name = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = InputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Name = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = InputDeviceProfile.Button(10)
				},
				new InputControlMapping
				{
					Name = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = InputDeviceProfile.Button(11)
				},
				new InputControlMapping
				{
					Name = "Start",
					Target = InputControlType.Start,
					Source = InputDeviceProfile.Button(9)
				}
			};
			base.AnalogMappings = new InputControlMapping[]
			{
				InputDeviceProfile.LeftStickLeftMapping(0),
				InputDeviceProfile.LeftStickRightMapping(0),
				InputDeviceProfile.LeftStickUpMapping(1),
				InputDeviceProfile.LeftStickDownMapping(1),
				InputDeviceProfile.RightStickLeftMapping(3),
				InputDeviceProfile.RightStickRightMapping(3),
				InputDeviceProfile.RightStickUpMapping(4),
				InputDeviceProfile.RightStickDownMapping(4),
				InputDeviceProfile.DPadLeftMapping(5),
				InputDeviceProfile.DPadRightMapping(5),
				InputDeviceProfile.DPadUpMapping(6),
				InputDeviceProfile.DPadDownMapping(6),
				new InputControlMapping
				{
					Name = "Throttle",
					Target = InputControlType.Analog0,
					Source = InputDeviceProfile.Analog(2)
				}
			};
		}
	}
}
