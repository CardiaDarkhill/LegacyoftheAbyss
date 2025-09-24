using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8B RID: 3211
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class IncrementPlayerDataInt : FsmStateAction
	{
		// Token: 0x06006092 RID: 24722 RVA: 0x001EA0BB File Offset: 0x001E82BB
		public override void Reset()
		{
			this.intName = null;
		}

		// Token: 0x06006093 RID: 24723 RVA: 0x001EA0C4 File Offset: 0x001E82C4
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			instance.IncrementPlayerDataInt(this.intName.Value);
			base.Finish();
		}

		// Token: 0x04005E14 RID: 24084
		[RequiredField]
		public FsmString intName;
	}
}
