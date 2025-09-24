using System;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class CameraRenderHooks : MonoBehaviour
{
	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000AF8 RID: 2808 RVA: 0x00031B78 File Offset: 0x0002FD78
	// (remove) Token: 0x06000AF9 RID: 2809 RVA: 0x00031BAC File Offset: 0x0002FDAC
	public static event Action<CameraRenderHooks.CameraSource> CameraPreCull;

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000AFA RID: 2810 RVA: 0x00031BE0 File Offset: 0x0002FDE0
	// (remove) Token: 0x06000AFB RID: 2811 RVA: 0x00031C14 File Offset: 0x0002FE14
	public static event Action<CameraRenderHooks.CameraSource> CameraPostRender;

	// Token: 0x06000AFC RID: 2812 RVA: 0x00031C47 File Offset: 0x0002FE47
	private void OnPreCull()
	{
		Action<CameraRenderHooks.CameraSource> cameraPreCull = CameraRenderHooks.CameraPreCull;
		if (cameraPreCull == null)
		{
			return;
		}
		cameraPreCull(this.cameraType);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00031C5E File Offset: 0x0002FE5E
	private void OnPostRender()
	{
		Action<CameraRenderHooks.CameraSource> cameraPostRender = CameraRenderHooks.CameraPostRender;
		if (cameraPostRender == null)
		{
			return;
		}
		cameraPostRender(this.cameraType);
	}

	// Token: 0x04000A80 RID: 2688
	[SerializeField]
	private CameraRenderHooks.CameraSource cameraType;

	// Token: 0x02001495 RID: 5269
	public enum CameraSource
	{
		// Token: 0x040083C7 RID: 33735
		Misc,
		// Token: 0x040083C8 RID: 33736
		MainCamera,
		// Token: 0x040083C9 RID: 33737
		HudCamera
	}
}
