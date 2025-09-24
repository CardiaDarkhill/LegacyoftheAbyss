using System;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class SetWorldTransform : MonoBehaviour
{
	// Token: 0x0600167A RID: 5754 RVA: 0x00065258 File Offset: 0x00063458
	private void Start()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		transform.SetParent(null, true);
		transform.localEulerAngles = this.eulerAngles;
		transform.localScale = this.scale;
		if (!this.deParent)
		{
			transform.SetParent(parent, true);
		}
	}

	// Token: 0x040014EC RID: 5356
	[SerializeField]
	private Vector3 eulerAngles;

	// Token: 0x040014ED RID: 5357
	[SerializeField]
	private Vector3 scale;

	// Token: 0x040014EE RID: 5358
	[SerializeField]
	private bool deParent;
}
