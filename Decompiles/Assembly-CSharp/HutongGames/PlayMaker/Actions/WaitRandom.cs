using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA6 RID: 3494
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Delays a State from finishing by the specified time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
	public class WaitRandom : FsmStateAction
	{
		// Token: 0x06006574 RID: 25972 RVA: 0x00200301 File Offset: 0x001FE501
		public override void Reset()
		{
			this.timeMin = 0f;
			this.timeMax = 1f;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006575 RID: 25973 RVA: 0x00200334 File Offset: 0x001FE534
		public override void OnEnter()
		{
			this.time = Random.Range(this.timeMin.Value, this.timeMax.Value);
			if (this.time <= 0f)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x06006576 RID: 25974 RVA: 0x002003A0 File Offset: 0x001FE5A0
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

		// Token: 0x04006476 RID: 25718
		[RequiredField]
		public FsmFloat timeMin;

		// Token: 0x04006477 RID: 25719
		public FsmFloat timeMax;

		// Token: 0x04006478 RID: 25720
		public FsmEvent finishEvent;

		// Token: 0x04006479 RID: 25721
		public bool realTime;

		// Token: 0x0400647A RID: 25722
		private float time;

		// Token: 0x0400647B RID: 25723
		private float startTime;

		// Token: 0x0400647C RID: 25724
		private float timer;
	}
}
