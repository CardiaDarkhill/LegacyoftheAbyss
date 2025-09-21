using System;
using UnityEngine;

// Token: 0x02000706 RID: 1798
public sealed class RectTransformDimensionChangedEvent : MonoBehaviour
{
	// Token: 0x140000DD RID: 221
	// (add) Token: 0x06004029 RID: 16425 RVA: 0x0011A8B8 File Offset: 0x00118AB8
	// (remove) Token: 0x0600402A RID: 16426 RVA: 0x0011A8F0 File Offset: 0x00118AF0
	public event Action DimensionsChanged;

	// Token: 0x0600402B RID: 16427 RVA: 0x0011A925 File Offset: 0x00118B25
	private void OnRectTransformDimensionsChange()
	{
		Action dimensionsChanged = this.DimensionsChanged;
		if (dimensionsChanged == null)
		{
			return;
		}
		dimensionsChanged();
	}
}
