using System;
using UnityEngine;

// Token: 0x020006C3 RID: 1731
public class InvNailSprite : MonoBehaviour
{
	// Token: 0x06003EAB RID: 16043 RVA: 0x00113EB7 File Offset: 0x001120B7
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06003EAC RID: 16044 RVA: 0x00113EC8 File Offset: 0x001120C8
	private void OnEnable()
	{
		if (this.playerData == null)
		{
			this.playerData = PlayerData.instance;
		}
		switch (this.playerData.GetInt("nailSmithUpgrades"))
		{
		case 0:
			this.spriteRenderer.sprite = this.level1;
			return;
		case 1:
			this.spriteRenderer.sprite = this.level2;
			return;
		case 2:
			this.spriteRenderer.sprite = this.level3;
			return;
		case 3:
			this.spriteRenderer.sprite = this.level4;
			return;
		case 4:
			this.spriteRenderer.sprite = this.level5;
			return;
		default:
			this.spriteRenderer.sprite = this.level1;
			return;
		}
	}

	// Token: 0x0400404C RID: 16460
	public Sprite level1;

	// Token: 0x0400404D RID: 16461
	public Sprite level2;

	// Token: 0x0400404E RID: 16462
	public Sprite level3;

	// Token: 0x0400404F RID: 16463
	public Sprite level4;

	// Token: 0x04004050 RID: 16464
	public Sprite level5;

	// Token: 0x04004051 RID: 16465
	private SpriteRenderer spriteRenderer;

	// Token: 0x04004052 RID: 16466
	private PlayerData playerData;
}
