using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B8 RID: 4792
	[ActionCategory("Hollow Knight")]
	public class SetShardDrop : FsmStateAction
	{
		// Token: 0x06007D72 RID: 32114 RVA: 0x0025669F File Offset: 0x0025489F
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.shards = new FsmInt();
		}

		// Token: 0x06007D73 RID: 32115 RVA: 0x002566B8 File Offset: 0x002548B8
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !this.shards.IsNone)
				{
					component.SetShellShards(this.shards.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D6A RID: 32106
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D6B RID: 32107
		public FsmInt shards;
	}
}
