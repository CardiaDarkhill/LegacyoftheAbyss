using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D1 RID: 4305
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Delays a State from finishing by a random time. NOTE: Other actions continue running during this time.\nAfter the random time the specified Finish Event or FINISHED is sent.")]
	public class RandomWait : FsmStateAction
	{
		// Token: 0x06007493 RID: 29843 RVA: 0x0023AB9D File Offset: 0x00238D9D
		public override void Reset()
		{
			this.min = 0f;
			this.max = 1f;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06007494 RID: 29844 RVA: 0x0023ABD0 File Offset: 0x00238DD0
		public override void OnEnter()
		{
			this.time = Random.Range(this.min.Value, this.max.Value);
			if (this.time <= 0f)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x06007495 RID: 29845 RVA: 0x0023AC3C File Offset: 0x00238E3C
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
			if (this.timer >= this.time)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x040074CC RID: 29900
		[RequiredField]
		[Tooltip("Minimum amount of time to wait.")]
		public FsmFloat min;

		// Token: 0x040074CD RID: 29901
		[RequiredField]
		[Tooltip("Maximum amount of time to wait.")]
		public FsmFloat max;

		// Token: 0x040074CE RID: 29902
		[Tooltip("Event to send when timer is finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040074CF RID: 29903
		[Tooltip("Ignore time scale.")]
		public bool realTime;

		// Token: 0x040074D0 RID: 29904
		private float startTime;

		// Token: 0x040074D1 RID: 29905
		private float timer;

		// Token: 0x040074D2 RID: 29906
		private float time;
	}
}
