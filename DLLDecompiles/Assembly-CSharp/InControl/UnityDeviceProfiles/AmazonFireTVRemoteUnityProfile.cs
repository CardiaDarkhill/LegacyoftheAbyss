using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x0200094C RID: 2380
	[Preserve]
	[UnityInputDeviceProfile]
	public class AmazonFireTVRemoteUnityProfile : InputDeviceProfile
	{
		// Token: 0x060054DB RID: 21723 RVA: 0x001825B4 File Offset: 0x001807B4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "Amazon Fire TV Remote";
			base.DeviceNotes = "Amazon Fire TV Remote on Amazon Fire TV";
			base.DeviceClass = InputDeviceClass.Remote;
			base.DeviceStyle = InputDeviceStyle.AmazonFireTV;
			base.IncludePlatforms = new string[]
			{
				"Amazon AFT"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = ""
				},
				new InputDeviceMatcher
				{
					NameLiteral = "Amazon Fire TV Remote"
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
				},
				new InputControlMapping
				{
					Name = "Menu",
					Target = InputControlType.Menu,
					Source = InputDeviceProfile.MenuKey
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
