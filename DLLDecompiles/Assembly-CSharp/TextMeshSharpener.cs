using System;
using UnityEngine;

// Token: 0x020007DB RID: 2011
public class TextMeshSharpener : MonoBehaviour
{
	// Token: 0x060046A6 RID: 18086 RVA: 0x0013B0F0 File Offset: 0x001392F0
	private void Start()
	{
		this.textMesh = base.GetComponent<TextMesh>();
		this.resize();
	}

	// Token: 0x060046A7 RID: 18087 RVA: 0x0013B104 File Offset: 0x00139304
	private void Update()
	{
		if ((float)Camera.main.pixelHeight != this.lastPixelHeight || (Application.isEditor && !Application.isPlaying))
		{
			this.resize();
		}
	}

	// Token: 0x060046A8 RID: 18088 RVA: 0x0013B130 File Offset: 0x00139330
	private void resize()
	{
		float num = (float)Camera.main.pixelHeight;
		float num2 = Camera.main.orthographicSize * 2f / num;
		float num3 = 128f;
		this.textMesh.characterSize = num2 * Camera.main.orthographicSize / Math.Max(base.transform.localScale.x, base.transform.localScale.y);
		this.textMesh.fontSize = (int)Math.Round((double)(num3 / this.textMesh.characterSize));
		this.lastPixelHeight = num;
	}

	// Token: 0x04004707 RID: 18183
	private float lastPixelHeight = -1f;

	// Token: 0x04004708 RID: 18184
	private TextMesh textMesh;
}
