using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E91 RID: 3729
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends events based on Touch Phases. Optionally filter by a fingerID.")]
	public class TouchEvent : FsmStateAction
	{
		// Token: 0x060069E8 RID: 27112 RVA: 0x00211E79 File Offset: 0x00210079
		public override void Reset()
		{
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.storeFingerId = null;
		}

		// Token: 0x060069E9 RID: 27113 RVA: 0x00211E94 File Offset: 0x00210094
		public override void OnUpdate()
		{
			if (Input.touchCount > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					if ((this.fingerId.IsNone || touch.fingerId == this.fingerId.Value) && touch.phase == this.touchPhase)
					{
						this.storeFingerId.Value = touch.fingerId;
						base.Fsm.Event(this.sendEvent);
					}
				}
			}
		}

		// Token: 0x04006925 RID: 26917
		[Tooltip("An optional Finger Id to filter by. For example, if you detected a Touch Began and stored the FingerId, you could look for the Ended event for that Finger Id.")]
		public FsmInt fingerId;

		// Token: 0x04006926 RID: 26918
		[Tooltip("The phase you're interested in detecting (Began, Moved, Stationary, Ended, Cancelled).")]
		public TouchPhase touchPhase;

		// Token: 0x04006927 RID: 26919
		[Tooltip("The event to send when the Touch Phase is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04006928 RID: 26920
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Finger Id associated with the touch event for later use.")]
		public FsmInt storeFingerId;
	}
}
