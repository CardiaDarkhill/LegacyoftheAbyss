using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200133B RID: 4923
	public class GetTrackTriggerCount : FsmStateAction
	{
		// Token: 0x06007F4C RID: 32588 RVA: 0x0025AF9E File Offset: 0x0025919E
		public override void Reset()
		{
			this.Target = null;
			this.StoreCount = null;
			this.EveryFrame = true;
		}

		// Token: 0x06007F4D RID: 32589 RVA: 0x0025AFB5 File Offset: 0x002591B5
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007F4E RID: 32590 RVA: 0x0025AFC4 File Offset: 0x002591C4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.track = safe.GetComponent<TrackTriggerObjects>();
				if (this.track)
				{
					this.StoreCount.Value = this.track.InsideCount;
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

		// Token: 0x06007F4F RID: 32591 RVA: 0x0025B034 File Offset: 0x00259234
		public override void OnFixedUpdate()
		{
			this.StoreCount.Value = this.track.InsideCount;
		}

		// Token: 0x04007ED4 RID: 32468
		public FsmOwnerDefault Target;

		// Token: 0x04007ED5 RID: 32469
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;

		// Token: 0x04007ED6 RID: 32470
		public bool EveryFrame;

		// Token: 0x04007ED7 RID: 32471
		private TrackTriggerObjects track;
	}
}
