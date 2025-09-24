using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009B8 RID: 2488
	[Preserve]
	[UnityInputDeviceProfile]
	public class XTR55_G2_MacUnityProfile : InputDeviceProfile
	{
		// Token: 0x060055B3 RID: 21939 RVA: 0x001959E4 File Offset: 0x00193BE4
		public override void Define()
		{
			base.Define();
			base.DeviceName = "SAILI Simulator XTR5.5 G2 FMS Controller";
			base.DeviceNotes = "SAILI Simulator XTR5.5 G2 FMS Controller on OS X";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"OS X"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "              SAILI Simulator --- XTR5.5+G2+FMS Controller"
				}
			};
		}
	}
}
