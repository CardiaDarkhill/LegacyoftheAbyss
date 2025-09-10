using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D6 RID: 470
public class TagDamager : MonoBehaviour
{
	// Token: 0x06001270 RID: 4720 RVA: 0x00055CC4 File Offset: 0x00053EC4
	private void OnDisable()
	{
		foreach (TagDamageTaker tagDamageTaker in this.addedTo)
		{
			tagDamageTaker.RemoveDamageTagFromStack(this.damageTag, false);
		}
		this.addedTo.Clear();
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00055D28 File Offset: 0x00053F28
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!this.damageTag)
		{
			return;
		}
		HealthManager componentInParent = other.GetComponentInParent<HealthManager>();
		if (!componentInParent)
		{
			return;
		}
		TagDamageTaker component = componentInParent.GetComponent<TagDamageTaker>();
		if (component == null)
		{
			return;
		}
		if (this.hitAmountOverride <= 0)
		{
			this.addedTo.Add(component);
		}
		component.AddDamageTagToStack(this.damageTag, this.hitAmountOverride);
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00055D8C File Offset: 0x00053F8C
	private void OnTriggerExit2D(Collider2D other)
	{
		if (!this.damageTag)
		{
			return;
		}
		HealthManager componentInParent = other.GetComponentInParent<HealthManager>();
		if (!componentInParent)
		{
			return;
		}
		TagDamageTaker component = componentInParent.GetComponent<TagDamageTaker>();
		if (this.addedTo.Remove(component))
		{
			component.RemoveDamageTagFromStack(this.damageTag, false);
		}
	}

	// Token: 0x04001133 RID: 4403
	[SerializeField]
	[AssetPickerDropdown]
	private DamageTag damageTag;

	// Token: 0x04001134 RID: 4404
	[SerializeField]
	private int hitAmountOverride;

	// Token: 0x04001135 RID: 4405
	private readonly List<TagDamageTaker> addedTo = new List<TagDamageTaker>();
}
