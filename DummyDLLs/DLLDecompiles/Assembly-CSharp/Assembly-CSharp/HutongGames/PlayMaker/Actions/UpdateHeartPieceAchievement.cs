using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F0 RID: 4848
	public class UpdateHeartPieceAchievement : FsmStateAction
	{
		// Token: 0x06007E4A RID: 32330 RVA: 0x00258A50 File Offset: 0x00256C50
		public override void Reset()
		{
		}

		// Token: 0x06007E4B RID: 32331 RVA: 0x00258A52 File Offset: 0x00256C52
		public override void OnEnter()
		{
			GameManager.instance.CheckHeartAchievements();
			base.Finish();
		}
	}
}
