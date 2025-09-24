using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F05 RID: 3845
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Button is released.")]
	public class GetButtonUp : FsmStateAction
	{
		// Token: 0x06006B8F RID: 27535 RVA: 0x002178A2 File Offset: 0x00215AA2
		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006B90 RID: 27536 RVA: 0x002178C4 File Offset: 0x00215AC4
		public override void OnUpdate()
		{
			bool buttonUp = Input.GetButtonUp(this.buttonName.Value);
			if (buttonUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = buttonUp;
		}

		// Token: 0x04006AE3 RID: 27363
		[RequiredField]
		[Tooltip("The name of the button. Defined in the Unity Input Manager.")]
		public FsmString buttonName;

		// Token: 0x04006AE4 RID: 27364
		[Tooltip("Event to send if the button is released.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AE5 RID: 27365
		[UIHint(UIHint.Variable)]
		[Tooltip("Set to True if the button is released, otherwise False.")]
		public FsmBool storeResult;
	}
}
