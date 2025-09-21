using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F04 RID: 3844
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Button is pressed.")]
	public class GetButtonDown : FsmStateAction
	{
		// Token: 0x06006B8C RID: 27532 RVA: 0x00217839 File Offset: 0x00215A39
		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006B8D RID: 27533 RVA: 0x0021785C File Offset: 0x00215A5C
		public override void OnUpdate()
		{
			bool buttonDown = Input.GetButtonDown(this.buttonName.Value);
			this.storeResult.Value = buttonDown;
			if (buttonDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04006AE0 RID: 27360
		[RequiredField]
		[Tooltip("The name of the button. Defined in the Unity Input Manager.")]
		public FsmString buttonName;

		// Token: 0x04006AE1 RID: 27361
		[Tooltip("Event to send if the button is pressed.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AE2 RID: 27362
		[Tooltip("Set to True if the button is pressed, otherwise False.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;
	}
}
