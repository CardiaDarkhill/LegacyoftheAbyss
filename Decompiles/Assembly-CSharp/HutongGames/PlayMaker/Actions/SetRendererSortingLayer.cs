using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4C RID: 3404
	public class SetRendererSortingLayer : FsmStateAction
	{
		// Token: 0x060063CD RID: 25549 RVA: 0x001F76AE File Offset: 0x001F58AE
		public override void Reset()
		{
			this.Target = null;
			this.SortingLayerName = null;
			this.SortingOrder = null;
		}

		// Token: 0x060063CE RID: 25550 RVA: 0x001F76C8 File Offset: 0x001F58C8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				Renderer component = safe.GetComponent<Renderer>();
				if (component)
				{
					component.sortingLayerName = this.SortingLayerName.Value;
					component.sortingOrder = this.SortingOrder.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04006223 RID: 25123
		public FsmOwnerDefault Target;

		// Token: 0x04006224 RID: 25124
		public FsmString SortingLayerName;

		// Token: 0x04006225 RID: 25125
		public FsmInt SortingOrder;
	}
}
