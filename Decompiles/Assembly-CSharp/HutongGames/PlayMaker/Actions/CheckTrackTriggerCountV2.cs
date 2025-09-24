using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200133A RID: 4922
	[ActionCategory("Hollow Knight")]
	public class CheckTrackTriggerCountV2 : FsmStateAction
	{
		// Token: 0x06007F45 RID: 32581 RVA: 0x0025AE78 File Offset: 0x00259078
		public override void Reset()
		{
			this.Target = null;
			this.Count = null;
			this.Test = null;
			this.EveryFrame = true;
			this.SetBool = null;
			this.SuccessEvent = null;
			this.FailEvent = null;
		}

		// Token: 0x06007F46 RID: 32582 RVA: 0x0025AEAB File Offset: 0x002590AB
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007F47 RID: 32583 RVA: 0x0025AEBC File Offset: 0x002590BC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.track = safe.GetComponent<TrackTriggerObjects>();
				if (this.track)
				{
					this.UpdateState(this.CheckCount());
					if (this.EveryFrame)
					{
						return;
					}
				}
				else
				{
					Debug.LogError("Target GameObject does not have a TrackTriggerObjects component attached!", base.Owner);
				}
			}
			base.Finish();
		}

		// Token: 0x06007F48 RID: 32584 RVA: 0x0025AF22 File Offset: 0x00259122
		public override void OnFixedUpdate()
		{
			this.UpdateState(this.CheckCount());
		}

		// Token: 0x06007F49 RID: 32585 RVA: 0x0025AF30 File Offset: 0x00259130
		public bool CheckCount()
		{
			return this.track && this.track.InsideCount.Test((Extensions.IntTest)this.Test.Value, this.Count.Value);
		}

		// Token: 0x06007F4A RID: 32586 RVA: 0x0025AF6C File Offset: 0x0025916C
		private void UpdateState(bool value)
		{
			base.Fsm.Event(value ? this.SuccessEvent : this.FailEvent);
			this.SetBool.Value = value;
		}

		// Token: 0x04007ECC RID: 32460
		public FsmOwnerDefault Target;

		// Token: 0x04007ECD RID: 32461
		public FsmInt Count;

		// Token: 0x04007ECE RID: 32462
		[ObjectType(typeof(Extensions.IntTest))]
		public FsmEnum Test;

		// Token: 0x04007ECF RID: 32463
		public bool EveryFrame;

		// Token: 0x04007ED0 RID: 32464
		[Space]
		[UIHint(UIHint.Variable)]
		public FsmBool SetBool;

		// Token: 0x04007ED1 RID: 32465
		public FsmEvent SuccessEvent;

		// Token: 0x04007ED2 RID: 32466
		public FsmEvent FailEvent;

		// Token: 0x04007ED3 RID: 32467
		private TrackTriggerObjects track;
	}
}
