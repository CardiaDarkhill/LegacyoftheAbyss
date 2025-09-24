using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200135A RID: 4954
	public class NpcDialogueTitleSkipNextHide : FSMUtility.GetComponentFsmStateAction<NpcDialogueTitle>
	{
		// Token: 0x06007FD8 RID: 32728 RVA: 0x0025C933 File Offset: 0x0025AB33
		protected override void DoAction(NpcDialogueTitle component)
		{
			component.SkipNextHide();
		}
	}
}
