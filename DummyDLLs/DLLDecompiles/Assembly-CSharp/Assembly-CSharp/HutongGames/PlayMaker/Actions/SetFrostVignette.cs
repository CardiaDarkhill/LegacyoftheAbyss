using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200138F RID: 5007
	public sealed class SetFrostVignette : FsmStateAction
	{
		// Token: 0x060080A1 RID: 32929 RVA: 0x0025ED08 File Offset: 0x0025CF08
		public override void Reset()
		{
			this.FrostVignetteTargetValue = null;
		}

		// Token: 0x060080A2 RID: 32930 RVA: 0x0025ED14 File Offset: 0x0025CF14
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.SetFrostAmount(this.FrostVignetteTargetValue.Value);
			}
			StatusVignette.SetFrostVignetteAmount(this.FrostVignetteTargetValue.Value);
			base.Finish();
		}

		// Token: 0x04007FED RID: 32749
		public FsmFloat FrostVignetteTargetValue;
	}
}
