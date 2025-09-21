using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012CB RID: 4811
	[Tooltip("Used for starting Walker V2 (NOT a version 2 for controlling original Walker)")]
	public class StartWalkerV2 : FSMUtility.GetComponentFsmStateAction<WalkerV2>
	{
		// Token: 0x06007DAB RID: 32171 RVA: 0x00256FE7 File Offset: 0x002551E7
		protected override void DoAction(WalkerV2 walker)
		{
			walker.StartWalking();
		}

		// Token: 0x06007DAC RID: 32172 RVA: 0x00256FF0 File Offset: 0x002551F0
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				WalkerV2 component = base.Owner.GetComponent<WalkerV2>();
				if (component != null)
				{
					component.StopWalking();
				}
			}
		}

		// Token: 0x04007D95 RID: 32149
		public bool stopOnExit;
	}
}
