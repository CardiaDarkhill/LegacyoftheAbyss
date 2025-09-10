using System;
using UnityEngine;

// Token: 0x020006BF RID: 1727
public class InvItemDisplay : MonoBehaviour
{
	// Token: 0x06003E97 RID: 16023 RVA: 0x00113C18 File Offset: 0x00111E18
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06003E98 RID: 16024 RVA: 0x00113C28 File Offset: 0x00111E28
	private void OnEnable()
	{
		if (this.spriteRenderer != null)
		{
			if (this.playerData == null)
			{
				this.playerData = PlayerData.instance;
			}
			if (this.playerData.GetBool(this.playerDataBool))
			{
				this.spriteRenderer.sprite = this.activeSprite;
				return;
			}
			this.spriteRenderer.sprite = this.inactiveSprite;
		}
	}

	// Token: 0x04004040 RID: 16448
	public string playerDataBool;

	// Token: 0x04004041 RID: 16449
	public Sprite activeSprite;

	// Token: 0x04004042 RID: 16450
	public Sprite inactiveSprite;

	// Token: 0x04004043 RID: 16451
	private PlayerData playerData;

	// Token: 0x04004044 RID: 16452
	private SpriteRenderer spriteRenderer;
}
