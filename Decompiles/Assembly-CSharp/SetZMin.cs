using System;
using UnityEngine;

// Token: 0x020005C3 RID: 1475
public class SetZMin : MonoBehaviour
{
	// Token: 0x060034A8 RID: 13480 RVA: 0x000E9D6D File Offset: 0x000E7F6D
	private void OnEnable()
	{
		this.SetZ();
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x000E9D75 File Offset: 0x000E7F75
	private void Start()
	{
		this.SetZ();
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x000E9D80 File Offset: 0x000E7F80
	private void SetZ()
	{
		if (base.transform.position.z < this.zMin)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, this.zMin);
		}
	}

	// Token: 0x0400381A RID: 14362
	public float zMin;
}
