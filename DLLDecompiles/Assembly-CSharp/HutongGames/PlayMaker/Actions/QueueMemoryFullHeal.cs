using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200126E RID: 4718
	public sealed class QueueMemoryFullHeal : FsmStateAction
	{
		// Token: 0x06007C65 RID: 31845 RVA: 0x00253003 File Offset: 0x00251203
		public override void Reset()
		{
		}

		// Token: 0x06007C66 RID: 31846 RVA: 0x00253008 File Offset: 0x00251208
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			PlayerData playerData = null;
			if (instance != null)
			{
				playerData = instance.playerData;
			}
			if (playerData != null)
			{
				playerData.PreMemoryState.DoFullHeal = true;
			}
			base.Finish();
		}
	}
}
