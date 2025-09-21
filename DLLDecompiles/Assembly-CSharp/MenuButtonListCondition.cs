using System;
using UnityEngine;

// Token: 0x020006E3 RID: 1763
public abstract class MenuButtonListCondition : MonoBehaviour
{
	// Token: 0x06003F83 RID: 16259
	public abstract bool IsFulfilled();

	// Token: 0x06003F84 RID: 16260 RVA: 0x00118510 File Offset: 0x00116710
	public bool IsFulfilledAllComponents()
	{
		if (!this.IsFulfilled())
		{
			return false;
		}
		if (this.components == null)
		{
			this.components = base.GetComponents<MenuButtonListCondition>();
		}
		MenuButtonListCondition[] array = this.components;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsFulfilled())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003F85 RID: 16261 RVA: 0x0011855D File Offset: 0x0011675D
	public virtual bool AlwaysVisible()
	{
		return false;
	}

	// Token: 0x06003F86 RID: 16262 RVA: 0x00118560 File Offset: 0x00116760
	public virtual void OnActiveStateSet(bool isActive)
	{
	}

	// Token: 0x0400413C RID: 16700
	private MenuButtonListCondition[] components;
}
