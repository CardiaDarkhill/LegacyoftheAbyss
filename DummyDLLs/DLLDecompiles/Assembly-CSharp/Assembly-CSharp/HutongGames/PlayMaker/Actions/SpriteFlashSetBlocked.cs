using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011FD RID: 4605
	public class SpriteFlashSetBlocked : FsmStateAction
	{
		// Token: 0x06007A9D RID: 31389 RVA: 0x0024CEFE File Offset: 0x0024B0FE
		public override void Reset()
		{
			this.Target = null;
			this.SetBlocked = null;
		}

		// Token: 0x06007A9E RID: 31390 RVA: 0x0024CF10 File Offset: 0x0024B110
		public override void OnEnter()
		{
			SpriteFlash safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.IsBlocked = this.SetBlocked.Value;
			}
			base.Finish();
		}

		// Token: 0x04007AE5 RID: 31461
		[RequiredField]
		[CheckForComponent(typeof(SpriteFlash))]
		public FsmOwnerDefault Target;

		// Token: 0x04007AE6 RID: 31462
		public FsmBool SetBlocked;
	}
}
