using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200137C RID: 4988
	[ActionCategory("Hollow Knight")]
	public class QueueMenuStyleUnlockAction : FsmStateAction
	{
		// Token: 0x0600805F RID: 32863 RVA: 0x0025E45E File Offset: 0x0025C65E
		public override void Reset()
		{
			this.unlockKey = null;
		}

		// Token: 0x06008060 RID: 32864 RVA: 0x0025E468 File Offset: 0x0025C668
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.unlockKey.Value))
			{
				GameManager instance = GameManager.instance;
				if (instance)
				{
					instance.QueuedMenuStyleUnlock(this.unlockKey.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FC7 RID: 32711
		public FsmString unlockKey;
	}
}
