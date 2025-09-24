using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200126F RID: 4719
	public class SetHeroNailImbuement : FsmStateAction
	{
		// Token: 0x06007C68 RID: 31848 RVA: 0x0025304A File Offset: 0x0025124A
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007C69 RID: 31849 RVA: 0x00253054 File Offset: 0x00251254
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				HeroNailImbuement component = safe.GetComponent<HeroNailImbuement>();
				if (component)
				{
					component.SetElement((NailElements)this.Element.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007C6F RID: 31855
		public FsmOwnerDefault Target;

		// Token: 0x04007C70 RID: 31856
		[ObjectType(typeof(NailElements))]
		public FsmEnum Element;
	}
}
