using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C11 RID: 3089
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class DecrementPlayerDataInt : FsmStateAction
	{
		// Token: 0x06005E33 RID: 24115 RVA: 0x001DAEF2 File Offset: 0x001D90F2
		public override void Reset()
		{
			this.gameObject = null;
			this.intName = null;
		}

		// Token: 0x06005E34 RID: 24116 RVA: 0x001DAF04 File Offset: 0x001D9104
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
			component.DecrementPlayerDataInt(this.intName.Value);
			base.Finish();
		}

		// Token: 0x04005A80 RID: 23168
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A81 RID: 23169
		[RequiredField]
		public FsmString intName;
	}
}
