using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200139B RID: 5019
	public sealed class StopRosaryCannonCharge : FsmStateAction
	{
		// Token: 0x060080C8 RID: 32968 RVA: 0x0025F1A8 File Offset: 0x0025D3A8
		public override void Reset()
		{
		}

		// Token: 0x060080C9 RID: 32969 RVA: 0x0025F1AC File Offset: 0x0025D3AC
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				HeroVibrationController vibrationCtrl = instance.GetVibrationCtrl();
				if (vibrationCtrl != null)
				{
					vibrationCtrl.StopRosaryCannonCharge();
				}
			}
			base.Finish();
		}
	}
}
