using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200126A RID: 4714
	public sealed class CanAddBlueHealth : FsmStateAction
	{
		// Token: 0x06007C59 RID: 31833 RVA: 0x00252E7B File Offset: 0x0025107B
		public override void Reset()
		{
			this.CanAddEvent = null;
			this.CannotAddEvent = null;
			this.Consume = true;
		}

		// Token: 0x06007C5A RID: 31834 RVA: 0x00252E98 File Offset: 0x00251098
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				if (instance.QueuedBlueHealth > 0)
				{
					base.Fsm.Event(this.CanAddEvent);
					if (this.Consume.Value)
					{
						GameManager gameManager = instance;
						int queuedBlueHealth = gameManager.QueuedBlueHealth;
						gameManager.QueuedBlueHealth = queuedBlueHealth - 1;
					}
				}
				else
				{
					base.Fsm.Event(this.CannotAddEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007C69 RID: 31849
		public FsmEvent CanAddEvent;

		// Token: 0x04007C6A RID: 31850
		public FsmEvent CannotAddEvent;

		// Token: 0x04007C6B RID: 31851
		public FsmBool Consume;
	}
}
