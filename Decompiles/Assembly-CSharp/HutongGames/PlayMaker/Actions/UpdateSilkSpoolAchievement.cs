using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F1 RID: 4849
	public class UpdateSilkSpoolAchievement : FsmStateAction
	{
		// Token: 0x06007E4D RID: 32333 RVA: 0x00258A6C File Offset: 0x00256C6C
		public override void Reset()
		{
		}

		// Token: 0x06007E4E RID: 32334 RVA: 0x00258A6E File Offset: 0x00256C6E
		public override void OnEnter()
		{
			GameManager.instance.CheckSilkSpoolAchievements();
			base.Finish();
		}
	}
}
