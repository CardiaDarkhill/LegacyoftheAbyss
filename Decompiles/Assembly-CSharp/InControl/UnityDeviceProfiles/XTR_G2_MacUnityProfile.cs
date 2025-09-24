using System;

namespace InControl.UnityDeviceProfiles
{
	// Token: 0x020009B9 RID: 2489
	[Preserve]
	[UnityInputDeviceProfile]
	public class XTR_G2_MacUnityProfile : InputDeviceProfile
	{
		// Token: 0x060055B5 RID: 21941 RVA: 0x00195A58 File Offset: 0x00193C58
		public override void Define()
		{
			base.Define();
			base.DeviceName = "KMODEL Simulator XTR G2 FMS Controller";
			base.DeviceNotes = "KMODEL Simulator XTR G2 FMS Controller on OS X";
			base.DeviceClass = InputDeviceClass.Controller;
			base.IncludePlatforms = new string[]
			{
				"OS X"
			};
			base.Matchers = new InputDeviceMatcher[]
			{
				new InputDeviceMatcher
				{
					NameLiteral = "FeiYing Model KMODEL Simulator - XTR+G2+FMS Controller"
				}
			};
		}
	}
}
