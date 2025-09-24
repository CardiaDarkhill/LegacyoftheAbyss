using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x02000983 RID: 2435
	[Preserve]
	[UnityInputDeviceProfile]
	public class NvidiaShieldRemoteAndroidUnityProfile : InputDeviceProfile
	{
		// Token: 0x06005549 RID: 21833 RVA: 0x0018B840 File Offset: 0x00189A40
		public override void Define()
		{
			base.Define();
			base.DeviceName = "NVIDIA Shield Remote";
			base.DeviceNotes = "NVIDIA Shield Remote on Android";
			base.DeviceClass = InputDeviceClass.Remote;
			base.DeviceStyle = InputDeviceStyle.NVIDIAShield;
			base.IncludePlatforms = new string[]
			{
				"Android"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "SHIELD Remote"
				},
				new InputDeviceMatcher
				{
					NamePattern = "SHIELD Remote"
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "A",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(0)
				}
			};
			base.AnalogMappings = new InputControlMapping[]
			{
				InputDeviceProfile.DPadLeftMapping(4),
				InputDeviceProfile.DPadRightMapping(4),
				InputDeviceProfile.DPadUpMapping(5),
				InputDeviceProfile.DPadDownMapping(5)
			};
		}
	}
}
