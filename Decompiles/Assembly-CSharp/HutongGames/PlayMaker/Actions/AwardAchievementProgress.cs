using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012ED RID: 4845
	public class AwardAchievementProgress : FsmStateAction
	{
		// Token: 0x06007E41 RID: 32321 RVA: 0x00258902 File Offset: 0x00256B02
		public override void Reset()
		{
			this.Key = null;
			this.CurrentValue = new FsmInt
			{
				UseVariable = true
			};
			this.MaxValue = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x06007E42 RID: 32322 RVA: 0x00258930 File Offset: 0x00256B30
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!string.IsNullOrWhiteSpace(this.Key.Value))
			{
				if (this.CurrentValue.IsNone)
				{
					instance.AwardAchievement(this.Key.Value);
				}
				else
				{
					instance.UpdateAchievementProgress(this.Key.Value, this.CurrentValue.Value, this.MaxValue.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007E10 RID: 32272
		public FsmString Key;

		// Token: 0x04007E11 RID: 32273
		public FsmInt CurrentValue;

		// Token: 0x04007E12 RID: 32274
		public FsmInt MaxValue;
	}
}
