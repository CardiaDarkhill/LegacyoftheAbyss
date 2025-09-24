using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001339 RID: 4921
	[ActionCategory("Hollow Knight")]
	[Tooltip("Check and respond to the amount of objects in a Trigger that has TrackTriggerObjects attached to the same object.")]
	public class CheckTrackTriggerCount : FsmStateAction
	{
		// Token: 0x06007F3F RID: 32575 RVA: 0x0025AD68 File Offset: 0x00258F68
		public override void Reset()
		{
			this.target = null;
			this.count = null;
			this.test = null;
			this.everyFrame = true;
			this.successEvent = null;
		}

		// Token: 0x06007F40 RID: 32576 RVA: 0x0025AD8D File Offset: 0x00258F8D
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007F41 RID: 32577 RVA: 0x0025AD9C File Offset: 0x00258F9C
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe)
			{
				this.track = safe.GetComponent<TrackTriggerObjects>();
				if (this.track)
				{
					if (!this.CheckCount())
					{
						if (this.everyFrame)
						{
							return;
						}
					}
					else
					{
						base.Fsm.Event(this.successEvent);
					}
				}
				else
				{
					Debug.LogError("Target GameObject does not have a TrackTriggerObjects component attached!", base.Owner);
				}
			}
			base.Finish();
		}

		// Token: 0x06007F42 RID: 32578 RVA: 0x0025AE11 File Offset: 0x00259011
		public override void OnFixedUpdate()
		{
			if (this.everyFrame && this.CheckCount())
			{
				base.Fsm.Event(this.successEvent);
			}
		}

		// Token: 0x06007F43 RID: 32579 RVA: 0x0025AE34 File Offset: 0x00259034
		public bool CheckCount()
		{
			return this.track && this.track.InsideCount.Test((Extensions.IntTest)this.test.Value, this.count.Value);
		}

		// Token: 0x04007EC6 RID: 32454
		public FsmOwnerDefault target;

		// Token: 0x04007EC7 RID: 32455
		public FsmInt count;

		// Token: 0x04007EC8 RID: 32456
		[ObjectType(typeof(Extensions.IntTest))]
		public FsmEnum test;

		// Token: 0x04007EC9 RID: 32457
		public bool everyFrame;

		// Token: 0x04007ECA RID: 32458
		[Space]
		public FsmEvent successEvent;

		// Token: 0x04007ECB RID: 32459
		private TrackTriggerObjects track;
	}
}
