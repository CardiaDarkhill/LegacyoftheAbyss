using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E86 RID: 3718
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Causes the device to vibrate for half a second.\nNOTE: Unity's built in vibrate function is fairly limited. However there are alternatives available on the Asset Store...")]
	public class DeviceVibrate : FsmStateAction
	{
		// Token: 0x060069B6 RID: 27062 RVA: 0x002113AC File Offset: 0x0020F5AC
		public override void Reset()
		{
		}

		// Token: 0x060069B7 RID: 27063 RVA: 0x002113AE File Offset: 0x0020F5AE
		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
