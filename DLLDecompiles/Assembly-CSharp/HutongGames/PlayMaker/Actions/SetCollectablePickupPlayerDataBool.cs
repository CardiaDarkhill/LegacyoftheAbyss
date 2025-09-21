using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001283 RID: 4739
	public class SetCollectablePickupPlayerDataBool : FsmStateAction
	{
		// Token: 0x06007CAD RID: 31917 RVA: 0x00254210 File Offset: 0x00252410
		public override void Reset()
		{
			this.Target = null;
			this.PlayerDataBoolName = null;
		}

		// Token: 0x06007CAE RID: 31918 RVA: 0x00254220 File Offset: 0x00252420
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				CollectableItemPickup component = safe.GetComponent<CollectableItemPickup>();
				if (component)
				{
					component.SetPlayerDataBool(this.PlayerDataBoolName.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007CC8 RID: 31944
		public FsmOwnerDefault Target;

		// Token: 0x04007CC9 RID: 31945
		public FsmString PlayerDataBoolName;
	}
}
