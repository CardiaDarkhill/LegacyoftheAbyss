using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001359 RID: 4953
	public class HideNpcDialogueTitle : FSMUtility.GetComponentFsmStateAction<NpcDialogueTitle>
	{
		// Token: 0x06007FD6 RID: 32726 RVA: 0x0025C923 File Offset: 0x0025AB23
		protected override void DoAction(NpcDialogueTitle component)
		{
			component.Hide();
		}
	}
}
