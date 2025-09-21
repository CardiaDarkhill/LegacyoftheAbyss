using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
[ExecuteInEditMode]
public class CameraCaptureRender : MonoBehaviour
{
	// Token: 0x06000A87 RID: 2695 RVA: 0x0002F278 File Offset: 0x0002D478
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Shader.SetGlobalTexture(this.globalTextureName, source);
		Graphics.Blit(source, destination);
	}

	// Token: 0x040009FF RID: 2559
	[SerializeField]
	private string globalTextureName = "_GameCameraCapture";
}
