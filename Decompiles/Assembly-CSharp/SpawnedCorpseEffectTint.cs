using System;
using UnityEngine;

// Token: 0x020002BD RID: 701
public class SpawnedCorpseEffectTint : MonoBehaviour
{
	// Token: 0x060018C0 RID: 6336 RVA: 0x00071770 File Offset: 0x0006F970
	private void Awake()
	{
		this.sprites = base.GetComponentsInChildren<tk2dSprite>();
		this.spriteColors = new Color[this.sprites.Length];
		for (int i = 0; i < this.sprites.Length; i++)
		{
			this.spriteColors[i] = this.sprites[i].color;
		}
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x000717C8 File Offset: 0x0006F9C8
	private void OnDisable()
	{
		for (int i = 0; i < this.sprites.Length; i++)
		{
			this.sprites[i].color = this.spriteColors[i];
		}
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x00071804 File Offset: 0x0006FA04
	public void SetTint(Color color)
	{
		for (int i = 0; i < this.sprites.Length; i++)
		{
			this.sprites[i].color = this.spriteColors[i] * color;
		}
	}

	// Token: 0x040017B6 RID: 6070
	private tk2dSprite[] sprites;

	// Token: 0x040017B7 RID: 6071
	private Color[] spriteColors;
}
