using System;
using UnityEngine;

// Token: 0x020006C4 RID: 1732
public class InvRelicBackboard : MonoBehaviour
{
	// Token: 0x06003EAE RID: 16046 RVA: 0x00113F88 File Offset: 0x00112188
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06003EAF RID: 16047 RVA: 0x00113F96 File Offset: 0x00112196
	private void OnEnable()
	{
		if (this.spriteRenderer != null)
		{
			if (this.playerData == null)
			{
				this.playerData = PlayerData.instance;
			}
			this.spriteRenderer.sprite = this.activeSprite;
		}
	}

	// Token: 0x04004053 RID: 16467
	public Sprite activeSprite;

	// Token: 0x04004054 RID: 16468
	public Sprite inactiveSprite;

	// Token: 0x04004055 RID: 16469
	private PlayerData playerData;

	// Token: 0x04004056 RID: 16470
	private SpriteRenderer spriteRenderer;
}
