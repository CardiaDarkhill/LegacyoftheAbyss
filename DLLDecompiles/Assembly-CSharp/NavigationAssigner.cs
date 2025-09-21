using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000777 RID: 1911
public class NavigationAssigner : MonoBehaviour
{
	// Token: 0x06004403 RID: 17411 RVA: 0x0012A763 File Offset: 0x00128963
	[ContextMenu("Gather")]
	private void Gather()
	{
		this.selectables = base.GetComponentsInChildren<Selectable>().Union(this.selectables).ToList<Selectable>();
	}

	// Token: 0x06004404 RID: 17412 RVA: 0x0012A784 File Offset: 0x00128984
	[ContextMenu("Assign")]
	private void AssignNavigation()
	{
		this.selectables.RemoveAll((Selectable o) => o == null);
		if (this.selectables.Count > 1)
		{
			Selectable selectOnUp = this.selectables[this.selectables.Count - 1];
			for (int i = 0; i < this.selectables.Count; i++)
			{
				Selectable selectable = this.selectables[i];
				Selectable selectOnDown = (i < this.selectables.Count - 1) ? this.selectables[i + 1] : this.selectables[0];
				Navigation navigation = selectable.navigation;
				navigation.selectOnUp = selectOnUp;
				navigation.selectOnDown = selectOnDown;
				selectable.navigation = navigation;
				selectOnUp = selectable;
			}
		}
	}

	// Token: 0x0400454A RID: 17738
	[SerializeField]
	private Navigation.Mode mode = Navigation.Mode.Explicit;

	// Token: 0x0400454B RID: 17739
	[SerializeField]
	private List<Selectable> selectables = new List<Selectable>();
}
