using System;
using UnityEngine;

// Token: 0x02000787 RID: 1927
public class SpritePreviewAttribute : PropertyAttribute
{
	// Token: 0x06004468 RID: 17512 RVA: 0x0012BA0F File Offset: 0x00129C0F
	public SpritePreviewAttribute(float previewHeight = 64f)
	{
		this.previewHeight = previewHeight;
	}

	// Token: 0x0400457D RID: 17789
	public float previewHeight;
}
