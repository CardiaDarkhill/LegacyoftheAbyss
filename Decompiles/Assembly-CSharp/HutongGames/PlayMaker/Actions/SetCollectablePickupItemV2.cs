using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001282 RID: 4738
	public class SetCollectablePickupItemV2 : FsmStateAction
	{
		// Token: 0x06007CAA RID: 31914 RVA: 0x00254196 File Offset: 0x00252396
		public override void Reset()
		{
			this.Target = null;
			this.Item = null;
			this.KeepPersistence = null;
		}

		// Token: 0x06007CAB RID: 31915 RVA: 0x002541B0 File Offset: 0x002523B0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				CollectableItemPickup component = safe.GetComponent<CollectableItemPickup>();
				if (component)
				{
					component.SetItem(this.Item.Value as SavedItem, this.KeepPersistence.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007CC5 RID: 31941
		public FsmOwnerDefault Target;

		// Token: 0x04007CC6 RID: 31942
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		// Token: 0x04007CC7 RID: 31943
		public FsmBool KeepPersistence;
	}
}
