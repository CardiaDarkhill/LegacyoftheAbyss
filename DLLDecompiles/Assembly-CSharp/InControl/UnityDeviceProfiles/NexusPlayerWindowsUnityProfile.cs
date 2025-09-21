using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009E8 RID: 2536
	[Preserve]
	[UnityInputDeviceProfile]
	public class NexusPlayerWindowsUnityProfile : InputDeviceProfile
	{
		// Token: 0x06005613 RID: 22035 RVA: 0x001A1190 File Offset: 0x0019F390
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Nexus Player Controller";
			base.DeviceNotes = "Nexus Player Controller on Windows";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"Windows"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "GamePad"
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "A",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(10)
				},
				new InputControlMapping
				{
					Name = "B",
					Target = InputControlType.Action2,
					Source = InputDeviceProfile.Button(9)
				},
				new InputControlMapping
				{
					Name = "X",
					Target = InputControlType.Action3,
					Source = InputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Name = "Y",
					Target = InputControlType.Action4,
					Source = InputDeviceProfile.Button(7)
				},
				new InputControlMapping
				{
					Name = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = InputDeviceProfile.Button(6)
				},
				new InputControlMapping
				{
					Name = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = InputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Name = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = InputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Name = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = InputDeviceProfile.Button(3)
				},
				new InputControlMapping
				{
					Name = "Back",
					Target = InputControlType.Select,
					Source = InputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Name = "Start",
					Target = InputControlType.Start,
					Source = InputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Name = "System",
					Target = InputControlType.System,
					Source = InputDeviceProfile.Button(2)
				}
			};
			base.AnalogMappings = new InputControlMapping[]
			{
				InputDeviceProfile.LeftStickLeftMapping(0),
				InputDeviceProfile.LeftStickRightMapping(0),
				InputDeviceProfile.LeftStickUpMapping(1),
				InputDeviceProfile.LeftStickDownMapping(1),
				InputDeviceProfile.RightStickLeftMapping(2),
				InputDeviceProfile.RightStickRightMapping(2),
				InputDeviceProfile.RightStickUpMapping(3),
				InputDeviceProfile.RightStickDownMapping(3),
				InputDeviceProfile.DPadLeftMapping(4),
				InputDeviceProfile.DPadRightMapping(4),
				InputDeviceProfile.DPadUpMapping2(5),
				InputDeviceProfile.DPadDownMapping2(5)
			};
		}
	}
}
