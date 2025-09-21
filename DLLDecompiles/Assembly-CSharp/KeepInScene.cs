using System;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class KeepInScene : MonoBehaviour
{
	// Token: 0x060022B0 RID: 8880 RVA: 0x0009F6D1 File Offset: 0x0009D8D1
	private void Start()
	{
		this.gm = GameManager.instance;
		this.maxX = this.gm.sceneWidth;
		this.maxY = this.gm.sceneHeight;
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x0009F700 File Offset: 0x0009D900
	private void OnEnable()
	{
		this.gm = GameManager.instance;
		this.maxX = this.gm.sceneWidth;
		this.maxY = this.gm.sceneHeight;
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x0009F730 File Offset: 0x0009D930
	private void Update()
	{
		Vector3 position = base.transform.position;
		bool flag = false;
		if (position.x < this.minX)
		{
			position.x = this.minX;
			flag = true;
		}
		else if (position.x > this.maxX)
		{
			position.x = this.maxX;
			flag = true;
		}
		if (position.y < this.minY)
		{
			position.y = this.minY;
			flag = true;
		}
		else if (position.y > this.maxY)
		{
			position.y = this.maxY;
			flag = true;
		}
		if (flag)
		{
			base.transform.position = position;
		}
	}

	// Token: 0x04002185 RID: 8581
	private GameManager gm;

	// Token: 0x04002186 RID: 8582
	private float minX;

	// Token: 0x04002187 RID: 8583
	private float maxX;

	// Token: 0x04002188 RID: 8584
	private float minY = -10f;

	// Token: 0x04002189 RID: 8585
	private float maxY;
}
