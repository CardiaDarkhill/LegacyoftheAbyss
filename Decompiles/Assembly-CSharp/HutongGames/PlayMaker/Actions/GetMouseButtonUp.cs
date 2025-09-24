using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F0B RID: 3851
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when the specified Mouse Button is released. Optionally store the button state in a bool variable.")]
	public class GetMouseButtonUp : FsmStateAction
	{
		// Token: 0x06006BA7 RID: 27559 RVA: 0x00217ADD File Offset: 0x00215CDD
		public override void Reset()
		{
			this.button = MouseButton.Left;
			this.sendEvent = null;
			this.storeResult = null;
			this.inUpdateOnly = true;
		}

		// Token: 0x06006BA8 RID: 27560 RVA: 0x00217AFB File Offset: 0x00215CFB
		public override void OnEnter()
		{
			if (!this.inUpdateOnly)
			{
				this.DoGetMouseButtonUp();
			}
		}

		// Token: 0x06006BA9 RID: 27561 RVA: 0x00217B0B File Offset: 0x00215D0B
		public override void OnUpdate()
		{
			this.DoGetMouseButtonUp();
		}

		// Token: 0x06006BAA RID: 27562 RVA: 0x00217B14 File Offset: 0x00215D14
		public void DoGetMouseButtonUp()
		{
			bool mouseButtonUp = Input.GetMouseButtonUp((int)this.button);
			this.storeResult.Value = mouseButtonUp;
			if (mouseButtonUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04006AF6 RID: 27382
		[RequiredField]
		[Tooltip("The mouse button to test.")]
		public MouseButton button;

		// Token: 0x04006AF7 RID: 27383
		[Tooltip("Event to send if the mouse button is down.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AF8 RID: 27384
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the pressed state in a Bool Variable.")]
		public FsmBool storeResult;

		// Token: 0x04006AF9 RID: 27385
		[Tooltip("Uncheck to run when entering the state.")]
		public bool inUpdateOnly;
	}
}
