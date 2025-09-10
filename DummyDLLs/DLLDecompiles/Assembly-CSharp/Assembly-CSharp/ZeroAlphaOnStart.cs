using System;
using UnityEngine;

// Token: 0x0200074B RID: 1867
[RequireComponent(typeof(CanvasGroup))]
public class ZeroAlphaOnStart : MonoBehaviour
{
	// Token: 0x06004276 RID: 17014 RVA: 0x00125579 File Offset: 0x00123779
	private void Start()
	{
		base.GetComponent<CanvasGroup>().alpha = 0f;
	}
}
