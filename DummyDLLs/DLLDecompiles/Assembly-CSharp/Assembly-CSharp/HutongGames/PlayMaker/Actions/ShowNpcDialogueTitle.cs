using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001358 RID: 4952
	public class ShowNpcDialogueTitle : FSMUtility.GetComponentFsmStateAction<NpcDialogueTitle>
	{
		// Token: 0x06007FD4 RID: 32724 RVA: 0x0025C913 File Offset: 0x0025AB13
		protected override void DoAction(NpcDialogueTitle component)
		{
			component.EnableAndShow();
		}
	}
}
