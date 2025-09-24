using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C73 RID: 3187
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataBool : FsmStateAction
	{
		// Token: 0x06006027 RID: 24615 RVA: 0x001E73DB File Offset: 0x001E55DB
		public override void Reset()
		{
			this.boolName = null;
			this.storeValue = null;
		}

		// Token: 0x06006028 RID: 24616 RVA: 0x001E73EC File Offset: 0x001E55EC
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			this.storeValue.Value = instance.GetPlayerDataBool(this.boolName.Value);
			base.Finish();
		}

		// Token: 0x04005D80 RID: 23936
		[RequiredField]
		public FsmString boolName;

		// Token: 0x04005D81 RID: 23937
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;
	}
}
