using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001351 RID: 4945
	public class RemoveMemoryMsg : FsmStateAction
	{
		// Token: 0x06007FB9 RID: 32697 RVA: 0x0025C5AD File Offset: 0x0025A7AD
		public override void Reset()
		{
			this.Source = null;
			this.DisappearDelay = null;
		}

		// Token: 0x06007FBA RID: 32698 RVA: 0x0025C5C0 File Offset: 0x0025A7C0
		public override void OnEnter()
		{
			GameObject safe = this.Source.GetSafe(this);
			if (safe)
			{
				MemoryMsgBox.RemoveText(safe, this.DisappearDelay.Value);
			}
			base.Finish();
		}

		// Token: 0x04007F3D RID: 32573
		public FsmOwnerDefault Source;

		// Token: 0x04007F3E RID: 32574
		public FsmFloat DisappearDelay;
	}
}
