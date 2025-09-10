using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Lens CA And Distortion")]
public class LensCAAndDistortion : MonoBehaviour
{
	// Token: 0x17000012 RID: 18
	// (get) Token: 0x0600008B RID: 139 RVA: 0x0000467E File Offset: 0x0000287E
	private Material material
	{
		get
		{
			if (this.curMaterial == null)
			{
				this.curMaterial = new Material(this.LensShader);
				this.curMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.curMaterial;
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x000046B4 File Offset: 0x000028B4
	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (this.LensShader != null)
		{
			this.material.SetFloat("_RedCyan", this.RedCyan * 10f);
			this.material.SetFloat("_GreenMagenta", this.GreenMagenta * 10f);
			this.material.SetFloat("_BlueYellow", this.BlueYellow * 10f);
			this.material.SetFloat("_Zoom", 0f - this.Zoom);
			this.material.SetFloat("_BarrelDistortion", this.BarrelDistortion);
			this.material.SetTexture("_BorderTex", this.TrimTexture);
			if (this.TrimExtents)
			{
				this.material.SetInt("_BorderOnOff", 1);
			}
			else
			{
				this.material.SetInt("_BorderOnOff", 0);
			}
			Graphics.Blit(sourceTexture, destTexture, this.material);
			return;
		}
		Graphics.Blit(sourceTexture, destTexture);
	}

	// Token: 0x0600008D RID: 141 RVA: 0x000047AF File Offset: 0x000029AF
	private void Update()
	{
	}

	// Token: 0x0600008E RID: 142 RVA: 0x000047B1 File Offset: 0x000029B1
	private void OnDisable()
	{
		if (this.curMaterial)
		{
			Object.DestroyImmediate(this.curMaterial);
		}
	}

	// Token: 0x04000062 RID: 98
	public Shader LensShader;

	// Token: 0x04000063 RID: 99
	[Range(-10f, 10f)]
	public float RedCyan;

	// Token: 0x04000064 RID: 100
	[Range(-10f, 10f)]
	public float GreenMagenta;

	// Token: 0x04000065 RID: 101
	[Range(-10f, 10f)]
	public float BlueYellow;

	// Token: 0x04000066 RID: 102
	public bool TrimExtents;

	// Token: 0x04000067 RID: 103
	public Texture2D TrimTexture;

	// Token: 0x04000068 RID: 104
	[Range(-1f, 1f)]
	public float Zoom;

	// Token: 0x04000069 RID: 105
	[Range(-5f, 5f)]
	public float BarrelDistortion;

	// Token: 0x0400006A RID: 106
	private Material curMaterial;
}
