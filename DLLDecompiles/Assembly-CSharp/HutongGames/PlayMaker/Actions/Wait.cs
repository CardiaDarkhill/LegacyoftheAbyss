using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D3 RID: 4307
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Delays a State from finishing. Optionally send an event after the specified time. NOTE: Other actions continue running and can send events before this action finishes.")]
	public class Wait : FsmStateAction
	{
		// Token: 0x0600749C RID: 29852 RVA: 0x0023AD26 File Offset: 0x00238F26
		public override void Reset()
		{
			this.time = 1f;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x0600749D RID: 29853 RVA: 0x0023AD48 File Offset: 0x00238F48
		public override void OnEnter()
		{
			if (this.time.Value <= 0f)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x0600749E RID: 29854 RVA: 0x0023AD98 File Offset: 0x00238F98
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer >= this.time.Value)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x040074D6 RID: 29910
		[RequiredField]
		[Tooltip("Time to wait in seconds.")]
		public FsmFloat time;

		// Token: 0x040074D7 RID: 29911
		[Tooltip("Event to send after the specified time.")]
		public FsmEvent finishEvent;

		// Token: 0x040074D8 RID: 29912
		[Tooltip("Ignore TimeScale. E.g., if the game is paused using Scale Time.")]
		public bool realTime;

		// Token: 0x040074D9 RID: 29913
		private float startTime;

		// Token: 0x040074DA RID: 29914
		private float timer;
	}
}
