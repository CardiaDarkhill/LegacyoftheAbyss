using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F9 RID: 4601
	public class DoSpriteFlash : FsmStateAction
	{
		// Token: 0x06007A8A RID: 31370 RVA: 0x0024CA6F File Offset: 0x0024AC6F
		[UsedImplicitly]
		private bool HideCancelOnExit()
		{
			return !this.IsRepeating.Value;
		}

		// Token: 0x06007A8B RID: 31371 RVA: 0x0024CA7F File Offset: 0x0024AC7F
		public override void Reset()
		{
			this.Target = null;
			this.FlashMethod = null;
			this.IsRepeating = null;
			this.CancelOnExit = null;
		}

		// Token: 0x06007A8C RID: 31372 RVA: 0x0024CAA0 File Offset: 0x0024ACA0
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
			MethodInfo method = typeof(SpriteFlash).GetMethod(this.FlashMethod.Value);
			this.flashHandle = ((method != null) ? (method.Invoke(this.spriteFlash, null) as SpriteFlash.FlashHandle?) : null);
			if (!this.IsRepeating.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A8D RID: 31373 RVA: 0x0024CB51 File Offset: 0x0024AD51
		public override void OnExit()
		{
			if (!this.CancelOnExit.Value || this.flashHandle == null)
			{
				return;
			}
			this.spriteFlash.CancelRepeatingFlash(this.flashHandle.Value);
		}

		// Token: 0x04007AC8 RID: 31432
		public FsmOwnerDefault Target;

		// Token: 0x04007AC9 RID: 31433
		public FsmString FlashMethod;

		// Token: 0x04007ACA RID: 31434
		public FsmBool IsRepeating;

		// Token: 0x04007ACB RID: 31435
		[HideIf("HideCancelOnExit")]
		public FsmBool CancelOnExit;

		// Token: 0x04007ACC RID: 31436
		private SpriteFlash spriteFlash;

		// Token: 0x04007ACD RID: 31437
		private SpriteFlash.FlashHandle? flashHandle;
	}
}
