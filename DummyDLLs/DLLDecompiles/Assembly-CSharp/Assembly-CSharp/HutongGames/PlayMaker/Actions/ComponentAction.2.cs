using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E66 RID: 3686
	public abstract class ComponentAction<T1, T2> : FsmStateAction where T1 : Component where T2 : Component
	{
		// Token: 0x06006933 RID: 26931 RVA: 0x002101AC File Offset: 0x0020E3AC
		protected bool UpdateCache(GameObject go1, GameObject go2)
		{
			if (go1 == null || go2 == null)
			{
				return false;
			}
			if (this.cachedComponent1 == null || this.cachedGameObject1 != go1)
			{
				this.cachedComponent1 = go1.GetComponent<T1>();
				this.cachedGameObject1 = go1;
				if (this.cachedComponent1 == null)
				{
					return false;
				}
			}
			if (this.cachedComponent2 == null || this.cachedGameObject2 != go2)
			{
				this.cachedComponent2 = go2.GetComponent<T2>();
				this.cachedGameObject2 = go2;
				if (this.cachedComponent2 == null)
				{
					return false;
				}
			}
			this.cachedTransform2 = this.cachedGameObject2.transform;
			return true;
		}

		// Token: 0x04006878 RID: 26744
		protected GameObject cachedGameObject1;

		// Token: 0x04006879 RID: 26745
		protected GameObject cachedGameObject2;

		// Token: 0x0400687A RID: 26746
		protected T1 cachedComponent1;

		// Token: 0x0400687B RID: 26747
		protected T2 cachedComponent2;

		// Token: 0x0400687C RID: 26748
		protected Transform cachedTransform2;
	}
}
