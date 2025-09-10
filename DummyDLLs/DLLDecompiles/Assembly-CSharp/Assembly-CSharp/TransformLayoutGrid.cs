using System;
using UnityEngine;

// Token: 0x0200073B RID: 1851
[ExecuteInEditMode]
public class TransformLayoutGrid : TransformLayout
{
	// Token: 0x06004211 RID: 16913 RVA: 0x001228B4 File Offset: 0x00120AB4
	[ContextMenu("Refresh")]
	public override void UpdatePositions()
	{
		int childCount = base.transform.childCount;
		int num = this.startAtZero ? -1 : 0;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				num++;
				Vector2 position = this.gridOffset + this.itemOffset * (float)num;
				child.SetLocalPosition2D(position);
			}
		}
	}

	// Token: 0x0400438D RID: 17293
	[SerializeField]
	private Vector2 gridOffset;

	// Token: 0x0400438E RID: 17294
	[SerializeField]
	private Vector2 itemOffset;

	// Token: 0x0400438F RID: 17295
	[SerializeField]
	private bool startAtZero;
}
