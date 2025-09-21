using System;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class ScreenFlashAnimator : MonoBehaviour
{
	// Token: 0x06000BBB RID: 3003 RVA: 0x0003572F File Offset: 0x0003392F
	private void Awake()
	{
		if (this.requiredVisible)
		{
			this.requiredVisibleRenderers = this.requiredVisible.GetComponentsInChildren<Renderer>();
		}
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x00035750 File Offset: 0x00033950
	private bool CanFlash()
	{
		if (this.requiredVisibleRenderers == null)
		{
			return true;
		}
		Renderer[] array = this.requiredVisibleRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isVisible)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0003578C File Offset: 0x0003398C
	public void DoScreenFlash(int index)
	{
		if (!this.CanFlash())
		{
			return;
		}
		if (index < 0 || index >= this.screenFlashColours.Length)
		{
			return;
		}
		Color colour = this.screenFlashColours[index];
		GameCameras.instance.cameraController.ScreenFlash(colour);
	}

	// Token: 0x04000B4F RID: 2895
	[SerializeField]
	private GameObject requiredVisible;

	// Token: 0x04000B50 RID: 2896
	[SerializeField]
	private Color[] screenFlashColours = new Color[]
	{
		new Color(1f, 1f, 1f, 0.5f)
	};

	// Token: 0x04000B51 RID: 2897
	private Renderer[] requiredVisibleRenderers;
}
