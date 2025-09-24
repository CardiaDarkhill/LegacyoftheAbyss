using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D42 RID: 3394
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataVector3 : FsmStateAction
	{
		// Token: 0x0600639E RID: 25502 RVA: 0x001F6B96 File Offset: 0x001F4D96
		public override void Reset()
		{
			this.gameObject = null;
			this.vector3Name = null;
			this.value = null;
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x001F6BB0 File Offset: 0x001F4DB0
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
			component.SetPlayerDataVector3(this.vector3Name.Value, this.value.Value);
			base.Finish();
		}

		// Token: 0x040061F3 RID: 25075
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061F4 RID: 25076
		[RequiredField]
		public FsmString vector3Name;

		// Token: 0x040061F5 RID: 25077
		[RequiredField]
		public FsmVector3 value;
	}
}
