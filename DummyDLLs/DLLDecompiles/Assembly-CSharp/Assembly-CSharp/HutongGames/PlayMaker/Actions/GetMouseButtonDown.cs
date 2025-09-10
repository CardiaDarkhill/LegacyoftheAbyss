using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F0A RID: 3850
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
	public class GetMouseButtonDown : FsmStateAction
	{
		// Token: 0x06006BA2 RID: 27554 RVA: 0x00217A66 File Offset: 0x00215C66
		public override void Reset()
		{
			this.button = MouseButton.Left;
			this.sendEvent = null;
			this.storeResult = null;
			this.inUpdateOnly = true;
		}

		// Token: 0x06006BA3 RID: 27555 RVA: 0x00217A84 File Offset: 0x00215C84
		public override void OnEnter()
		{
			if (!this.inUpdateOnly)
			{
				this.DoGetMouseButtonDown();
			}
		}

		// Token: 0x06006BA4 RID: 27556 RVA: 0x00217A94 File Offset: 0x00215C94
		public override void OnUpdate()
		{
			this.DoGetMouseButtonDown();
		}

		// Token: 0x06006BA5 RID: 27557 RVA: 0x00217A9C File Offset: 0x00215C9C
		private void DoGetMouseButtonDown()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown((int)this.button);
			this.storeResult.Value = mouseButtonDown;
			if (mouseButtonDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04006AF2 RID: 27378
		[RequiredField]
		[Tooltip("The mouse button to test.")]
		public MouseButton button;

		// Token: 0x04006AF3 RID: 27379
		[Tooltip("Event to send if the mouse button is down.")]
		public FsmEvent sendEvent;

		// Token: 0x04006AF4 RID: 27380
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the button state in a Bool Variable.")]
		public FsmBool storeResult;

		// Token: 0x04006AF5 RID: 27381
		[Tooltip("Uncheck to run when entering the state.")]
		public bool inUpdateOnly;
	}
}
