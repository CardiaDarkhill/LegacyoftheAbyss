using System;
using UnityEngine;

// Token: 0x02000356 RID: 854
public class InstantiateAsChild : MonoBehaviour
{
	// Token: 0x06001D94 RID: 7572 RVA: 0x000887F0 File Offset: 0x000869F0
	private void Awake()
	{
		if (!this.prefab)
		{
			return;
		}
		Transform parent = this.transformOverride ? this.transformOverride : base.transform;
		Vector3 vector = this.usePrefabLocalPosition ? this.prefab.transform.localPosition : this.localPosition;
		Transform transform = Object.Instantiate<GameObject>(this.prefab, parent).transform;
		transform.localPosition = vector;
		transform.localScale = transform.localScale.MultiplyElements(this.scaleMultiplier);
	}

	// Token: 0x04001CC5 RID: 7365
	[SerializeField]
	private Transform transformOverride;

	// Token: 0x04001CC6 RID: 7366
	[SerializeField]
	private GameObject prefab;

	// Token: 0x04001CC7 RID: 7367
	[SerializeField]
	private bool usePrefabLocalPosition;

	// Token: 0x04001CC8 RID: 7368
	[SerializeField]
	[ModifiableProperty]
	[Conditional("usePrefabLocalPosition", false, false, false)]
	private Vector3 localPosition;

	// Token: 0x04001CC9 RID: 7369
	[SerializeField]
	private Vector3 scaleMultiplier = Vector3.one;
}
