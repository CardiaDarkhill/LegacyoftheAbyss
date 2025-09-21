using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x02000958 RID: 2392
	[Preserve]
	[UnityInputDeviceProfile]
	public class AndroidTVRemoteUnityProfile : InputDeviceProfile
	{
		// Token: 0x060054F3 RID: 21747 RVA: 0x00184340 File Offset: 0x00182540
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Android TV Remote";
			base.DeviceNotes = "Android TV Remote on Android TV";
			base.DeviceClass = InputDeviceClass.Remote;
			base.IncludePlatforms = new string[]
			{
				"Android"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = ""
				},
				new InputDeviceMatcher
				{
					NameLiteral = "touch-input"
				},
				new InputDeviceMatcher
				{
					NameLiteral = "navigation-input"
				}
			};
			base.ButtonMappings = new InputControlMapping[]
			{
				new InputControlMapping
				{
					Name = "A",
					Target = InputControlType.Action1,
					Source = InputDeviceProfile.Button(0)
				},
				new InputControlMapping
				{
					Name = "Back",
					Target = InputControlType.Back,
					Source = InputDeviceProfile.EscapeKey
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
