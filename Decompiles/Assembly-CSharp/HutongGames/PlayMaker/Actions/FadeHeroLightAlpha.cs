using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012DE RID: 4830
	public class FadeHeroLightAlpha : FsmStateAction
	{
		// Token: 0x06007DEE RID: 32238 RVA: 0x00257A18 File Offset: 0x00255C18
		public override void Reset()
		{
			this.Target = null;
			this.Alpha = null;
			this.FadeDuration = null;
			this.SetAlphaOnExit = null;
			this.StoreInitialAlpha = null;
		}

		// Token: 0x06007DEF RID: 32239 RVA: 0x00257A40 File Offset: 0x00255C40
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.light = safe.GetComponent<HeroLight>();
			}
			if (!this.light)
			{
				base.Finish();
				return;
			}
			this.startingAlpha = this.light.Alpha;
			this.StoreInitialAlpha.Value = this.startingAlpha;
			if (this.FadeDuration.Value > 0f)
			{
				return;
			}
			this.light.Alpha = this.Alpha.Value;
			base.Finish();
		}

		// Token: 0x06007DF0 RID: 32240 RVA: 0x00257AD4 File Offset: 0x00255CD4
		public override void OnUpdate()
		{
			float progress = this.GetProgress();
			this.light.Alpha = Mathf.Lerp(this.startingAlpha, this.Alpha.Value, progress);
			if (progress >= 1f)
			{
				base.Finish();
			}
		}

		// Token: 0x06007DF1 RID: 32241 RVA: 0x00257B18 File Offset: 0x00255D18
		public override void OnExit()
		{
			if (!this.light)
			{
				return;
			}
			if (this.SetAlphaOnExit.Value)
			{
				this.light.Alpha = this.Alpha.Value;
			}
		}

		// Token: 0x06007DF2 RID: 32242 RVA: 0x00257B4B File Offset: 0x00255D4B
		public override float GetProgress()
		{
			return base.State.StateTime / this.FadeDuration.Value;
		}

		// Token: 0x04007DC6 RID: 32198
		public FsmOwnerDefault Target;

		// Token: 0x04007DC7 RID: 32199
		public FsmFloat Alpha;

		// Token: 0x04007DC8 RID: 32200
		public FsmFloat FadeDuration;

		// Token: 0x04007DC9 RID: 32201
		public FsmBool SetAlphaOnExit;

		// Token: 0x04007DCA RID: 32202
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreInitialAlpha;

		// Token: 0x04007DCB RID: 32203
		private HeroLight light;

		// Token: 0x04007DCC RID: 32204
		private float startingAlpha;
	}
}
