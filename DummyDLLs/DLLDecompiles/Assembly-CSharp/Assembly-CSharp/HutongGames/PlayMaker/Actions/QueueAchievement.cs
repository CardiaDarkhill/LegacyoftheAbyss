using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012EE RID: 4846
	public class QueueAchievement : FsmStateAction
	{
		// Token: 0x06007E44 RID: 32324 RVA: 0x002589AA File Offset: 0x00256BAA
		public override void Reset()
		{
			this.Key = null;
		}

		// Token: 0x06007E45 RID: 32325 RVA: 0x002589B4 File Offset: 0x00256BB4
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!string.IsNullOrWhiteSpace(this.Key.Value))
			{
				instance.QueueAchievement(this.Key.Value);
			}
			base.Finish();
		}

		// Token: 0x04007E13 RID: 32275
		public FsmString Key;
	}
}
