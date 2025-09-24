using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011FB RID: 4603
	public class DoSpriteFlashNamed : FsmStateAction
	{
		// Token: 0x06007A94 RID: 31380 RVA: 0x0024CD12 File Offset: 0x0024AF12
		public override void Reset()
		{
			this.Target = null;
			this.Flash = null;
			this.CancelOnExit = null;
			this.FlashID = null;
		}

		// Token: 0x06007A95 RID: 31381 RVA: 0x0024CD30 File Offset: 0x0024AF30
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.spriteFlash = (safe ? safe.GetComponent<SpriteFlash>() : null);
			if (this.spriteFlash == null)
			{
				this.flashHandle = null;
				base.Fsm.Event(this.FinishedFlashing);
				base.Finish();
				return;
			}
			SpriteFlash.FlashHandle value;
			if (this.TryDoFlash(out value))
			{
				this.flashHandle = new SpriteFlash.FlashHandle?(value);
				this.FlashID.Value = value.ID;
			}
			else
			{
				this.flashHandle = null;
			}
			if (!this.WaitForFlash.Value)
			{
				base.Fsm.Event(this.FinishedFlashing);
				base.Finish();
			}
		}

		// Token: 0x06007A96 RID: 31382 RVA: 0x0024CDED File Offset: 0x0024AFED
		public override void OnExit()
		{
			if (!this.CancelOnExit.Value || this.flashHandle == null)
			{
				return;
			}
			this.spriteFlash.CancelRepeatingFlash(this.flashHandle.Value);
		}

		// Token: 0x06007A97 RID: 31383 RVA: 0x0024CE20 File Offset: 0x0024B020
		public override void OnUpdate()
		{
			if (this.flashHandle != null && this.spriteFlash.IsFlashing(this.flashHandle.Value.ID))
			{
				return;
			}
			base.Fsm.Event(this.FinishedFlashing);
			base.Finish();
		}

		// Token: 0x06007A98 RID: 31384 RVA: 0x0024CE72 File Offset: 0x0024B072
		private bool TryDoFlash(out SpriteFlash.FlashHandle flashHandle)
		{
			if ((DoSpriteFlashNamed.FlashVariants)this.Flash.Value == DoSpriteFlashNamed.FlashVariants.FlashingSuperDash)
			{
				flashHandle = this.spriteFlash.FlashingSuperDashHandled();
				return true;
			}
			flashHandle = default(SpriteFlash.FlashHandle);
			return false;
		}

		// Token: 0x04007ADB RID: 31451
		public FsmOwnerDefault Target;

		// Token: 0x04007ADC RID: 31452
		[ObjectType(typeof(DoSpriteFlashNamed.FlashVariants))]
		public FsmEnum Flash;

		// Token: 0x04007ADD RID: 31453
		public FsmBool CancelOnExit;

		// Token: 0x04007ADE RID: 31454
		[UIHint(UIHint.Variable)]
		public FsmInt FlashID;

		// Token: 0x04007ADF RID: 31455
		public FsmBool WaitForFlash;

		// Token: 0x04007AE0 RID: 31456
		public FsmEvent FinishedFlashing;

		// Token: 0x04007AE1 RID: 31457
		private SpriteFlash spriteFlash;

		// Token: 0x04007AE2 RID: 31458
		private SpriteFlash.FlashHandle? flashHandle;

		// Token: 0x02001BD6 RID: 7126
		[Serializable]
		private enum FlashVariants
		{
			// Token: 0x04009F16 RID: 40726
			FlashingSuperDash
		}
	}
}
