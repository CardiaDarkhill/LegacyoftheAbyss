using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCD RID: 3277
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class PlayerDataIntAdd : FsmStateAction
	{
		// Token: 0x060061B9 RID: 25017 RVA: 0x001EF19D File Offset: 0x001ED39D
		public override void Reset()
		{
			this.intName = null;
			this.amount = null;
		}

		// Token: 0x060061BA RID: 25018 RVA: 0x001EF1B0 File Offset: 0x001ED3B0
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			instance.IntAdd(this.intName.Value, this.amount.Value);
			base.Finish();
		}

		// Token: 0x04005FED RID: 24557
		[RequiredField]
		public FsmString intName;

		// Token: 0x04005FEE RID: 24558
		[RequiredField]
		public FsmInt amount;
	}
}
