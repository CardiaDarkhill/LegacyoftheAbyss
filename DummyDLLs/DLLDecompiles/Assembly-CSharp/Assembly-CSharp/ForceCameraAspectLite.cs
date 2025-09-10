using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class ForceCameraAspectLite : MonoBehaviour
{
	// Token: 0x06000B71 RID: 2929 RVA: 0x000346A3 File Offset: 0x000328A3
	private void Start()
	{
		this.AutoScaleViewport();
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000346AC File Offset: 0x000328AC
	private void Update()
	{
		this.viewportChanged = false;
		if (this.lastX != Screen.width)
		{
			this.viewportChanged = true;
		}
		if (this.lastY != Screen.height)
		{
			this.viewportChanged = true;
		}
		if (this.viewportChanged)
		{
			this.AutoScaleViewport();
		}
		this.lastX = Screen.width;
		this.lastY = Screen.height;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0003470C File Offset: 0x0003290C
	private void AutoScaleViewport()
	{
		float num = (float)Screen.width / (float)Screen.height / 1.7777778f;
		float num2 = 1f + this.scaleAdjust;
		Rect rect = this.sceneCamera.rect;
		if (num < 1f)
		{
			rect.width = 1f * num2;
			rect.height = num * num2;
			float x = (1f - rect.width) / 2f;
			rect.x = x;
			float y = (1f - rect.height) / 2f;
			rect.y = y;
		}
		else
		{
			float num3 = 1f / num;
			rect.width = num3 * num2;
			rect.height = 1f * num2;
			float x2 = (1f - rect.width) / 2f;
			rect.x = x2;
			float y2 = (1f - rect.height) / 2f;
			rect.y = y2;
		}
		this.sceneCamera.rect = rect;
	}

	// Token: 0x04000B13 RID: 2835
	public Camera sceneCamera;

	// Token: 0x04000B14 RID: 2836
	private bool viewportChanged;

	// Token: 0x04000B15 RID: 2837
	private int lastX;

	// Token: 0x04000B16 RID: 2838
	private int lastY;

	// Token: 0x04000B17 RID: 2839
	private float scaleAdjust;
}
