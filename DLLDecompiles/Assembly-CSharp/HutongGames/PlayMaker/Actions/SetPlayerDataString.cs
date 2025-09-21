using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D41 RID: 3393
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataString : FsmStateAction
	{
		// Token: 0x0600639B RID: 25499 RVA: 0x001F6B11 File Offset: 0x001F4D11
		public override void Reset()
		{
			this.gameObject = null;
			this.stringName = null;
			this.value = null;
		}

		// Token: 0x0600639C RID: 25500 RVA: 0x001F6B28 File Offset: 0x001F4D28
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			GameManager component = ownerDefaultTarget.GetComponent<GameManager>();
			if (component == null)
			{
				Debug.Log("SetPlayerDataInt: could not find a GameManager on this object, please refere to the GameManager global variable");
				return;
			}
			component.SetPlayerDataString(this.stringName.Value, this.value.Value);
			base.Finish();
		}

		// Token: 0x040061F0 RID: 25072
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061F1 RID: 25073
		[RequiredField]
		public FsmString stringName;

		// Token: 0x040061F2 RID: 25074
		[RequiredField]
		public FsmString value;
	}
}
