using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009AD RID: 2477
	[Preserve]
	[UnityInputDeviceProfile]
	public class PlayStation4MacBluetoothUnityProfile : InputDeviceProfile
	{
		// Token: 0x0600559D RID: 21917 RVA: 0x001937C4 File Offset: 0x001919C4
		public override void Define()
		{
			base.Define();
			string str = "®";
			base.DeviceName = "PlayStation 4 Controller";
			base.DeviceNotes = "PlayStation 4 Controller on macOS";
			base.DeviceClass = InputDeviceClass.Controller;
			base.DeviceStyle = InputDeviceStyle.PlayStation4;
			base.IncludePlatforms = new string[]
			{
				"OS X"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "Unknown Wireless Controller"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Sony Interactive Entertainment DUALSHOCK" + str + "4 USB Wireless Adaptor"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Unknown DUALSHOCK 4 Wireless Controller"
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "Cross",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(1)
				},
				new InputControlMapping
				{
					Name = "Circle",
					Target = InputControlType.Action2,
					Source = InputDeviceProfile.Button(2)
				},
				new InputControlMapping
				{
					Name = "Square",
					Target = InputControlType.Action3,
					Source = InputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Name = "Triangle",
					Target = InputControlType.Action4,
					Source = InputDeviceProfile.Button(3)
				},
				new InputControlMapping
				{
					Name = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = InputDeviceProfile.Button(4)
				},
				new InputControlMapping
				{
					Name = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = InputDeviceProfile.Button(5)
				},
				new InputControlMapping
				{
					Name = "Share",
					Target = InputControlType.Share,
					Source = InputDeviceProfile.Button(8)
				},
				new InputControlMapping
				{
					Name = "Options",
					Target = InputControlType.Options,
					Source = InputDeviceProfile.Button(9)
				},
				new InputControlMapping
				{
					Name = "L3",
					Target = InputControlType.LeftStickButton,
					Source = InputDeviceProfile.Button(10)
				},
				new InputControlMapping
				{
					Name = "R3",
					Target = InputControlType.RightStickButton,
					Source = InputDeviceProfile.Button(11)
				},
				new InputControlMapping
				{
					Name = "System",
					Target = InputControlType.System,
					Source = InputDeviceProfile.Button(12)
				},
				new InputControlMapping
				{
					Name = "TouchPad Button",
					Target = InputControlType.TouchPadButton,
					Source = InputDeviceProfile.Button(13)
				}
			};
		}
	}
}
