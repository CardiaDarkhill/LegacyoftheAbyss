using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C75 RID: 3189
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataFloat : FsmStateAction
	{
		// Token: 0x0600602D RID: 24621 RVA: 0x001E7536 File Offset: 0x001E5736
		public override void Reset()
		{
			this.gameObject = null;
			this.floatName = null;
			this.storeValue = null;
		}

		// Token: 0x0600602E RID: 24622 RVA: 0x001E7550 File Offset: 0x001E5750
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
			this.storeValue.Value = component.GetPlayerDataFloat(this.floatName.Value);
			base.Finish();
		}

		// Token: 0x04005D86 RID: 23942
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D87 RID: 23943
		[RequiredField]
		public FsmString floatName;

		// Token: 0x04005D88 RID: 23944
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeValue;
	}
}
