using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils
{
	// Token: 0x02000B13 RID: 2835
	[Serializable]
	public class PlayMakerEventTarget
	{
		// Token: 0x06005939 RID: 22841 RVA: 0x001C455D File Offset: 0x001C275D
		public PlayMakerEventTarget()
		{
		}

		// Token: 0x0600593A RID: 22842 RVA: 0x001C456C File Offset: 0x001C276C
		public PlayMakerEventTarget(bool includeChildren = true)
		{
			this.includeChildren = includeChildren;
		}

		// Token: 0x0600593B RID: 22843 RVA: 0x001C4582 File Offset: 0x001C2782
		public PlayMakerEventTarget(ProxyEventTarget evenTarget, bool includeChildren = true)
		{
			this.eventTarget = evenTarget;
			this.includeChildren = includeChildren;
		}

		// Token: 0x04005491 RID: 21649
		public ProxyEventTarget eventTarget;

		// Token: 0x04005492 RID: 21650
		public GameObject gameObject;

		// Token: 0x04005493 RID: 21651
		public bool includeChildren = true;

		// Token: 0x04005494 RID: 21652
		public PlayMakerFSM fsmComponent;
	}
}
