using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200073F RID: 1855
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class UIImageKeepNativeSize : MonoBehaviour
{
	// Token: 0x06004233 RID: 16947 RVA: 0x00124A30 File Offset: 0x00122C30
	private void Awake()
	{
		this.image = base.GetComponent<Image>();
	}

	// Token: 0x06004234 RID: 16948 RVA: 0x00124A3E File Offset: 0x00122C3E
	private void OnRectTransformDimensionsChange()
	{
		if (this.image)
		{
			this.image.SetNativeSize();
		}
	}

	// Token: 0x06004235 RID: 16949 RVA: 0x00124A58 File Offset: 0x00122C58
	private void LateUpdate()
	{
		if (this.image.sprite != this.sprite)
		{
			this.sprite = this.image.sprite;
			this.image.SetNativeSize();
		}
	}

	// Token: 0x040043D6 RID: 17366
	private Image image;

	// Token: 0x040043D7 RID: 17367
	private Sprite sprite;
}
