using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F00 RID: 3840
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when the user hits any Key or Mouse Button.")]
	public class AnyKey : FsmStateAction
	{
		// Token: 0x06006B7C RID: 27516 RVA: 0x002174B2 File Offset: 0x002156B2
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
		}

		// Token: 0x06006B7D RID: 27517 RVA: 0x002174C2 File Offset: 0x002156C2
		public override void OnUpdate()
		{
			if (ActionHelpers.AnyKeyDown())
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent);
			}
		}

		// Token: 0x04006ACF RID: 27343
		[Tooltip("Where to send the event")]
		public FsmEventTarget eventTarget;

		// Token: 0x04006AD0 RID: 27344
		[RequiredField]
		[Tooltip("Event to send when any Key or Mouse Button is pressed.")]
		public FsmEvent sendEvent;
	}
}
