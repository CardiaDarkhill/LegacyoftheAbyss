using System;
using UnityEngine;

// Token: 0x020005C1 RID: 1473
[ExecuteInEditMode]
public class SetWorldScale : MonoBehaviour
{
	// Token: 0x0600349D RID: 13469 RVA: 0x000E9B90 File Offset: 0x000E7D90
	private void Awake()
	{
		this.DoSet();
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x000E9B98 File Offset: 0x000E7D98
	private void DoSet()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if (parent)
		{
			Vector3 lossyScale = parent.lossyScale;
			transform.localScale = new Vector3(this.worldScale.x / lossyScale.x, this.worldScale.y / lossyScale.y, this.worldScale.z / lossyScale.z);
			return;
		}
		transform.localScale = this.worldScale;
	}

	// Token: 0x0400380E RID: 14350
	[SerializeField]
	private Vector3 worldScale = Vector3.one;
}
