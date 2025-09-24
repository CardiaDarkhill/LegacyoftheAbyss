using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3E RID: 3390
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataBool : FsmStateAction
	{
		// Token: 0x06006392 RID: 25490 RVA: 0x001F6998 File Offset: 0x001F4B98
		public override void Reset()
		{
			this.boolName = null;
			this.value = null;
		}

		// Token: 0x06006393 RID: 25491 RVA: 0x001F69A8 File Offset: 0x001F4BA8
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				Debug.Log("GameManager could not be found");
				base.Finish();
				return;
			}
			if (!string.IsNullOrEmpty(this.boolName.Value))
			{
				instance.SetPlayerDataBool(this.boolName.Value, this.value.Value);
			}
			base.Finish();
		}

		// Token: 0x040061E9 RID: 25065
		[RequiredField]
		public FsmString boolName;

		// Token: 0x040061EA RID: 25066
		[RequiredField]
		public FsmBool value;
	}
}
