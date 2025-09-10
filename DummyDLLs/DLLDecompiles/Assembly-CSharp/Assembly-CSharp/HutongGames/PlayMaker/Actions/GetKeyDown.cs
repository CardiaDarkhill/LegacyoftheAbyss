using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F07 RID: 3847
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Key is pressed.")]
	public class GetKeyDown : FsmStateAction
	{
		// Token: 0x06006B97 RID: 27543 RVA: 0x0021795F File Offset: 0x00215B5F
		public override void Reset()
		{
			this.sendEvent = null;
			this.key = KeyCode.None;
			this.storeResult = null;
		}

		// Token: 0x06006B98 RID: 27544 RVA: 0x00217978 File Offset: 0x00215B78
		public override void OnUpdate()
		{
			bool keyDown = Input.GetKeyDown(this.key);
			this.storeResult.Value = keyDown;
			if (keyDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04006AE9 RID: 27369
		[RequiredField]
		[Tooltip("The key to detect.")]
		public KeyCode key;

		// Token: 0x04006AEA RID: 27370
		[Tooltip("The Event to send when the key is pressed.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AEB RID: 27371
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable. True if pressed, otherwise False.")]
		public FsmBool storeResult;
	}
}
