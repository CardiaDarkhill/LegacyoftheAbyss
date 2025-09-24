using System;
using UnityEngine;
using UnityEngine.Playables;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDC RID: 3292
	public class ResumeTimeline : FsmStateAction
	{
		// Token: 0x060061FF RID: 25087 RVA: 0x001EFB3F File Offset: 0x001EDD3F
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06006200 RID: 25088 RVA: 0x001EFB48 File Offset: 0x001EDD48
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				safe.GetComponent<PlayableDirector>().Resume();
			}
			base.Finish();
		}

		// Token: 0x04006013 RID: 24595
		public FsmOwnerDefault Target;
	}
}
