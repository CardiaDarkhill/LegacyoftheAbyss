using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001310 RID: 4880
	public class SetNeedolinTextCollection : FsmStateAction
	{
		// Token: 0x06007EBC RID: 32444 RVA: 0x00259A72 File Offset: 0x00257C72
		public override void Reset()
		{
			this.Target = null;
			this.Collection = null;
		}

		// Token: 0x06007EBD RID: 32445 RVA: 0x00259A84 File Offset: 0x00257C84
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				NeedolinTextOwner component = safe.GetComponent<NeedolinTextOwner>();
				if (component)
				{
					component.SetTextCollection(this.Collection.Value as LocalisedTextCollection);
				}
			}
			base.Finish();
		}

		// Token: 0x04007E68 RID: 32360
		[RequiredField]
		[CheckForComponent(typeof(NeedolinTextOwner))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E69 RID: 32361
		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Collection;
	}
}
