using System;
using UnityEngine;
using UnityEngine.Playables;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDA RID: 3290
	public class PlayTimeline : FsmStateAction
	{
		// Token: 0x060061F7 RID: 25079 RVA: 0x001EFA4C File Offset: 0x001EDC4C
		public override void Reset()
		{
			this.Target = null;
			this.FinishEvent = null;
		}

		// Token: 0x060061F8 RID: 25080 RVA: 0x001EFA5C File Offset: 0x001EDC5C
		public override void OnEnter()
		{
			this.timeline = null;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.timeline = safe.GetComponent<PlayableDirector>();
				this.timeline.time = 0.0;
				this.timeline.Play();
				return;
			}
			base.Finish();
		}

		// Token: 0x060061F9 RID: 25081 RVA: 0x001EFAB7 File Offset: 0x001EDCB7
		public override void OnUpdate()
		{
			if (this.timeline.time >= this.timeline.duration)
			{
				this.End();
			}
		}

		// Token: 0x060061FA RID: 25082 RVA: 0x001EFAD7 File Offset: 0x001EDCD7
		private void End()
		{
			base.Fsm.Event(this.FinishEvent);
			base.Finish();
		}

		// Token: 0x0400600F RID: 24591
		public FsmOwnerDefault Target;

		// Token: 0x04006010 RID: 24592
		public FsmEvent FinishEvent;

		// Token: 0x04006011 RID: 24593
		private PlayableDirector timeline;
	}
}
