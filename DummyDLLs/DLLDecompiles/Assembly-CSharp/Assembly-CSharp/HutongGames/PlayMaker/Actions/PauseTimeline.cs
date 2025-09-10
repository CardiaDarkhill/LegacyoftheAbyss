using System;
using UnityEngine;
using UnityEngine.Playables;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDB RID: 3291
	public class PauseTimeline : FsmStateAction
	{
		// Token: 0x060061FC RID: 25084 RVA: 0x001EFAF8 File Offset: 0x001EDCF8
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x060061FD RID: 25085 RVA: 0x001EFB04 File Offset: 0x001EDD04
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				safe.GetComponent<PlayableDirector>().Pause();
			}
			base.Finish();
		}

		// Token: 0x04006012 RID: 24594
		public FsmOwnerDefault Target;
	}
}
