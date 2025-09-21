using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
[ExecuteInEditMode]
public class AnimateSpriteIndex : MonoBehaviour
{
	// Token: 0x0600026C RID: 620 RVA: 0x0000E755 File Offset: 0x0000C955
	private void LateUpdate()
	{
		this.UpdateSprite();
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000E760 File Offset: 0x0000C960
	private void UpdateSprite()
	{
		if (this.index == this.previousIndex)
		{
			return;
		}
		this.index = Mathf.Clamp(this.index, 0, this.sprites.Length - 1);
		this.previousIndex = this.index;
		if (this.sprites.Length == 0)
		{
			return;
		}
		if (this.spriteRenderer)
		{
			this.spriteRenderer.sprite = this.sprites[this.index];
		}
	}

	// Token: 0x04000214 RID: 532
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04000215 RID: 533
	[SerializeField]
	private int index;

	// Token: 0x04000216 RID: 534
	[SerializeField]
	private Sprite[] sprites;

	// Token: 0x04000217 RID: 535
	private int previousIndex = -1;
}
