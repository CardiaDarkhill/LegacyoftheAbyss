using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001384 RID: 4996
	public abstract class ShopCheck : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x0600807B RID: 32891 RVA: 0x0025E7FF File Offset: 0x0025C9FF
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
		}

		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x0600807C RID: 32892 RVA: 0x0025E810 File Offset: 0x0025CA10
		public override bool IsTrue
		{
			get
			{
				GameObject gameObject = this.Target.GetSafe(this);
				if (!gameObject)
				{
					return false;
				}
				ShopOwnerBase component = gameObject.GetComponent<ShopOwnerBase>();
				if (component)
				{
					gameObject = component.ShopObject;
				}
				ShopMenuStock component2 = gameObject.GetComponent<ShopMenuStock>();
				return component2 && this.CheckShop(component2);
			}
		}

		// Token: 0x0600807D RID: 32893
		protected abstract bool CheckShop(ShopMenuStock shop);

		// Token: 0x04007FD6 RID: 32726
		public FsmOwnerDefault Target;
	}
}
