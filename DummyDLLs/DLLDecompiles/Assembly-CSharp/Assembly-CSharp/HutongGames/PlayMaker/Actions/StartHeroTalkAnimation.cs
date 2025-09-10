using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001357 RID: 4951
	public sealed class StartHeroTalkAnimation : FsmStateAction
	{
		// Token: 0x06007FD1 RID: 32721 RVA: 0x0025C8D1 File Offset: 0x0025AAD1
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007FD2 RID: 32722 RVA: 0x0025C8DC File Offset: 0x0025AADC
		public override void OnEnter()
		{
			NPCControlBase safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.BeginHeroTalkAnimation();
			}
			base.Finish();
		}

		// Token: 0x04007F4B RID: 32587
		[RequiredField]
		[CheckForComponent(typeof(NPCControlBase))]
		public FsmOwnerDefault Target;
	}
}
