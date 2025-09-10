using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class BindThreadling : MonoBehaviour
{
	// Token: 0x060013CE RID: 5070 RVA: 0x0005A276 File Offset: 0x00058476
	private void Start()
	{
		if (this.transform_Hornet == null)
		{
			this.transform_Hornet = GameManager.instance.hero_ctrl.transform;
		}
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0005A29B File Offset: 0x0005849B
	private void OnEnable()
	{
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0005A29D File Offset: 0x0005849D
	private void Update()
	{
	}

	// Token: 0x04001233 RID: 4659
	public Rigidbody2D rb;

	// Token: 0x04001234 RID: 4660
	public Transform transform_Hornet;
}
