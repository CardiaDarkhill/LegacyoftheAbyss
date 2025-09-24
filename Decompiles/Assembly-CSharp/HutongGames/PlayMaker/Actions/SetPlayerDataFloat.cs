using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3F RID: 3391
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataFloat : FsmStateAction
	{
		// Token: 0x06006395 RID: 25493 RVA: 0x001F6A11 File Offset: 0x001F4C11
		public override void Reset()
		{
			this.gameObject = null;
			this.floatName = null;
			this.value = null;
		}

		// Token: 0x06006396 RID: 25494 RVA: 0x001F6A28 File Offset: 0x001F4C28
		public override void OnEnter()
		{
			if (VariableExtensions.VariableExists<float, PlayerData>(this.floatName.Value))
			{
				PlayerData.instance.SetVariable(this.floatName.Value, this.value.Value);
			}
			else
			{
				Debug.Log(string.Format("PlayerData float {0} does not exist. (FSM: {1}, State: {2})", this.floatName.Value, base.Fsm.Name, base.State.Name), base.Owner);
			}
			base.Finish();
		}

		// Token: 0x040061EB RID: 25067
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061EC RID: 25068
		[RequiredField]
		public FsmString floatName;

		// Token: 0x040061ED RID: 25069
		[RequiredField]
		public FsmFloat value;
	}
}
