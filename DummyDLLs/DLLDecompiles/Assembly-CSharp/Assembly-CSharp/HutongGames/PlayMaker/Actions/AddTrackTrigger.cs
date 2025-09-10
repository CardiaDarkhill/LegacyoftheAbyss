using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001338 RID: 4920
	[ActionCategory("Hollow Knight")]
	public class AddTrackTrigger : FsmStateAction
	{
		// Token: 0x06007F3C RID: 32572 RVA: 0x0025AD1A File Offset: 0x00258F1A
		public override void Reset()
		{
			this.target = null;
		}

		// Token: 0x06007F3D RID: 32573 RVA: 0x0025AD24 File Offset: 0x00258F24
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe && !safe.GetComponent<TrackTriggerObjects>())
			{
				safe.AddComponent<TrackTriggerObjects>();
			}
			base.Finish();
		}

		// Token: 0x04007EC5 RID: 32453
		public FsmOwnerDefault target;
	}
}
