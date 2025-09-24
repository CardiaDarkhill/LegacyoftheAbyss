using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D40 RID: 3392
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataInt : FsmStateAction
	{
		// Token: 0x06006398 RID: 25496 RVA: 0x001F6AAD File Offset: 0x001F4CAD
		public override void Reset()
		{
			this.intName = null;
			this.value = null;
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x001F6AC0 File Offset: 0x001F4CC0
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				Debug.Log("GameManager could not be found");
				return;
			}
			instance.SetPlayerDataInt(this.intName.Value, this.value.Value);
			base.Finish();
		}

		// Token: 0x040061EE RID: 25070
		[RequiredField]
		public FsmString intName;

		// Token: 0x040061EF RID: 25071
		[RequiredField]
		public FsmInt value;
	}
}
