using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012EF RID: 4847
	public class AwardQueuedAchievements : FsmStateAction
	{
		// Token: 0x06007E47 RID: 32327 RVA: 0x002589F8 File Offset: 0x00256BF8
		public override void Reset()
		{
			this.delay = null;
		}

		// Token: 0x06007E48 RID: 32328 RVA: 0x00258A04 File Offset: 0x00256C04
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (this.delay.Value > 0f)
			{
				instance.AwardQueuedAchievements(this.delay.Value);
			}
			else
			{
				instance.AwardQueuedAchievements();
			}
			base.Finish();
		}

		// Token: 0x04007E14 RID: 32276
		public FsmFloat delay;
	}
}
