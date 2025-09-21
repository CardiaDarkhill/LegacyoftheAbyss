using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C77 RID: 3191
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataString : FsmStateAction
	{
		// Token: 0x06006033 RID: 24627 RVA: 0x001E760B File Offset: 0x001E580B
		public override void Reset()
		{
			this.gameObject = null;
			this.stringName = null;
			this.storeValue = null;
		}

		// Token: 0x06006034 RID: 24628 RVA: 0x001E7624 File Offset: 0x001E5824
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
				return;
			}
			this.storeValue.Value = component.GetPlayerDataString(this.stringName.Value);
			base.Finish();
		}

		// Token: 0x04005D8B RID: 23947
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D8C RID: 23948
		[RequiredField]
		public FsmString stringName;

		// Token: 0x04005D8D RID: 23949
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString storeValue;
	}
}
