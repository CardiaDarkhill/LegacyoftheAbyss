using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001397 RID: 5015
	public class StartGameplayTimer : FSMUtility.GetComponentFsmStateAction<GameplayTimer>
	{
		// Token: 0x060080BB RID: 32955 RVA: 0x0025F080 File Offset: 0x0025D280
		public override void Reset()
		{
			base.Reset();
			this.Duration = null;
		}

		// Token: 0x060080BC RID: 32956 RVA: 0x0025F08F File Offset: 0x0025D28F
		protected override void DoAction(GameplayTimer component)
		{
			component.StartTimer(this.Duration.Value);
		}

		// Token: 0x04007FFC RID: 32764
		public FsmFloat Duration;
	}
}
