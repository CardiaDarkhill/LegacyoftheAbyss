using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F08 RID: 3848
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Key is released.")]
	public class GetKeyUp : FsmStateAction
	{
		// Token: 0x06006B9A RID: 27546 RVA: 0x002179B9 File Offset: 0x00215BB9
		public override void Reset()
		{
			this.sendEvent = null;
			this.key = KeyCode.None;
			this.storeResult = null;
		}

		// Token: 0x06006B9B RID: 27547 RVA: 0x002179D0 File Offset: 0x00215BD0
		public override void OnUpdate()
		{
			bool keyUp = Input.GetKeyUp(this.key);
			this.storeResult.Value = keyUp;
			if (keyUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04006AEC RID: 27372
		[RequiredField]
		[Tooltip("The key to detect.")]
		public KeyCode key;

		// Token: 0x04006AED RID: 27373
		[Tooltip("The Event to send when the key is released.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AEE RID: 27374
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable. True if released, otherwise False.")]
		public FsmBool storeResult;
	}
}
