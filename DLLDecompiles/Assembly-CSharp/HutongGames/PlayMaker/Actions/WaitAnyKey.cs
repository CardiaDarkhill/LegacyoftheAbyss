using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F15 RID: 3861
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Waits until any key is pressed then action finishes. Similar to AnyKey action but can be used in Action Sequences.")]
	public class WaitAnyKey : FsmStateAction
	{
		// Token: 0x06006BDD RID: 27613 RVA: 0x00218988 File Offset: 0x00216B88
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
		}

		// Token: 0x06006BDE RID: 27614 RVA: 0x00218998 File Offset: 0x00216B98
		public override void OnUpdate()
		{
			if (ActionHelpers.AnyKeyDown())
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent);
				base.Finish();
			}
		}

		// Token: 0x04006B3C RID: 27452
		[Tooltip("Where to send the optional event")]
		public FsmEventTarget eventTarget;

		// Token: 0x04006B3D RID: 27453
		[Tooltip("Optional event to send when any Key or Mouse Button is pressed.")]
		public FsmEvent sendEvent;
	}
}
