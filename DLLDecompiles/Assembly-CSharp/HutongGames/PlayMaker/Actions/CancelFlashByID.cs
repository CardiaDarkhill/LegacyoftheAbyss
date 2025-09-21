using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011FC RID: 4604
	public class CancelFlashByID : FsmStateAction
	{
		// Token: 0x06007A9A RID: 31386 RVA: 0x0024CEAB File Offset: 0x0024B0AB
		public override void Reset()
		{
			this.Target = null;
			this.ID = null;
		}

		// Token: 0x06007A9B RID: 31387 RVA: 0x0024CEBC File Offset: 0x0024B0BC
		public override void OnEnter()
		{
			SpriteFlash safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.CancelFlashByID(this.ID.Value);
			}
			base.Finish();
		}

		// Token: 0x04007AE3 RID: 31459
		[RequiredField]
		[CheckForComponent(typeof(SpriteFlash))]
		public FsmOwnerDefault Target;

		// Token: 0x04007AE4 RID: 31460
		public FsmInt ID;
	}
}
