using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200139A RID: 5018
	public sealed class StartRosaryCannonCharge : FsmStateAction
	{
		// Token: 0x060080C5 RID: 32965 RVA: 0x0025F166 File Offset: 0x0025D366
		public override void Reset()
		{
		}

		// Token: 0x060080C6 RID: 32966 RVA: 0x0025F168 File Offset: 0x0025D368
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				HeroVibrationController vibrationCtrl = instance.GetVibrationCtrl();
				if (vibrationCtrl != null)
				{
					vibrationCtrl.StartRosaryCannonCharge();
				}
			}
			base.Finish();
		}
	}
}
