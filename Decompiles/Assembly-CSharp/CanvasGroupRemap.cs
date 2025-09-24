using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000611 RID: 1553
[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupRemap : MonoBehaviour
{
	// Token: 0x06003770 RID: 14192 RVA: 0x000F4862 File Offset: 0x000F2A62
	private void Awake()
	{
		if (!this.group)
		{
			this.group = base.GetComponent<CanvasGroup>();
		}
		this.spriteRenderers = base.GetComponentsInChildren<SpriteRenderer>(true);
		this.textMeshes = base.GetComponentsInChildren<TextMeshPro>(true);
		this.Sync(0f);
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x000F48A4 File Offset: 0x000F2AA4
	private void Update()
	{
		if (!this.skippedFirstUpdate)
		{
			this.skippedFirstUpdate = true;
			return;
		}
		if (this.group.alpha != this.alpha)
		{
			this.alpha = this.group.alpha;
			this.Sync(this.alpha);
		}
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x000F48F4 File Offset: 0x000F2AF4
	private void Sync(float alpha)
	{
		foreach (SpriteRenderer spriteRenderer in this.spriteRenderers)
		{
			Color color = spriteRenderer.color;
			color.a = alpha;
			spriteRenderer.color = color;
		}
		foreach (TextMeshPro textMeshPro in this.textMeshes)
		{
			Color color2 = textMeshPro.color;
			color2.a = alpha;
			textMeshPro.color = color2;
		}
	}

	// Token: 0x04003A5A RID: 14938
	private SpriteRenderer[] spriteRenderers;

	// Token: 0x04003A5B RID: 14939
	private TextMeshPro[] textMeshes;

	// Token: 0x04003A5C RID: 14940
	public CanvasGroup group;

	// Token: 0x04003A5D RID: 14941
	private float alpha;

	// Token: 0x04003A5E RID: 14942
	private bool skippedFirstUpdate;
}
