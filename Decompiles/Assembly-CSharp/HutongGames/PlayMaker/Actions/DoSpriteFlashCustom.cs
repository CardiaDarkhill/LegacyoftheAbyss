using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011FA RID: 4602
	public class DoSpriteFlashCustom : FsmStateAction
	{
		// Token: 0x06007A8F RID: 31375 RVA: 0x0024CB8C File Offset: 0x0024AD8C
		[UsedImplicitly]
		private bool HideCancelOnExit()
		{
			return !this.Repeating.Value;
		}

		// Token: 0x06007A90 RID: 31376 RVA: 0x0024CB9C File Offset: 0x0024AD9C
		public override void Reset()
		{
			this.Target = null;
			this.FlashColour = null;
			this.Amount = null;
			this.TimeUp = null;
			this.StayTime = null;
			this.TimeDown = null;
			this.StayDownTime = null;
			this.Repeating = null;
			this.RepeatTimes = null;
			this.RepeatingPriority = null;
			this.CancelOnExit = null;
		}

		// Token: 0x06007A91 RID: 31377 RVA: 0x0024CBF8 File Offset: 0x0024ADF8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.spriteFlash = (safe ? safe.GetComponent<SpriteFlash>() : null);
			if (this.spriteFlash == null)
			{
				this.flashHandle = null;
				base.Finish();
				return;
			}
			this.flashHandle = new SpriteFlash.FlashHandle?(this.spriteFlash.Flash(this.FlashColour.Value, this.Amount.Value, this.TimeUp.Value, this.StayTime.Value, this.TimeDown.Value, this.StayDownTime.Value, this.Repeating.Value, this.RepeatTimes.Value, this.RepeatingPriority.Value, false));
			if (!this.Repeating.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A92 RID: 31378 RVA: 0x0024CCD7 File Offset: 0x0024AED7
		public override void OnExit()
		{
			if (!this.CancelOnExit.Value || this.flashHandle == null)
			{
				return;
			}
			this.spriteFlash.CancelRepeatingFlash(this.flashHandle.Value);
		}

		// Token: 0x04007ACE RID: 31438
		public FsmOwnerDefault Target;

		// Token: 0x04007ACF RID: 31439
		public FsmColor FlashColour;

		// Token: 0x04007AD0 RID: 31440
		public FsmFloat Amount;

		// Token: 0x04007AD1 RID: 31441
		public FsmFloat TimeUp;

		// Token: 0x04007AD2 RID: 31442
		public FsmFloat StayTime;

		// Token: 0x04007AD3 RID: 31443
		public FsmFloat TimeDown;

		// Token: 0x04007AD4 RID: 31444
		public FsmFloat StayDownTime;

		// Token: 0x04007AD5 RID: 31445
		public FsmBool Repeating;

		// Token: 0x04007AD6 RID: 31446
		public FsmInt RepeatTimes;

		// Token: 0x04007AD7 RID: 31447
		public FsmInt RepeatingPriority;

		// Token: 0x04007AD8 RID: 31448
		[HideIf("HideCancelOnExit")]
		public FsmBool CancelOnExit;

		// Token: 0x04007AD9 RID: 31449
		private SpriteFlash spriteFlash;

		// Token: 0x04007ADA RID: 31450
		private SpriteFlash.FlashHandle? flashHandle;
	}
}
