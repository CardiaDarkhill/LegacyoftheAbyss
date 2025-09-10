using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C76 RID: 3190
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataInt : FsmStateAction
	{
		// Token: 0x06006030 RID: 24624 RVA: 0x001E75B4 File Offset: 0x001E57B4
		public override void Reset()
		{
			this.intName = null;
			this.storeValue = null;
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x001E75C4 File Offset: 0x001E57C4
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			this.storeValue.Value = instance.GetPlayerDataInt(this.intName.Value);
			base.Finish();
		}

		// Token: 0x04005D89 RID: 23945
		[RequiredField]
		public FsmString intName;

		// Token: 0x04005D8A RID: 23946
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;
	}
}
