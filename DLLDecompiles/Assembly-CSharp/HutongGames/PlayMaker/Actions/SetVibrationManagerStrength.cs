using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A4 RID: 5028
	public sealed class SetVibrationManagerStrength : FsmStateAction
	{
		// Token: 0x060080F6 RID: 33014 RVA: 0x0025FDEC File Offset: 0x0025DFEC
		public override void Reset()
		{
			this.strength = null;
			this.fadeDuration = null;
			this.resetOnExit = null;
			this.strengthOnExit = new FsmFloat
			{
				UseVariable = true
			};
			this.restoreFadeDuration = new FsmFloat
			{
				UseVariable = true
			};
			this.blockNextSceneAutoFade = null;
		}

		// Token: 0x060080F7 RID: 33015 RVA: 0x0025FE3C File Offset: 0x0025E03C
		public override void OnEnter()
		{
			this.originalStrength = VibrationManager.InternalStrength;
			VibrationManager.FadeVibration(this.strength.Value, this.fadeDuration.Value);
			if (this.blockNextSceneAutoFade.Value)
			{
				GameManager instance = GameManager.instance;
				if (instance)
				{
					instance.BlockNextVibrationFadeIn = true;
				}
			}
			else if (this.strength.Value > 0f && !this.blockNextSceneAutoFade.Value)
			{
				GameManager instance2 = GameManager.instance;
				if (instance2)
				{
					instance2.BlockNextVibrationFadeIn = false;
				}
			}
			base.Finish();
		}

		// Token: 0x060080F8 RID: 33016 RVA: 0x0025FED0 File Offset: 0x0025E0D0
		public override void OnExit()
		{
			if (this.resetOnExit.Value)
			{
				float duration = this.restoreFadeDuration.IsNone ? this.fadeDuration.Value : this.restoreFadeDuration.Value;
				VibrationManager.FadeVibration(this.strengthOnExit.IsNone ? this.originalStrength : this.strengthOnExit.Value, duration);
			}
		}

		// Token: 0x04008035 RID: 32821
		public FsmFloat strength;

		// Token: 0x04008036 RID: 32822
		public FsmFloat fadeDuration;

		// Token: 0x04008037 RID: 32823
		public FsmBool resetOnExit;

		// Token: 0x04008038 RID: 32824
		[Tooltip("Will restore original strength if not set")]
		public FsmFloat strengthOnExit;

		// Token: 0x04008039 RID: 32825
		public FsmFloat restoreFadeDuration;

		// Token: 0x0400803A RID: 32826
		public FsmBool blockNextSceneAutoFade;

		// Token: 0x0400803B RID: 32827
		private float originalStrength;
	}
}
