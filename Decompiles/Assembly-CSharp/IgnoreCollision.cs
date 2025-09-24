using System;
using UnityEngine;

// Token: 0x0200050B RID: 1291
public class IgnoreCollision : MonoBehaviour
{
	// Token: 0x06002E21 RID: 11809 RVA: 0x000CA8C6 File Offset: 0x000C8AC6
	private void Reset()
	{
		this.obj1 = base.gameObject;
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000CA8D4 File Offset: 0x000C8AD4
	private void Awake()
	{
		if (this.getParent)
		{
			Transform parent = base.transform.parent;
			if (!parent)
			{
				Debug.LogError("Did not have parent!", this);
				return;
			}
			this.obj2 = parent.gameObject;
		}
		if (!this.obj1 || !this.obj2)
		{
			Debug.LogError("Both objects need to be assigned to ignore collision!", this);
			return;
		}
		Collider2D[] componentsInChildren = this.obj1.GetComponentsInChildren<Collider2D>(true);
		Collider2D[] componentsInChildren2 = this.obj2.GetComponentsInChildren<Collider2D>(true);
		foreach (Collider2D collider in componentsInChildren)
		{
			foreach (Collider2D collider2 in componentsInChildren2)
			{
				Physics2D.IgnoreCollision(collider, collider2);
			}
		}
	}

	// Token: 0x04003060 RID: 12384
	[SerializeField]
	private GameObject obj1;

	// Token: 0x04003061 RID: 12385
	[SerializeField]
	[ModifiableProperty]
	[Conditional("getParent", false, false, false)]
	private GameObject obj2;

	// Token: 0x04003062 RID: 12386
	[SerializeField]
	private bool getParent;
}
