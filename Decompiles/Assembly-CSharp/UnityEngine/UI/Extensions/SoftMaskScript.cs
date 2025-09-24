using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x0200087F RID: 2175
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
	public class SoftMaskScript : MonoBehaviour
	{
		// Token: 0x06004BF2 RID: 19442 RVA: 0x001669DC File Offset: 0x00164BDC
		private void Start()
		{
			if (this.MaskArea == null)
			{
				this.MaskArea = base.GetComponent<RectTransform>();
			}
			Text component = base.GetComponent<Text>();
			if (component != null)
			{
				this.mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
				component.material = this.mat;
				this.cachedCanvas = component.canvas;
				this.cachedCanvasTransform = this.cachedCanvas.transform;
				if (base.transform.parent.GetComponent<Mask>() == null)
				{
					base.transform.parent.gameObject.AddComponent<Mask>();
				}
				base.transform.parent.GetComponent<Mask>().enabled = false;
				return;
			}
			Graphic component2 = base.GetComponent<Graphic>();
			if (component2 != null)
			{
				this.mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
				component2.material = this.mat;
				this.cachedCanvas = component2.canvas;
				this.cachedCanvasTransform = this.cachedCanvas.transform;
			}
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x00166AE7 File Offset: 0x00164CE7
		private void Update()
		{
			if (this.cachedCanvas != null)
			{
				this.SetMask();
			}
		}

		// Token: 0x06004BF4 RID: 19444 RVA: 0x00166B00 File Offset: 0x00164D00
		private void SetMask()
		{
			Rect canvasRect = this.GetCanvasRect();
			Vector2 size = canvasRect.size;
			this.maskScale.Set(1f / size.x, 1f / size.y);
			this.maskOffset = -canvasRect.min;
			this.maskOffset.Scale(this.maskScale);
			this.mat.SetTextureOffset("_AlphaMask", this.maskOffset);
			this.mat.SetTextureScale("_AlphaMask", this.maskScale);
			this.mat.SetTexture("_AlphaMask", this.AlphaMask);
			this.mat.SetFloat("_HardBlend", (float)(this.HardBlend ? 1 : 0));
			this.mat.SetInt("_FlipAlphaMask", this.FlipAlphaMask ? 1 : 0);
			this.mat.SetFloat("_CutOff", this.CutOff);
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x00166BF4 File Offset: 0x00164DF4
		public Rect GetCanvasRect()
		{
			if (this.cachedCanvas == null)
			{
				return default(Rect);
			}
			this.MaskArea.GetWorldCorners(this.m_WorldCorners);
			for (int i = 0; i < 4; i++)
			{
				this.m_CanvasCorners[i] = this.cachedCanvasTransform.InverseTransformPoint(this.m_WorldCorners[i]);
			}
			return new Rect(this.m_CanvasCorners[0].x, this.m_CanvasCorners[0].y, this.m_CanvasCorners[2].x - this.m_CanvasCorners[0].x, this.m_CanvasCorners[2].y - this.m_CanvasCorners[0].y);
		}

		// Token: 0x04004D59 RID: 19801
		private Material mat;

		// Token: 0x04004D5A RID: 19802
		private Canvas cachedCanvas;

		// Token: 0x04004D5B RID: 19803
		private Transform cachedCanvasTransform;

		// Token: 0x04004D5C RID: 19804
		private readonly Vector3[] m_WorldCorners = new Vector3[4];

		// Token: 0x04004D5D RID: 19805
		private readonly Vector3[] m_CanvasCorners = new Vector3[4];

		// Token: 0x04004D5E RID: 19806
		[Tooltip("The area that is to be used as the container.")]
		public RectTransform MaskArea;

		// Token: 0x04004D5F RID: 19807
		[Tooltip("Texture to be used to do the soft alpha")]
		public Texture AlphaMask;

		// Token: 0x04004D60 RID: 19808
		[Tooltip("At what point to apply the alpha min range 0-1")]
		[Range(0f, 1f)]
		public float CutOff;

		// Token: 0x04004D61 RID: 19809
		[Tooltip("Implement a hard blend based on the Cutoff")]
		public bool HardBlend;

		// Token: 0x04004D62 RID: 19810
		[Tooltip("Flip the masks alpha value")]
		public bool FlipAlphaMask;

		// Token: 0x04004D63 RID: 19811
		private Vector2 maskOffset = Vector2.zero;

		// Token: 0x04004D64 RID: 19812
		private Vector2 maskScale = Vector2.one;
	}
}
