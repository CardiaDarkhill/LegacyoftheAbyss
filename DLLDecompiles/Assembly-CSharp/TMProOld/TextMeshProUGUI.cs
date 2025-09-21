using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x020007E9 RID: 2025
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(CanvasRenderer))]
	[AddComponentMenu("UI/TextMeshPro - Text (UI) (OLD)", 11)]
	[SelectionBase]
	public class TextMeshProUGUI : TMP_Text, ILayoutElement
	{
		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x0600476A RID: 18282 RVA: 0x001450E8 File Offset: 0x001432E8
		public override Material materialForRendering
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return null;
				}
				return this.GetModifiedMaterial(this.m_sharedMaterial);
			}
		}

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x0600476B RID: 18283 RVA: 0x00145106 File Offset: 0x00143306
		public override Mesh mesh
		{
			get
			{
				return this.m_mesh;
			}
		}

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x0600476C RID: 18284 RVA: 0x0014510E File Offset: 0x0014330E
		public new CanvasRenderer canvasRenderer
		{
			get
			{
				if (this.m_canvasRenderer == null)
				{
					this.m_canvasRenderer = base.GetComponent<CanvasRenderer>();
				}
				return this.m_canvasRenderer;
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x0600476D RID: 18285 RVA: 0x00145130 File Offset: 0x00143330
		public InlineGraphicManager inlineGraphicManager
		{
			get
			{
				return this.m_inlineGraphics;
			}
		}

		// Token: 0x0600476E RID: 18286 RVA: 0x00145138 File Offset: 0x00143338
		public void CalculateLayoutInputHorizontal()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_isCalculateSizeRequired || this.m_rectTransform.hasChanged)
			{
				this.m_preferredWidth = base.GetPreferredWidth();
				this.ComputeMarginSize();
				this.m_isLayoutDirty = true;
			}
		}

		// Token: 0x0600476F RID: 18287 RVA: 0x00145178 File Offset: 0x00143378
		public void CalculateLayoutInputVertical()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_isCalculateSizeRequired || this.m_rectTransform.hasChanged)
			{
				this.m_preferredHeight = base.GetPreferredHeight();
				this.ComputeMarginSize();
				this.m_isLayoutDirty = true;
			}
			this.m_isCalculateSizeRequired = false;
		}

		// Token: 0x06004770 RID: 18288 RVA: 0x001451C8 File Offset: 0x001433C8
		public override void SetVerticesDirty()
		{
			if (this.m_verticesAlreadyDirty || this == null || !this.IsActive() || CanvasUpdateRegistry.IsRebuildingGraphics())
			{
				return;
			}
			this.m_verticesAlreadyDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		// Token: 0x06004771 RID: 18289 RVA: 0x001451F8 File Offset: 0x001433F8
		public override void SetLayoutDirty()
		{
			if (this.m_layoutAlreadyDirty || this == null || !this.IsActive())
			{
				return;
			}
			this.m_layoutAlreadyDirty = true;
			LayoutRebuilder.MarkLayoutForRebuild(base.rectTransform);
			this.m_isLayoutDirty = true;
		}

		// Token: 0x06004772 RID: 18290 RVA: 0x0014522D File Offset: 0x0014342D
		public override void SetMaterialDirty()
		{
			if (this == null || !this.IsActive() || CanvasUpdateRegistry.IsRebuildingGraphics())
			{
				return;
			}
			this.m_isMaterialDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		// Token: 0x06004773 RID: 18291 RVA: 0x00145255 File Offset: 0x00143455
		public override void SetAllDirty()
		{
			this.SetLayoutDirty();
			this.SetVerticesDirty();
			this.SetMaterialDirty();
		}

		// Token: 0x06004774 RID: 18292 RVA: 0x00145269 File Offset: 0x00143469
		public override void Rebuild(CanvasUpdate update)
		{
			if (this == null)
			{
				return;
			}
			if (update == CanvasUpdate.PreRender)
			{
				this.OnPreRenderCanvas();
				this.m_verticesAlreadyDirty = false;
				this.m_layoutAlreadyDirty = false;
				if (!this.m_isMaterialDirty)
				{
					return;
				}
				this.UpdateMaterial();
				this.m_isMaterialDirty = false;
			}
		}

		// Token: 0x06004775 RID: 18293 RVA: 0x001452A4 File Offset: 0x001434A4
		private void UpdateSubObjectPivot()
		{
			if (this.m_textInfo == null)
			{
				return;
			}
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				this.m_subTextObjects[num].SetPivotDirty();
				num++;
			}
		}

		// Token: 0x06004776 RID: 18294 RVA: 0x001452EC File Offset: 0x001434EC
		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (this.m_ShouldRecalculateStencil)
			{
				this.m_stencilID = TMP_MaterialManager.GetStencilID(base.gameObject);
				this.m_ShouldRecalculateStencil = false;
			}
			if (this.m_stencilID > 0)
			{
				material = TMP_MaterialManager.GetStencilMaterial(baseMaterial, this.m_stencilID);
				if (this.m_MaskMaterial != null)
				{
					TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
				}
				this.m_MaskMaterial = material;
			}
			return material;
		}

		// Token: 0x06004777 RID: 18295 RVA: 0x00145354 File Offset: 0x00143554
		protected override void UpdateMaterial()
		{
			if (this.m_canvasRenderer == null)
			{
				this.m_canvasRenderer = this.canvasRenderer;
			}
			this.m_canvasRenderer.materialCount = 1;
			this.m_canvasRenderer.SetMaterial(this.materialForRendering, this.m_sharedMaterial.mainTexture);
		}

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x06004778 RID: 18296 RVA: 0x001453A3 File Offset: 0x001435A3
		// (set) Token: 0x06004779 RID: 18297 RVA: 0x001453AB File Offset: 0x001435AB
		public Vector4 maskOffset
		{
			get
			{
				return this.m_maskOffset;
			}
			set
			{
				this.m_maskOffset = value;
				this.UpdateMask();
				this.m_havePropertiesChanged = true;
			}
		}

		// Token: 0x0600477A RID: 18298 RVA: 0x001453C1 File Offset: 0x001435C1
		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		// Token: 0x0600477B RID: 18299 RVA: 0x001453C9 File Offset: 0x001435C9
		public override void RecalculateMasking()
		{
			this.m_ShouldRecalculateStencil = true;
			this.SetMaterialDirty();
		}

		// Token: 0x0600477C RID: 18300 RVA: 0x001453D8 File Offset: 0x001435D8
		public override void UpdateMeshPadding()
		{
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_havePropertiesChanged = true;
			this.checkPaddingRequired = false;
			for (int i = 1; i < this.m_textInfo.materialCount; i++)
			{
				this.m_subTextObjects[i].UpdateMeshPadding(this.m_enableExtraPadding, this.m_isUsingBold);
			}
		}

		// Token: 0x0600477D RID: 18301 RVA: 0x00145450 File Offset: 0x00143650
		protected override void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 1; i < materialCount; i++)
			{
				this.m_subTextObjects[i].CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			}
		}

		// Token: 0x0600477E RID: 18302 RVA: 0x00145488 File Offset: 0x00143688
		protected override void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 1; i < materialCount; i++)
			{
				this.m_subTextObjects[i].CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			}
		}

		// Token: 0x0600477F RID: 18303 RVA: 0x001454BD File Offset: 0x001436BD
		public override void ForceMeshUpdate()
		{
			this.m_havePropertiesChanged = true;
			this.OnPreRenderCanvas();
		}

		// Token: 0x06004780 RID: 18304 RVA: 0x001454CC File Offset: 0x001436CC
		public override void ForceMeshUpdate(bool ignoreInactive)
		{
			this.m_havePropertiesChanged = true;
			this.m_ignoreActiveState = true;
			this.OnPreRenderCanvas();
		}

		// Token: 0x06004781 RID: 18305 RVA: 0x001454E4 File Offset: 0x001436E4
		public override TMP_TextInfo GetTextInfo(string text)
		{
			base.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			this.m_renderMode = TextRenderFlags.DontRender;
			this.ComputeMarginSize();
			if (this.m_canvas == null)
			{
				this.m_canvas = base.canvas;
			}
			this.GenerateTextMesh();
			this.m_renderMode = TextRenderFlags.Render;
			return base.textInfo;
		}

		// Token: 0x06004782 RID: 18306 RVA: 0x00145549 File Offset: 0x00143749
		public override void UpdateGeometry(Mesh mesh, int index)
		{
			mesh.RecalculateBounds();
			if (index == 0)
			{
				this.m_canvasRenderer.SetMesh(mesh);
				return;
			}
			this.m_subTextObjects[index].canvasRenderer.SetMesh(mesh);
		}

		// Token: 0x06004783 RID: 18307 RVA: 0x00145574 File Offset: 0x00143774
		public override void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 0; i < materialCount; i++)
			{
				Mesh mesh;
				if (i == 0)
				{
					mesh = this.m_mesh;
				}
				else
				{
					mesh = this.m_subTextObjects[i].mesh;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Vertices) == TMP_VertexDataUpdateFlags.Vertices)
				{
					mesh.vertices = this.m_textInfo.meshInfo[i].vertices;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Uv0) == TMP_VertexDataUpdateFlags.Uv0)
				{
					mesh.uv = this.m_textInfo.meshInfo[i].uvs0;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Uv2) == TMP_VertexDataUpdateFlags.Uv2)
				{
					mesh.uv2 = this.m_textInfo.meshInfo[i].uvs2;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Colors32) == TMP_VertexDataUpdateFlags.Colors32)
				{
					mesh.colors32 = this.m_textInfo.meshInfo[i].colors32;
				}
				mesh.RecalculateBounds();
				if (i == 0)
				{
					this.m_canvasRenderer.SetMesh(mesh);
				}
				else
				{
					this.m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
				}
			}
		}

		// Token: 0x06004784 RID: 18308 RVA: 0x00145670 File Offset: 0x00143870
		public override void UpdateVertexData()
		{
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 0; i < materialCount; i++)
			{
				Mesh mesh;
				if (i == 0)
				{
					mesh = this.m_mesh;
				}
				else
				{
					mesh = this.m_subTextObjects[i].mesh;
				}
				mesh.vertices = this.m_textInfo.meshInfo[i].vertices;
				mesh.uv = this.m_textInfo.meshInfo[i].uvs0;
				mesh.uv2 = this.m_textInfo.meshInfo[i].uvs2;
				mesh.colors32 = this.m_textInfo.meshInfo[i].colors32;
				mesh.RecalculateBounds();
				if (i == 0)
				{
					this.m_canvasRenderer.SetMesh(mesh);
				}
				else
				{
					this.m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
				}
			}
		}

		// Token: 0x06004785 RID: 18309 RVA: 0x0014574F File Offset: 0x0014394F
		public void UpdateFontAsset()
		{
			this.LoadFontAsset();
		}

		// Token: 0x06004786 RID: 18310 RVA: 0x00145758 File Offset: 0x00143958
		protected override void Awake()
		{
			this.m_canvas = base.canvas;
			this.m_isOrthographic = true;
			this.m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			if (this.m_rectTransform == null)
			{
				this.m_rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			this.m_canvasRenderer = base.GetComponent<CanvasRenderer>();
			if (this.m_canvasRenderer == null)
			{
				this.m_canvasRenderer = base.gameObject.AddComponent<CanvasRenderer>();
			}
			if (this.m_mesh == null)
			{
				this.m_mesh = new Mesh();
				this.m_mesh.name = "TMPro_UGUI_Private_Mesh";
				this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
			}
			if (this.m_text == null)
			{
				this.m_enableWordWrapping = TMP_Settings.enableWordWrapping;
				this.m_enableKerning = TMP_Settings.enableKerning;
				this.m_enableExtraPadding = TMP_Settings.enableExtraPadding;
				this.m_tintAllSprites = TMP_Settings.enableTintAllSprites;
				this.m_parseCtrlCharacters = TMP_Settings.enableParseEscapeCharacters;
			}
			this.LoadFontAsset();
			TMP_StyleSheet.LoadDefaultStyleSheet();
			this.m_char_buffer = new int[this.m_max_characters];
			this.m_cached_TextElement = new TMP_Glyph();
			this.m_isFirstAllocation = true;
			if (this.m_textInfo == null)
			{
				this.m_textInfo = new TMP_TextInfo(this);
			}
			if (this.m_fontAsset == null)
			{
				Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.", this);
				return;
			}
			if (this.m_fontSizeMin == 0f)
			{
				this.m_fontSizeMin = this.m_fontSize / 2f;
			}
			if (this.m_fontSizeMax == 0f)
			{
				this.m_fontSizeMax = this.m_fontSize * 2f;
			}
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.m_isAwake = true;
		}

		// Token: 0x06004787 RID: 18311 RVA: 0x00145914 File Offset: 0x00143B14
		protected override void OnEnable()
		{
			if (!this.m_isRegisteredForEvents)
			{
				this.m_isRegisteredForEvents = true;
			}
			this.m_canvas = this.GetCanvas();
			this.SetActiveSubMeshes(true);
			GraphicRegistry.RegisterGraphicForCanvas(this.m_canvas, this);
			this.ComputeMarginSize();
			this.m_verticesAlreadyDirty = false;
			this.m_layoutAlreadyDirty = false;
			this.m_ShouldRecalculateStencil = true;
			this.m_isInputParsingRequired = true;
			this.SetAllDirty();
			this.RecalculateClipping();
		}

		// Token: 0x06004788 RID: 18312 RVA: 0x00145980 File Offset: 0x00143B80
		protected override void OnDisable()
		{
			if (this.m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
				this.m_MaskMaterial = null;
			}
			GraphicRegistry.UnregisterGraphicForCanvas(this.m_canvas, this);
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_canvasRenderer != null)
			{
				this.m_canvasRenderer.Clear();
			}
			this.SetActiveSubMeshes(false);
			LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
			this.RecalculateClipping();
		}

		// Token: 0x06004789 RID: 18313 RVA: 0x001459F0 File Offset: 0x00143BF0
		protected override void OnDestroy()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(this.m_canvas, this);
			if (this.m_mesh != null)
			{
				Object.DestroyImmediate(this.m_mesh);
			}
			if (this.m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
				this.m_MaskMaterial = null;
			}
			this.m_isRegisteredForEvents = false;
		}

		// Token: 0x0600478A RID: 18314 RVA: 0x00145A4C File Offset: 0x00143C4C
		protected override void LoadFontAsset()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_fontAsset == null)
			{
				if (TMP_Settings.defaultFontAsset != null)
				{
					this.m_fontAsset = TMP_Settings.defaultFontAsset;
				}
				else
				{
					this.m_fontAsset = (Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TMP_FontAsset)) as TMP_FontAsset);
				}
				if (this.m_fontAsset == null)
				{
					Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".", this);
					return;
				}
				if (this.m_fontAsset.characterDictionary == null)
				{
					Debug.Log("Dictionary is Null!");
				}
				this.m_sharedMaterial = this.m_fontAsset.material;
			}
			else
			{
				if (this.m_fontAsset.characterDictionary == null)
				{
					this.m_fontAsset.ReadFontDefinition();
				}
				if (this.m_sharedMaterial == null && this.m_baseMaterial != null)
				{
					this.m_sharedMaterial = this.m_baseMaterial;
					this.m_baseMaterial = null;
				}
				if (this.m_sharedMaterial == null || this.m_sharedMaterial.mainTexture == null || this.m_fontAsset.atlas.GetInstanceID() != this.m_sharedMaterial.mainTexture.GetInstanceID())
				{
					if (this.m_fontAsset.material == null)
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"The Font Atlas Texture of the Font Asset ",
							this.m_fontAsset.name,
							" assigned to ",
							base.gameObject.name,
							" is missing."
						}), this);
					}
					else
					{
						this.m_sharedMaterial = this.m_fontAsset.material;
					}
				}
			}
			base.GetSpecialCharacters(this.m_fontAsset);
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		// Token: 0x0600478B RID: 18315 RVA: 0x00145C18 File Offset: 0x00143E18
		private Canvas GetCanvas()
		{
			Canvas result = null;
			List<Canvas> list = TMP_ListPool<Canvas>.Get();
			base.gameObject.GetComponentsInParent<Canvas>(false, list);
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].isActiveAndEnabled)
					{
						result = list[i];
						break;
					}
				}
			}
			TMP_ListPool<Canvas>.Release(list);
			return result;
		}

		// Token: 0x0600478C RID: 18316 RVA: 0x00145C74 File Offset: 0x00143E74
		private void UpdateEnvMapMatrix()
		{
			if (!this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) || this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null)
			{
				return;
			}
			Vector3 euler = this.m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
			this.m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
			this.m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, this.m_EnvMapMatrix);
		}

		// Token: 0x0600478D RID: 18317 RVA: 0x00145CF4 File Offset: 0x00143EF4
		private void EnableMasking()
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_canvasRenderer.SetMaterial(this.m_fontMaterial, this.m_sharedMaterial.mainTexture);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_ClipRect))
			{
				this.m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
				this.m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
				this.m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_TEX);
				this.UpdateMask();
			}
			this.m_isMaskingEnabled = true;
		}

		// Token: 0x0600478E RID: 18318 RVA: 0x00145D98 File Offset: 0x00143F98
		private void DisableMasking()
		{
			if (this.m_fontMaterial != null)
			{
				if (this.m_stencilID > 0)
				{
					this.m_sharedMaterial = this.m_MaskMaterial;
				}
				this.m_canvasRenderer.SetMaterial(this.m_sharedMaterial, this.m_sharedMaterial.mainTexture);
				Object.DestroyImmediate(this.m_fontMaterial);
			}
			this.m_isMaskingEnabled = false;
		}

		// Token: 0x0600478F RID: 18319 RVA: 0x00145DF8 File Offset: 0x00143FF8
		private void UpdateMask()
		{
			if (this.m_rectTransform != null)
			{
				if (!ShaderUtilities.isInitialized)
				{
					ShaderUtilities.GetShaderPropertyIDs();
				}
				this.m_isScrollRegionSet = true;
				float num = Mathf.Min(Mathf.Min(this.m_margin.x, this.m_margin.z), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
				float num2 = Mathf.Min(Mathf.Min(this.m_margin.y, this.m_margin.w), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
				num = ((num > 0f) ? num : 0f);
				num2 = ((num2 > 0f) ? num2 : 0f);
				float z = (this.m_rectTransform.rect.width - Mathf.Max(this.m_margin.x, 0f) - Mathf.Max(this.m_margin.z, 0f)) / 2f + num;
				float w = (this.m_rectTransform.rect.height - Mathf.Max(this.m_margin.y, 0f) - Mathf.Max(this.m_margin.w, 0f)) / 2f + num2;
				Vector2 vector = this.m_rectTransform.localPosition + new Vector3((0.5f - this.m_rectTransform.pivot.x) * this.m_rectTransform.rect.width + (Mathf.Max(this.m_margin.x, 0f) - Mathf.Max(this.m_margin.z, 0f)) / 2f, (0.5f - this.m_rectTransform.pivot.y) * this.m_rectTransform.rect.height + (-Mathf.Max(this.m_margin.y, 0f) + Mathf.Max(this.m_margin.w, 0f)) / 2f);
				Vector4 value = new Vector4(vector.x, vector.y, z, w);
				this.m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, value);
			}
		}

		// Token: 0x06004790 RID: 18320 RVA: 0x00146040 File Offset: 0x00144240
		protected override Material GetMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_fontMaterial == null || this.m_fontMaterial.GetInstanceID() != mat.GetInstanceID())
			{
				this.m_fontMaterial = this.CreateMaterialInstance(mat);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.m_ShouldRecalculateStencil = true;
			this.SetVerticesDirty();
			this.SetMaterialDirty();
			return this.m_sharedMaterial;
		}

		// Token: 0x06004791 RID: 18321 RVA: 0x001460B4 File Offset: 0x001442B4
		protected override Material[] GetMaterials(Material[] mats)
		{
			int materialCount = this.m_textInfo.materialCount;
			if (this.m_fontMaterials == null)
			{
				this.m_fontMaterials = new Material[materialCount];
			}
			else if (this.m_fontMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize<Material>(ref this.m_fontMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					this.m_fontMaterials[i] = base.fontMaterial;
				}
				else
				{
					this.m_fontMaterials[i] = this.m_subTextObjects[i].material;
				}
			}
			this.m_fontSharedMaterials = this.m_fontMaterials;
			return this.m_fontMaterials;
		}

		// Token: 0x06004792 RID: 18322 RVA: 0x00146141 File Offset: 0x00144341
		protected override void SetSharedMaterial(Material mat)
		{
			this.m_sharedMaterial = mat;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		// Token: 0x06004793 RID: 18323 RVA: 0x0014615C File Offset: 0x0014435C
		protected override Material[] GetSharedMaterials()
		{
			int materialCount = this.m_textInfo.materialCount;
			if (this.m_fontSharedMaterials == null)
			{
				this.m_fontSharedMaterials = new Material[materialCount];
			}
			else if (this.m_fontSharedMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize<Material>(ref this.m_fontSharedMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					this.m_fontSharedMaterials[i] = this.m_sharedMaterial;
				}
				else
				{
					this.m_fontSharedMaterials[i] = this.m_subTextObjects[i].sharedMaterial;
				}
			}
			return this.m_fontSharedMaterials;
		}

		// Token: 0x06004794 RID: 18324 RVA: 0x001461E0 File Offset: 0x001443E0
		protected override void SetSharedMaterials(Material[] materials)
		{
			int materialCount = this.m_textInfo.materialCount;
			if (this.m_fontSharedMaterials == null)
			{
				this.m_fontSharedMaterials = new Material[materialCount];
			}
			else if (this.m_fontSharedMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize<Material>(ref this.m_fontSharedMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					if (!(materials[i].mainTexture == null) && materials[i].mainTexture.GetInstanceID() == this.m_sharedMaterial.mainTexture.GetInstanceID())
					{
						this.m_sharedMaterial = (this.m_fontSharedMaterials[i] = materials[i]);
						this.m_padding = this.GetPaddingForMaterial(this.m_sharedMaterial);
					}
				}
				else if (!(materials[i].mainTexture == null) && materials[i].mainTexture.GetInstanceID() == this.m_subTextObjects[i].sharedMaterial.mainTexture.GetInstanceID() && this.m_subTextObjects[i].isDefaultMaterial)
				{
					this.m_subTextObjects[i].sharedMaterial = (this.m_fontSharedMaterials[i] = materials[i]);
				}
			}
		}

		// Token: 0x06004795 RID: 18325 RVA: 0x001462FC File Offset: 0x001444FC
		protected override void SetOutlineThickness(float thickness)
		{
			if (this.m_fontMaterial != null && this.m_sharedMaterial.GetInstanceID() != this.m_fontMaterial.GetInstanceID())
			{
				this.m_sharedMaterial = this.m_fontMaterial;
				this.m_canvasRenderer.SetMaterial(this.m_sharedMaterial, this.m_sharedMaterial.mainTexture);
			}
			else if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_sharedMaterial = this.m_fontMaterial;
				this.m_canvasRenderer.SetMaterial(this.m_sharedMaterial, this.m_sharedMaterial.mainTexture);
			}
			thickness = Mathf.Clamp01(thickness);
			this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
			this.m_padding = this.GetPaddingForMaterial();
		}

		// Token: 0x06004796 RID: 18326 RVA: 0x001463C8 File Offset: 0x001445C8
		protected override void SetFaceColor(Color32 color)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.m_sharedMaterial.SetColor(ShaderUtilities.ID_FaceColor, color);
		}

		// Token: 0x06004797 RID: 18327 RVA: 0x00146424 File Offset: 0x00144624
		protected override void SetOutlineColor(Color32 color)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.m_sharedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, color);
		}

		// Token: 0x06004798 RID: 18328 RVA: 0x00146480 File Offset: 0x00144680
		protected override void SetShaderDepth()
		{
			if (this.m_canvas == null || this.m_sharedMaterial == null)
			{
				return;
			}
			if (this.m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || this.m_isOverlay)
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
				return;
			}
			this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4f);
		}

		// Token: 0x06004799 RID: 18329 RVA: 0x001464EA File Offset: 0x001446EA
		protected override void SetCulling()
		{
			if (this.m_isCullingEnabled)
			{
				this.m_canvasRenderer.GetMaterial().SetFloat("_CullMode", 2f);
				return;
			}
			this.m_canvasRenderer.GetMaterial().SetFloat("_CullMode", 0f);
		}

		// Token: 0x0600479A RID: 18330 RVA: 0x00146529 File Offset: 0x00144729
		private void SetPerspectiveCorrection()
		{
			if (this.m_isOrthographic)
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
				return;
			}
			this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
		}

		// Token: 0x0600479B RID: 18331 RVA: 0x00146560 File Offset: 0x00144760
		protected override float GetPaddingForMaterial(Material mat)
		{
			this.m_padding = ShaderUtilities.GetPadding(mat, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_isSDFShader = mat.HasProperty(ShaderUtilities.ID_WeightNormal);
			return this.m_padding;
		}

		// Token: 0x0600479C RID: 18332 RVA: 0x001465B0 File Offset: 0x001447B0
		protected override float GetPaddingForMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_isSDFShader = this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal);
			return this.m_padding;
		}

		// Token: 0x0600479D RID: 18333 RVA: 0x0014660C File Offset: 0x0014480C
		private void SetMeshArrays(int size)
		{
			this.m_textInfo.meshInfo[0].ResizeMeshInfo(size);
			this.m_canvasRenderer.SetMesh(this.m_textInfo.meshInfo[0].mesh);
		}

		// Token: 0x0600479E RID: 18334 RVA: 0x00146648 File Offset: 0x00144848
		protected override int SetArraySizes(int[] chars)
		{
			int num = 0;
			int num2 = 0;
			this.m_totalCharacterCount = 0;
			this.m_isUsingBold = false;
			this.m_isParsingText = false;
			this.tag_NoParsing = false;
			this.m_style = this.m_fontStyle;
			this.m_fontWeightInternal = (((this.m_style & FontStyles.Bold) == FontStyles.Bold) ? 700 : this.m_fontWeight);
			this.m_fontWeightStack.SetDefault(this.m_fontWeightInternal);
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			this.m_materialReferenceStack.SetDefault(new MaterialReference(0, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			this.m_materialReferenceIndexLookup.Clear();
			MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
			if (this.m_textInfo == null)
			{
				this.m_textInfo = new TMP_TextInfo();
			}
			this.m_textElementType = TMP_TextElementType.Character;
			int num3 = 0;
			while (chars[num3] != 0)
			{
				if (this.m_textInfo.characterInfo == null || this.m_totalCharacterCount >= this.m_textInfo.characterInfo.Length)
				{
					TMP_TextInfo.Resize<TMP_CharacterInfo>(ref this.m_textInfo.characterInfo, this.m_totalCharacterCount + 1, true);
				}
				int num4 = chars[num3];
				if (!this.m_isRichText || num4 != 60)
				{
					goto IL_220;
				}
				int currentMaterialIndex = this.m_currentMaterialIndex;
				if (!base.ValidateHtmlTag(chars, num3 + 1, out num))
				{
					goto IL_220;
				}
				num3 = num;
				if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
				{
					this.m_isUsingBold = true;
				}
				if (this.m_textElementType == TMP_TextElementType.Sprite)
				{
					MaterialReference[] materialReferences = this.m_materialReferences;
					int currentMaterialIndex2 = this.m_currentMaterialIndex;
					materialReferences[currentMaterialIndex2].referenceCount = materialReferences[currentMaterialIndex2].referenceCount + 1;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].character = (char)(57344 + this.m_spriteIndex);
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].fontAsset = this.m_currentFontAsset;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].materialReferenceIndex = this.m_currentMaterialIndex;
					this.m_textElementType = TMP_TextElementType.Character;
					this.m_currentMaterialIndex = currentMaterialIndex;
					num2++;
					this.m_totalCharacterCount++;
				}
				IL_853:
				num3++;
				continue;
				IL_220:
				bool flag = false;
				bool isUsingAlternateTypeface = false;
				TMP_FontAsset currentFontAsset = this.m_currentFontAsset;
				Material currentMaterial = this.m_currentMaterial;
				int currentMaterialIndex3 = this.m_currentMaterialIndex;
				if (this.m_textElementType == TMP_TextElementType.Character)
				{
					if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num4))
						{
							num4 = (int)char.ToUpper((char)num4);
						}
					}
					else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num4))
						{
							num4 = (int)char.ToLower((char)num4);
						}
					}
					else if (((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num4))
					{
						num4 = (int)char.ToUpper((char)num4);
					}
				}
				TMP_FontAsset tmp_FontAsset = base.GetFontAssetForWeight(this.m_fontWeightInternal);
				if (tmp_FontAsset != null)
				{
					flag = true;
					isUsingAlternateTypeface = true;
					this.m_currentFontAsset = tmp_FontAsset;
				}
				TMP_Glyph tmp_Glyph;
				if (!this.m_currentFontAsset.characterDictionary.TryGetValue(num4, out tmp_Glyph))
				{
					if (this.m_currentFontAsset.fallbackFontAssets != null && this.m_currentFontAsset.fallbackFontAssets.Count > 0)
					{
						for (int i = 0; i < this.m_currentFontAsset.fallbackFontAssets.Count; i++)
						{
							tmp_FontAsset = this.m_currentFontAsset.fallbackFontAssets[i];
							if (!(tmp_FontAsset == null) && tmp_FontAsset.characterDictionary.TryGetValue(num4, out tmp_Glyph))
							{
								flag = true;
								this.m_currentFontAsset = tmp_FontAsset;
								break;
							}
						}
					}
					if (tmp_Glyph == null && TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
					{
						for (int j = 0; j < TMP_Settings.fallbackFontAssets.Count; j++)
						{
							tmp_FontAsset = TMP_Settings.fallbackFontAssets[j];
							if (!(tmp_FontAsset == null) && tmp_FontAsset.characterDictionary.TryGetValue(num4, out tmp_Glyph))
							{
								flag = true;
								this.m_currentFontAsset = tmp_FontAsset;
								break;
							}
						}
					}
					if (tmp_Glyph == null)
					{
						if (char.IsLower((char)num4))
						{
							if (this.m_currentFontAsset.characterDictionary.TryGetValue((int)char.ToUpper((char)num4), out tmp_Glyph))
							{
								num4 = (chars[num3] = (int)char.ToUpper((char)num4));
							}
						}
						else if (char.IsUpper((char)num4) && this.m_currentFontAsset.characterDictionary.TryGetValue((int)char.ToLower((char)num4), out tmp_Glyph))
						{
							num4 = (chars[num3] = (int)char.ToLower((char)num4));
						}
					}
					if (tmp_Glyph == null)
					{
						int num5 = (TMP_Settings.missingGlyphCharacter == 0) ? 9633 : TMP_Settings.missingGlyphCharacter;
						if (this.m_currentFontAsset.characterDictionary.TryGetValue(num5, out tmp_Glyph))
						{
							if (!TMP_Settings.warningsDisabled)
							{
								Debug.LogWarning("Character with ASCII value of " + num4.ToString() + " was not found in the Font Asset Glyph Table.", this);
							}
							num4 = (chars[num3] = num5);
						}
						else
						{
							if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
							{
								for (int k = 0; k < TMP_Settings.fallbackFontAssets.Count; k++)
								{
									tmp_FontAsset = TMP_Settings.fallbackFontAssets[k];
									if (!(tmp_FontAsset == null) && tmp_FontAsset.characterDictionary.TryGetValue(num5, out tmp_Glyph))
									{
										if (!TMP_Settings.warningsDisabled)
										{
											Debug.LogWarning("Character with ASCII value of " + num4.ToString() + " was not found in the Font Asset Glyph Table.", this);
										}
										num4 = (chars[num3] = num5);
										flag = true;
										this.m_currentFontAsset = tmp_FontAsset;
										break;
									}
								}
							}
							if (tmp_Glyph == null)
							{
								tmp_FontAsset = TMP_Settings.GetFontAsset();
								if (tmp_FontAsset != null && tmp_FontAsset.characterDictionary.TryGetValue(num5, out tmp_Glyph))
								{
									if (!TMP_Settings.warningsDisabled)
									{
										Debug.LogWarning("Character with ASCII value of " + num4.ToString() + " was not found in the Font Asset Glyph Table.", this);
									}
									num4 = (chars[num3] = num5);
									flag = true;
									this.m_currentFontAsset = tmp_FontAsset;
								}
								else
								{
									tmp_FontAsset = TMP_FontAsset.defaultFontAsset;
									if (tmp_FontAsset != null && tmp_FontAsset.characterDictionary.TryGetValue(num5, out tmp_Glyph))
									{
										if (!TMP_Settings.warningsDisabled)
										{
											Debug.LogWarning("Character with ASCII value of " + num4.ToString() + " was not found in the Font Asset Glyph Table.", this);
										}
										num4 = (chars[num3] = num5);
										flag = true;
										this.m_currentFontAsset = tmp_FontAsset;
									}
									else if (this.m_currentFontAsset.characterDictionary.TryGetValue(32, out tmp_Glyph))
									{
										if (!TMP_Settings.warningsDisabled)
										{
											Debug.LogWarning("Character with ASCII value of " + num4.ToString() + " was not found in the Font Asset Glyph Table. It was replaced by a space.", this);
										}
										num4 = (chars[num3] = 32);
									}
								}
							}
						}
					}
				}
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].textElement = tmp_Glyph;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].isUsingAlternateTypeface = isUsingAlternateTypeface;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].character = (char)num4;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].fontAsset = this.m_currentFontAsset;
				if (flag)
				{
					if (TMP_Settings.matchMaterialPreset)
					{
						this.m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(this.m_currentMaterial, this.m_currentFontAsset.material);
					}
					else
					{
						this.m_currentMaterial = this.m_currentFontAsset.material;
					}
					this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
				}
				if (!char.IsWhiteSpace((char)num4))
				{
					if (this.m_materialReferences[this.m_currentMaterialIndex].referenceCount < 16383)
					{
						MaterialReference[] materialReferences2 = this.m_materialReferences;
						int currentMaterialIndex4 = this.m_currentMaterialIndex;
						materialReferences2[currentMaterialIndex4].referenceCount = materialReferences2[currentMaterialIndex4].referenceCount + 1;
					}
					else
					{
						this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(new Material(this.m_currentMaterial), this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
						MaterialReference[] materialReferences3 = this.m_materialReferences;
						int currentMaterialIndex5 = this.m_currentMaterialIndex;
						materialReferences3[currentMaterialIndex5].referenceCount = materialReferences3[currentMaterialIndex5].referenceCount + 1;
					}
				}
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].material = this.m_currentMaterial;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].materialReferenceIndex = this.m_currentMaterialIndex;
				this.m_materialReferences[this.m_currentMaterialIndex].isFallbackMaterial = flag;
				if (flag)
				{
					this.m_materialReferences[this.m_currentMaterialIndex].fallbackMaterial = currentMaterial;
					this.m_currentFontAsset = currentFontAsset;
					this.m_currentMaterial = currentMaterial;
					this.m_currentMaterialIndex = currentMaterialIndex3;
				}
				this.m_totalCharacterCount++;
				goto IL_853;
			}
			if (this.m_isCalculatingPreferredValues)
			{
				this.m_isCalculatingPreferredValues = false;
				this.m_isInputParsingRequired = true;
				return this.m_totalCharacterCount;
			}
			this.m_textInfo.spriteCount = num2;
			int num6 = this.m_textInfo.materialCount = this.m_materialReferenceIndexLookup.Count;
			if (num6 > this.m_textInfo.meshInfo.Length)
			{
				TMP_TextInfo.Resize<TMP_MeshInfo>(ref this.m_textInfo.meshInfo, num6, false);
			}
			if (this.m_textInfo.characterInfo.Length - this.m_totalCharacterCount > 256)
			{
				TMP_TextInfo.Resize<TMP_CharacterInfo>(ref this.m_textInfo.characterInfo, Mathf.Max(this.m_totalCharacterCount + 1, 256), true);
			}
			for (int l = 0; l < num6; l++)
			{
				if (l > 0)
				{
					if (this.m_subTextObjects[l] == null)
					{
						this.m_subTextObjects[l] = TMP_SubMeshUI.AddSubTextObject(this, this.m_materialReferences[l]);
						this.m_textInfo.meshInfo[l].vertices = null;
					}
					if (this.m_rectTransform.pivot != this.m_subTextObjects[l].rectTransform.pivot)
					{
						this.m_subTextObjects[l].rectTransform.pivot = this.m_rectTransform.pivot;
					}
					if (this.m_subTextObjects[l].sharedMaterial == null || this.m_subTextObjects[l].sharedMaterial.GetInstanceID() != this.m_materialReferences[l].material.GetInstanceID())
					{
						bool isDefaultMaterial = this.m_materialReferences[l].isDefaultMaterial;
						this.m_subTextObjects[l].isDefaultMaterial = isDefaultMaterial;
						if (!isDefaultMaterial || this.m_subTextObjects[l].sharedMaterial == null || this.m_subTextObjects[l].sharedMaterial.mainTexture.GetInstanceID() != this.m_materialReferences[l].material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
						{
							this.m_subTextObjects[l].sharedMaterial = this.m_materialReferences[l].material;
							this.m_subTextObjects[l].fontAsset = this.m_materialReferences[l].fontAsset;
							this.m_subTextObjects[l].spriteAsset = this.m_materialReferences[l].spriteAsset;
						}
					}
					if (this.m_materialReferences[l].isFallbackMaterial)
					{
						this.m_subTextObjects[l].fallbackMaterial = this.m_materialReferences[l].material;
						this.m_subTextObjects[l].fallbackSourceMaterial = this.m_materialReferences[l].fallbackMaterial;
					}
				}
				int referenceCount = this.m_materialReferences[l].referenceCount;
				if (this.m_textInfo.meshInfo[l].vertices == null || this.m_textInfo.meshInfo[l].vertices.Length < referenceCount * 4)
				{
					if (this.m_textInfo.meshInfo[l].vertices == null)
					{
						if (l == 0)
						{
							this.m_textInfo.meshInfo[l] = new TMP_MeshInfo(this.m_mesh, referenceCount + 1);
						}
						else
						{
							this.m_textInfo.meshInfo[l] = new TMP_MeshInfo(this.m_subTextObjects[l].mesh, referenceCount + 1);
						}
					}
					else
					{
						this.m_textInfo.meshInfo[l].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.NextPowerOfTwo(referenceCount));
					}
				}
				else if (this.m_textInfo.meshInfo[l].vertices.Length - referenceCount * 4 > 1024)
				{
					this.m_textInfo.meshInfo[l].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.Max(Mathf.NextPowerOfTwo(referenceCount), 256));
				}
			}
			int num7 = num6;
			while (num7 < this.m_subTextObjects.Length && this.m_subTextObjects[num7] != null)
			{
				if (num7 < this.m_textInfo.meshInfo.Length)
				{
					this.m_subTextObjects[num7].canvasRenderer.SetMesh(null);
				}
				num7++;
			}
			return this.m_totalCharacterCount;
		}

		// Token: 0x0600479F RID: 18335 RVA: 0x00147318 File Offset: 0x00145518
		protected override void ComputeMarginSize()
		{
			if (base.rectTransform != null)
			{
				this.m_marginWidth = this.m_rectTransform.rect.width - this.m_margin.x - this.m_margin.z;
				this.m_marginHeight = this.m_rectTransform.rect.height - this.m_margin.y - this.m_margin.w;
				this.m_RectTransformCorners = this.GetTextContainerLocalCorners();
			}
		}

		// Token: 0x060047A0 RID: 18336 RVA: 0x001473A1 File Offset: 0x001455A1
		protected override void OnDidApplyAnimationProperties()
		{
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x060047A1 RID: 18337 RVA: 0x001473B6 File Offset: 0x001455B6
		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			this.m_canvas = base.canvas;
		}

		// Token: 0x060047A2 RID: 18338 RVA: 0x001473CA File Offset: 0x001455CA
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.m_canvas = base.canvas;
			this.ComputeMarginSize();
			this.m_havePropertiesChanged = true;
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x001473EB File Offset: 0x001455EB
		protected override void OnRectTransformDimensionsChange()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.ComputeMarginSize();
			this.UpdateSubObjectPivot();
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x060047A4 RID: 18340 RVA: 0x00147414 File Offset: 0x00145614
		private void LateUpdate()
		{
			if (this.m_rectTransform.hasChanged)
			{
				float y = this.m_rectTransform.lossyScale.y;
				if (!this.m_havePropertiesChanged && y != this.m_previousLossyScaleY && this.m_text != string.Empty && this.m_text != null)
				{
					this.UpdateSDFScale(y);
					this.m_previousLossyScaleY = y;
				}
				this.m_rectTransform.hasChanged = false;
			}
			if (this.m_isUsingLegacyAnimationComponent)
			{
				this.m_havePropertiesChanged = true;
				this.OnPreRenderCanvas();
			}
		}

		// Token: 0x060047A5 RID: 18341 RVA: 0x0014749C File Offset: 0x0014569C
		private void OnPreRenderCanvas()
		{
			if (!this.m_isAwake || (!this.m_ignoreActiveState && !this.IsActive()))
			{
				return;
			}
			if (this.m_canvas == null)
			{
				this.m_canvas = base.canvas;
				if (this.m_canvas == null)
				{
					return;
				}
			}
			this.loopCountA = 0;
			if (this.m_havePropertiesChanged || this.m_isLayoutDirty)
			{
				if (this.checkPaddingRequired)
				{
					this.UpdateMeshPadding();
				}
				if (this.m_isInputParsingRequired || this.m_isTextTruncated)
				{
					base.ParseInputText();
				}
				if (this.m_enableAutoSizing)
				{
					this.m_fontSize = Mathf.Clamp(this.m_fontSize, this.m_fontSizeMin, this.m_fontSizeMax);
				}
				this.m_maxFontSize = this.m_fontSizeMax;
				this.m_minFontSize = this.m_fontSizeMin;
				this.m_lineSpacingDelta = 0f;
				this.m_charWidthAdjDelta = 0f;
				this.m_recursiveCount = 0;
				this.m_isCharacterWrappingEnabled = false;
				this.m_isTextTruncated = false;
				this.m_havePropertiesChanged = false;
				this.m_isLayoutDirty = false;
				this.m_ignoreActiveState = false;
				this.GenerateTextMesh();
			}
		}

		// Token: 0x060047A6 RID: 18342 RVA: 0x001475AC File Offset: 0x001457AC
		protected override void GenerateTextMesh()
		{
			if (this.m_fontAsset == null || this.m_fontAsset.characterDictionary == null)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID().ToString());
				return;
			}
			if (this.m_textInfo != null)
			{
				this.m_textInfo.Clear();
			}
			if (this.m_char_buffer == null || this.m_char_buffer.Length == 0 || this.m_char_buffer[0] == 0)
			{
				this.ClearMesh();
				this.m_preferredWidth = 0f;
				this.m_preferredHeight = 0f;
				TMPro_EventManager.ON_TEXT_CHANGED(this);
				return;
			}
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			this.m_materialReferenceStack.SetDefault(new MaterialReference(0, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			this.m_currentSpriteAsset = this.m_spriteAsset;
			int totalCharacterCount = this.m_totalCharacterCount;
			this.m_fontScale = this.m_fontSize / this.m_currentFontAsset.fontInfo.PointSize;
			float num = this.m_fontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale;
			float num2 = this.m_fontScale;
			this.m_fontScaleMultiplier = 1f;
			this.m_currentFontSize = this.m_fontSize;
			this.m_sizeStack.SetDefault(this.m_currentFontSize);
			this.m_style = this.m_fontStyle;
			this.m_fontWeightInternal = (((this.m_style & FontStyles.Bold) == FontStyles.Bold) ? 700 : this.m_fontWeight);
			this.m_fontWeightStack.SetDefault(this.m_fontWeightInternal);
			this.m_lineJustification = this.m_textAlignment;
			float num3 = 0f;
			float num4 = 1f;
			this.m_baselineOffset = 0f;
			bool flag = false;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			bool flag2 = false;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			this.m_fontColor32 = this.m_fontColor;
			this.m_htmlColor = this.m_fontColor32;
			this.m_colorStack.SetDefault(this.m_htmlColor);
			this.m_styleStack.Clear();
			this.m_actionStack.Clear();
			this.m_lineOffset = 0f;
			this.m_lineHeight = 0f;
			float num5 = this.m_currentFontAsset.fontInfo.LineHeight - (this.m_currentFontAsset.fontInfo.Ascender - this.m_currentFontAsset.fontInfo.Descender);
			this.m_cSpacing = 0f;
			this.m_monoSpacing = 0f;
			this.m_xAdvance = 0f;
			this.tag_LineIndent = 0f;
			this.tag_Indent = 0f;
			this.m_indentStack.SetDefault(0f);
			this.tag_NoParsing = false;
			this.m_characterCount = 0;
			this.m_firstCharacterOfLine = 0;
			this.m_lastCharacterOfLine = 0;
			this.m_firstVisibleCharacterOfLine = 0;
			this.m_lastVisibleCharacterOfLine = 0;
			this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
			this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
			this.m_lineNumber = 0;
			this.m_lineVisibleCharacterCount = 0;
			bool flag3 = true;
			this.m_pageNumber = 0;
			int num6 = Mathf.Clamp(this.m_pageToDisplay - 1, 0, this.m_textInfo.pageInfo.Length - 1);
			int num7 = 0;
			Vector4 margin = this.m_margin;
			float marginWidth = this.m_marginWidth;
			float marginHeight = this.m_marginHeight;
			this.m_marginLeft = 0f;
			this.m_marginRight = 0f;
			this.m_width = -1f;
			float num8 = marginWidth + 0.0001f - this.m_marginLeft - this.m_marginRight;
			this.m_meshExtents.min = TMP_Text.k_LargePositiveVector2;
			this.m_meshExtents.max = TMP_Text.k_LargeNegativeVector2;
			this.m_textInfo.ClearLineInfo();
			this.m_maxCapHeight = 0f;
			this.m_maxAscender = 0f;
			this.m_maxDescender = 0f;
			float num9 = 0f;
			float num10 = 0f;
			bool flag4 = false;
			this.m_isNewPage = false;
			bool flag5 = true;
			bool flag6 = false;
			int num11 = 0;
			this.loopCountA++;
			int num12 = 0;
			int num13 = 0;
			while (this.m_char_buffer[num13] != 0)
			{
				int num14 = this.m_char_buffer[num13];
				this.m_textElementType = TMP_TextElementType.Character;
				this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
				this.m_currentFontAsset = this.m_materialReferences[this.m_currentMaterialIndex].fontAsset;
				int currentMaterialIndex = this.m_currentMaterialIndex;
				if (!this.m_isRichText || num14 != 60)
				{
					goto IL_4A4;
				}
				this.m_isParsingText = true;
				if (!base.ValidateHtmlTag(this.m_char_buffer, num13 + 1, out num12))
				{
					goto IL_4A4;
				}
				num13 = num12;
				if (this.m_textElementType != TMP_TextElementType.Character)
				{
					goto IL_4A4;
				}
				IL_2AA5:
				num13++;
				continue;
				IL_4A4:
				this.m_isParsingText = false;
				bool isUsingAlternateTypeface = this.m_textInfo.characterInfo[this.m_characterCount].isUsingAlternateTypeface;
				float num15 = 1f;
				if (this.m_textElementType == TMP_TextElementType.Character)
				{
					if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num14))
						{
							num14 = (int)char.ToUpper((char)num14);
						}
					}
					else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num14))
						{
							num14 = (int)char.ToLower((char)num14);
						}
					}
					else if (((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num14))
					{
						num15 = 0.8f;
						num14 = (int)char.ToUpper((char)num14);
					}
				}
				if (this.m_textElementType == TMP_TextElementType.Sprite)
				{
					TMP_Sprite tmp_Sprite = this.m_currentSpriteAsset.spriteInfoList[this.m_spriteIndex];
					if (tmp_Sprite == null)
					{
						goto IL_2AA5;
					}
					num14 = 57344 + this.m_spriteIndex;
					this.m_currentFontAsset = this.m_fontAsset;
					float num16 = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale;
					num2 = this.m_fontAsset.fontInfo.Ascender / tmp_Sprite.height * tmp_Sprite.scale * num16;
					this.m_cached_TextElement = tmp_Sprite;
					this.m_textInfo.characterInfo[this.m_characterCount].elementType = TMP_TextElementType.Sprite;
					this.m_textInfo.characterInfo[this.m_characterCount].scale = num16;
					this.m_textInfo.characterInfo[this.m_characterCount].spriteAsset = this.m_currentSpriteAsset;
					this.m_textInfo.characterInfo[this.m_characterCount].fontAsset = this.m_currentFontAsset;
					this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex = this.m_currentMaterialIndex;
					this.m_currentMaterialIndex = currentMaterialIndex;
					num3 = 0f;
				}
				else if (this.m_textElementType == TMP_TextElementType.Character)
				{
					this.m_cached_TextElement = this.m_textInfo.characterInfo[this.m_characterCount].textElement;
					if (this.m_cached_TextElement == null)
					{
						goto IL_2AA5;
					}
					this.m_currentFontAsset = this.m_textInfo.characterInfo[this.m_characterCount].fontAsset;
					this.m_currentMaterial = this.m_textInfo.characterInfo[this.m_characterCount].material;
					this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
					this.m_fontScale = this.m_currentFontSize * num15 / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale;
					num2 = this.m_fontScale * this.m_fontScaleMultiplier * this.m_cached_TextElement.scale;
					this.m_textInfo.characterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
					this.m_textInfo.characterInfo[this.m_characterCount].scale = num2;
					num3 = ((this.m_currentMaterialIndex == 0) ? this.m_padding : this.m_subTextObjects[this.m_currentMaterialIndex].padding);
				}
				float num17 = num2;
				if (num14 == 173)
				{
					num2 = 0f;
				}
				if (this.m_isRightToLeft)
				{
					this.m_xAdvance -= ((this.m_cached_TextElement.xAdvance * num4 + this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
				}
				this.m_textInfo.characterInfo[this.m_characterCount].character = (char)num14;
				this.m_textInfo.characterInfo[this.m_characterCount].pointSize = this.m_currentFontSize;
				this.m_textInfo.characterInfo[this.m_characterCount].color = this.m_htmlColor;
				this.m_textInfo.characterInfo[this.m_characterCount].style = this.m_style;
				this.m_textInfo.characterInfo[this.m_characterCount].index = (short)num13;
				if (this.m_enableKerning && this.m_characterCount >= 1)
				{
					int character = (int)this.m_textInfo.characterInfo[this.m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num14);
					KerningPair kerningPair;
					this.m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out kerningPair);
					if (kerningPair != null)
					{
						this.m_xAdvance += kerningPair.XadvanceOffset * num2;
					}
				}
				float num18 = 0f;
				if (this.m_monoSpacing != 0f)
				{
					num18 = (this.m_monoSpacing / 2f - (this.m_cached_TextElement.width / 2f + this.m_cached_TextElement.xOffset) * num2) * (1f - this.m_charWidthAdjDelta);
					this.m_xAdvance += num18;
				}
				float num19;
				if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((this.m_style & FontStyles.Bold) == FontStyles.Bold || (this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
				{
					num19 = this.m_currentFontAsset.boldStyle * 2f;
					num4 = 1f + this.m_currentFontAsset.boldSpacing * 0.01f;
				}
				else
				{
					num19 = this.m_currentFontAsset.normalStyle * 2f;
					num4 = 1f;
				}
				float baseline = this.m_currentFontAsset.fontInfo.Baseline;
				Vector3 vector;
				vector.x = this.m_xAdvance + (this.m_cached_TextElement.xOffset - num3 - num19) * num2 * (1f - this.m_charWidthAdjDelta);
				vector.y = (baseline + this.m_cached_TextElement.yOffset + num3) * num2 - this.m_lineOffset + this.m_baselineOffset;
				vector.z = 0f;
				Vector3 vector2;
				vector2.x = vector.x;
				vector2.y = vector.y - (this.m_cached_TextElement.height + num3 * 2f) * num2;
				vector2.z = 0f;
				Vector3 vector3;
				vector3.x = vector2.x + (this.m_cached_TextElement.width + num3 * 2f + num19 * 2f) * num2 * (1f - this.m_charWidthAdjDelta);
				vector3.y = vector.y;
				vector3.z = 0f;
				Vector3 vector4;
				vector4.x = vector3.x;
				vector4.y = vector2.y;
				vector4.z = 0f;
				if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((this.m_style & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic))
				{
					float num20 = (float)this.m_currentFontAsset.italicStyle * 0.01f;
					Vector3 b = new Vector3(num20 * ((this.m_cached_TextElement.yOffset + num3 + num19) * num2), 0f, 0f);
					Vector3 b2 = new Vector3(num20 * ((this.m_cached_TextElement.yOffset - this.m_cached_TextElement.height - num3 - num19) * num2), 0f, 0f);
					vector += b;
					vector2 += b2;
					vector3 += b;
					vector4 += b2;
				}
				this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft = vector2;
				this.m_textInfo.characterInfo[this.m_characterCount].topLeft = vector;
				this.m_textInfo.characterInfo[this.m_characterCount].topRight = vector3;
				this.m_textInfo.characterInfo[this.m_characterCount].bottomRight = vector4;
				this.m_textInfo.characterInfo[this.m_characterCount].origin = this.m_xAdvance;
				this.m_textInfo.characterInfo[this.m_characterCount].baseLine = 0f - this.m_lineOffset + this.m_baselineOffset;
				this.m_textInfo.characterInfo[this.m_characterCount].aspectRatio = (vector3.x - vector2.x) / (vector.y - vector2.y);
				float num21 = this.m_currentFontAsset.fontInfo.Ascender * ((this.m_textElementType == TMP_TextElementType.Character) ? num2 : this.m_textInfo.characterInfo[this.m_characterCount].scale) + this.m_baselineOffset;
				this.m_textInfo.characterInfo[this.m_characterCount].ascender = num21 - this.m_lineOffset;
				this.m_maxLineAscender = ((num21 > this.m_maxLineAscender) ? num21 : this.m_maxLineAscender);
				float num22 = this.m_currentFontAsset.fontInfo.Descender * ((this.m_textElementType == TMP_TextElementType.Character) ? num2 : this.m_textInfo.characterInfo[this.m_characterCount].scale) + this.m_baselineOffset;
				float num23 = this.m_textInfo.characterInfo[this.m_characterCount].descender = num22 - this.m_lineOffset;
				this.m_maxLineDescender = ((num22 < this.m_maxLineDescender) ? num22 : this.m_maxLineDescender);
				if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript || (this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					float num24 = (num21 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
					num21 = this.m_maxLineAscender;
					this.m_maxLineAscender = ((num24 > this.m_maxLineAscender) ? num24 : this.m_maxLineAscender);
					float num25 = (num22 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
					num22 = this.m_maxLineDescender;
					this.m_maxLineDescender = ((num25 < this.m_maxLineDescender) ? num25 : this.m_maxLineDescender);
				}
				if (this.m_lineNumber == 0)
				{
					this.m_maxAscender = ((this.m_maxAscender > num21) ? this.m_maxAscender : num21);
					this.m_maxCapHeight = Mathf.Max(this.m_maxCapHeight, this.m_currentFontAsset.fontInfo.CapHeight * num2);
				}
				if (this.m_lineOffset == 0f)
				{
					num9 = ((num9 > num21) ? num9 : num21);
				}
				this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
				if (num14 == 9 || !char.IsWhiteSpace((char)num14) || this.m_textElementType == TMP_TextElementType.Sprite)
				{
					this.m_textInfo.characterInfo[this.m_characterCount].isVisible = true;
					num8 = ((this.m_width != -1f) ? Mathf.Min(marginWidth + 0.0001f - this.m_marginLeft - this.m_marginRight, this.m_width) : (marginWidth + 0.0001f - this.m_marginLeft - this.m_marginRight));
					this.m_textInfo.lineInfo[this.m_lineNumber].marginLeft = this.m_marginLeft;
					if (Mathf.Abs(this.m_xAdvance) + ((!this.m_isRightToLeft) ? this.m_cached_TextElement.xAdvance : 0f) * (1f - this.m_charWidthAdjDelta) * ((num14 != 173) ? num2 : num17) > num8)
					{
						num7 = this.m_characterCount - 1;
						if (base.enableWordWrapping && this.m_characterCount != this.m_firstCharacterOfLine)
						{
							if (num11 == this.m_SavedWordWrapState.previous_WordBreak || flag5)
							{
								if (this.m_enableAutoSizing && this.m_fontSize > this.m_fontSizeMin)
								{
									if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f)
									{
										this.loopCountA = 0;
										this.m_charWidthAdjDelta += 0.01f;
										this.GenerateTextMesh();
										return;
									}
									this.m_maxFontSize = this.m_fontSize;
									this.m_fontSize -= Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
									this.m_fontSize = (float)((int)(Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f + 0.5f)) / 20f;
									if (this.loopCountA > 20)
									{
										return;
									}
									this.GenerateTextMesh();
									return;
								}
								else
								{
									if (!this.m_isCharacterWrappingEnabled)
									{
										this.m_isCharacterWrappingEnabled = true;
									}
									else
									{
										flag6 = true;
									}
									this.m_recursiveCount++;
									if (this.m_recursiveCount > 20)
									{
										goto IL_2AA5;
									}
								}
							}
							num13 = base.RestoreWordWrappingState(ref this.m_SavedWordWrapState);
							num11 = num13;
							if (this.m_char_buffer[num13] == 173)
							{
								this.m_isTextTruncated = true;
								this.m_char_buffer[num13] = 45;
								this.GenerateTextMesh();
								return;
							}
							if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f && !this.m_isNewPage)
							{
								float num26 = this.m_maxLineAscender - this.m_startOfLineAscender;
								this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num26);
								this.m_lineOffset += num26;
								this.m_SavedWordWrapState.lineOffset = this.m_lineOffset;
								this.m_SavedWordWrapState.previousLineAscender = this.m_maxLineAscender;
							}
							this.m_isNewPage = false;
							float num27 = this.m_maxLineAscender - this.m_lineOffset;
							float num28 = this.m_maxLineDescender - this.m_lineOffset;
							this.m_maxDescender = ((this.m_maxDescender < num28) ? this.m_maxDescender : num28);
							if (!flag4)
							{
								num10 = this.m_maxDescender;
							}
							if (this.m_useMaxVisibleDescender && (this.m_characterCount >= this.m_maxVisibleCharacters || this.m_lineNumber >= this.m_maxVisibleLines))
							{
								flag4 = true;
							}
							this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstCharacterOfLine;
							this.m_textInfo.lineInfo[this.m_lineNumber].firstVisibleCharacterIndex = (this.m_firstVisibleCharacterOfLine = ((this.m_firstCharacterOfLine > this.m_firstVisibleCharacterOfLine) ? this.m_firstCharacterOfLine : this.m_firstVisibleCharacterOfLine));
							this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = (this.m_lastCharacterOfLine = ((this.m_characterCount - 1 > 0) ? (this.m_characterCount - 1) : 0));
							this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = (this.m_lastVisibleCharacterOfLine = ((this.m_lastVisibleCharacterOfLine < this.m_firstVisibleCharacterOfLine) ? this.m_firstVisibleCharacterOfLine : this.m_lastVisibleCharacterOfLine));
							this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
							this.m_textInfo.lineInfo[this.m_lineNumber].visibleCharacterCount = this.m_lineVisibleCharacterCount;
							this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num28);
							this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num27);
							this.m_textInfo.lineInfo[this.m_lineNumber].length = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x;
							this.m_textInfo.lineInfo[this.m_lineNumber].width = num8;
							this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 - this.m_cSpacing;
							this.m_textInfo.lineInfo[this.m_lineNumber].baseline = 0f - this.m_lineOffset;
							this.m_textInfo.lineInfo[this.m_lineNumber].ascender = num27;
							this.m_textInfo.lineInfo[this.m_lineNumber].descender = num28;
							this.m_textInfo.lineInfo[this.m_lineNumber].lineHeight = num27 - num28 + num5 * num;
							this.m_firstCharacterOfLine = this.m_characterCount;
							this.m_lineVisibleCharacterCount = 0;
							base.SaveWordWrappingState(ref this.m_SavedLineState, num13, this.m_characterCount - 1);
							this.m_lineNumber++;
							flag3 = true;
							if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
							{
								base.ResizeLineExtents(this.m_lineNumber);
							}
							if (this.m_lineHeight == 0f)
							{
								float num29 = this.m_textInfo.characterInfo[this.m_characterCount].ascender - this.m_textInfo.characterInfo[this.m_characterCount].baseLine;
								float num30 = 0f - this.m_maxLineDescender + num29 + (num5 + this.m_lineSpacing + this.m_lineSpacingDelta) * num;
								this.m_lineOffset += num30;
								this.m_startOfLineAscender = num29;
							}
							else
							{
								this.m_lineOffset += this.m_lineHeight + this.m_lineSpacing * num;
							}
							this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
							this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
							this.m_xAdvance = 0f + this.tag_Indent;
							goto IL_2AA5;
						}
						else if (this.m_enableAutoSizing && this.m_fontSize > this.m_fontSizeMin)
						{
							if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f)
							{
								this.loopCountA = 0;
								this.m_charWidthAdjDelta += 0.01f;
								this.GenerateTextMesh();
								return;
							}
							this.m_maxFontSize = this.m_fontSize;
							this.m_fontSize -= Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
							this.m_fontSize = (float)((int)(Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f + 0.5f)) / 20f;
							this.m_recursiveCount = 0;
							if (this.loopCountA > 20)
							{
								return;
							}
							this.GenerateTextMesh();
							return;
						}
						else
						{
							switch (this.m_overflowMode)
							{
							case TextOverflowModes.Overflow:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								break;
							case TextOverflowModes.Ellipsis:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								this.m_isTextTruncated = true;
								if (this.m_characterCount >= 1)
								{
									this.m_char_buffer[num13 - 1] = 8230;
									this.m_char_buffer[num13] = 0;
									if (this.m_cached_Ellipsis_GlyphInfo != null)
									{
										this.m_textInfo.characterInfo[num7].character = '…';
										this.m_textInfo.characterInfo[num7].textElement = this.m_cached_Ellipsis_GlyphInfo;
										this.m_textInfo.characterInfo[num7].fontAsset = this.m_materialReferences[0].fontAsset;
										this.m_textInfo.characterInfo[num7].material = this.m_materialReferences[0].material;
										this.m_textInfo.characterInfo[num7].materialReferenceIndex = 0;
									}
									else
									{
										Debug.LogWarning("Unable to use Ellipsis character since it wasn't found in the current Font Asset [" + this.m_fontAsset.name + "]. Consider regenerating this font asset to include the Ellipsis character (u+2026).\nNote: Warnings can be disabled in the TMP Settings file.", this);
									}
									this.m_totalCharacterCount = num7 + 1;
									this.GenerateTextMesh();
									return;
								}
								this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
								break;
							case TextOverflowModes.Masking:
								if (!this.m_isMaskingEnabled)
								{
									this.EnableMasking();
								}
								break;
							case TextOverflowModes.Truncate:
								if (this.m_isMaskingEnabled)
								{
									this.DisableMasking();
								}
								this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
								break;
							case TextOverflowModes.ScrollRect:
								if (!this.m_isMaskingEnabled)
								{
									this.EnableMasking();
								}
								break;
							}
						}
					}
					if (num14 != 9)
					{
						Color32 vertexColor;
						if (this.m_overrideHtmlColors)
						{
							vertexColor = this.m_fontColor32;
						}
						else
						{
							vertexColor = this.m_htmlColor;
						}
						if (this.m_textElementType == TMP_TextElementType.Character)
						{
							this.SaveGlyphVertexInfo(num3, num19, vertexColor);
						}
						else if (this.m_textElementType == TMP_TextElementType.Sprite)
						{
							this.SaveSpriteVertexInfo(vertexColor);
						}
					}
					else
					{
						this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
						this.m_lastVisibleCharacterOfLine = this.m_characterCount;
						TMP_LineInfo[] lineInfo = this.m_textInfo.lineInfo;
						int lineNumber = this.m_lineNumber;
						lineInfo[lineNumber].spaceCount = lineInfo[lineNumber].spaceCount + 1;
						this.m_textInfo.spaceCount++;
					}
					if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible && num14 != 173)
					{
						if (flag3)
						{
							flag3 = false;
							this.m_firstVisibleCharacterOfLine = this.m_characterCount;
						}
						this.m_lineVisibleCharacterCount++;
						this.m_lastVisibleCharacterOfLine = this.m_characterCount;
					}
				}
				else if ((num14 == 10 || char.IsSeparator((char)num14)) && num14 != 173 && num14 != 8203 && num14 != 8288)
				{
					TMP_LineInfo[] lineInfo2 = this.m_textInfo.lineInfo;
					int lineNumber2 = this.m_lineNumber;
					lineInfo2[lineNumber2].spaceCount = lineInfo2[lineNumber2].spaceCount + 1;
					this.m_textInfo.spaceCount++;
				}
				if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f && !this.m_isNewPage)
				{
					float num31 = this.m_maxLineAscender - this.m_startOfLineAscender;
					this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num31);
					num23 -= num31;
					this.m_lineOffset += num31;
					this.m_startOfLineAscender += num31;
					this.m_SavedWordWrapState.lineOffset = this.m_lineOffset;
					this.m_SavedWordWrapState.previousLineAscender = this.m_startOfLineAscender;
				}
				this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = (short)this.m_lineNumber;
				this.m_textInfo.characterInfo[this.m_characterCount].pageNumber = (short)this.m_pageNumber;
				if ((num14 != 10 && num14 != 13 && num14 != 8230) || this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1)
				{
					this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
				}
				if (this.m_maxAscender - num23 > marginHeight + 0.0001f)
				{
					if (this.m_enableAutoSizing && this.m_lineSpacingDelta > this.m_lineSpacingMax && this.m_lineNumber > 0)
					{
						this.loopCountA = 0;
						this.m_lineSpacingDelta -= 1f;
						this.GenerateTextMesh();
						return;
					}
					if (this.m_enableAutoSizing && this.m_fontSize > this.m_fontSizeMin)
					{
						this.m_maxFontSize = this.m_fontSize;
						this.m_fontSize -= Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
						this.m_fontSize = (float)((int)(Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f + 0.5f)) / 20f;
						this.m_recursiveCount = 0;
						if (this.loopCountA > 20)
						{
							return;
						}
						this.GenerateTextMesh();
						return;
					}
					else
					{
						switch (this.m_overflowMode)
						{
						case TextOverflowModes.Overflow:
							if (this.m_isMaskingEnabled)
							{
								this.DisableMasking();
							}
							break;
						case TextOverflowModes.Ellipsis:
							if (this.m_isMaskingEnabled)
							{
								this.DisableMasking();
							}
							if (this.m_lineNumber > 0)
							{
								this.m_char_buffer[(int)this.m_textInfo.characterInfo[num7].index] = 8230;
								this.m_char_buffer[(int)(this.m_textInfo.characterInfo[num7].index + 1)] = 0;
								if (this.m_cached_Ellipsis_GlyphInfo != null)
								{
									this.m_textInfo.characterInfo[num7].character = '…';
									this.m_textInfo.characterInfo[num7].textElement = this.m_cached_Ellipsis_GlyphInfo;
									this.m_textInfo.characterInfo[num7].fontAsset = this.m_materialReferences[0].fontAsset;
									this.m_textInfo.characterInfo[num7].material = this.m_materialReferences[0].material;
									this.m_textInfo.characterInfo[num7].materialReferenceIndex = 0;
								}
								else
								{
									Debug.LogWarning("Unable to use Ellipsis character since it wasn't found in the current Font Asset [" + this.m_fontAsset.name + "]. Consider regenerating this font asset to include the Ellipsis character (u+2026).\nNote: Warnings can be disabled in the TMP Settings file.", this);
								}
								this.m_totalCharacterCount = num7 + 1;
								this.GenerateTextMesh();
								this.m_isTextTruncated = true;
								return;
							}
							this.ClearMesh();
							return;
						case TextOverflowModes.Masking:
							if (!this.m_isMaskingEnabled)
							{
								this.EnableMasking();
							}
							break;
						case TextOverflowModes.Truncate:
							if (this.m_isMaskingEnabled)
							{
								this.DisableMasking();
							}
							if (this.m_lineNumber > 0)
							{
								this.m_char_buffer[(int)(this.m_textInfo.characterInfo[num7].index + 1)] = 0;
								this.m_totalCharacterCount = num7 + 1;
								this.GenerateTextMesh();
								this.m_isTextTruncated = true;
								return;
							}
							this.ClearMesh();
							return;
						case TextOverflowModes.ScrollRect:
							if (!this.m_isMaskingEnabled)
							{
								this.EnableMasking();
							}
							break;
						case TextOverflowModes.Page:
							if (this.m_isMaskingEnabled)
							{
								this.DisableMasking();
							}
							if (num14 != 13 && num14 != 10)
							{
								num13 = base.RestoreWordWrappingState(ref this.m_SavedLineState);
								if (num13 == 0)
								{
									this.ClearMesh();
									return;
								}
								this.m_isNewPage = true;
								this.m_xAdvance = 0f + this.tag_Indent;
								this.m_lineOffset = 0f;
								this.m_lineNumber++;
								this.m_pageNumber++;
								goto IL_2AA5;
							}
							break;
						}
					}
				}
				if (num14 == 9)
				{
					float num32 = this.m_currentFontAsset.fontInfo.TabWidth * num2;
					float num33 = Mathf.Ceil(this.m_xAdvance / num32) * num32;
					this.m_xAdvance = ((num33 > this.m_xAdvance) ? num33 : (this.m_xAdvance + num32));
				}
				else if (this.m_monoSpacing != 0f)
				{
					this.m_xAdvance += (this.m_monoSpacing - num18 + (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
				}
				else if (!this.m_isRightToLeft)
				{
					this.m_xAdvance += ((this.m_cached_TextElement.xAdvance * num4 + this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
				}
				this.m_textInfo.characterInfo[this.m_characterCount].xAdvance = this.m_xAdvance;
				if (num14 == 13)
				{
					this.m_xAdvance = 0f + this.tag_Indent;
				}
				if (num14 == 10 || this.m_characterCount == totalCharacterCount - 1)
				{
					if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f && !this.m_isNewPage)
					{
						float num34 = this.m_maxLineAscender - this.m_startOfLineAscender;
						this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num34);
						num23 -= num34;
						this.m_lineOffset += num34;
					}
					this.m_isNewPage = false;
					float num35 = this.m_maxLineAscender - this.m_lineOffset;
					float num36 = this.m_maxLineDescender - this.m_lineOffset;
					this.m_maxDescender = ((this.m_maxDescender < num36) ? this.m_maxDescender : num36);
					if (!flag4)
					{
						num10 = this.m_maxDescender;
					}
					if (this.m_useMaxVisibleDescender && (this.m_characterCount >= this.m_maxVisibleCharacters || this.m_lineNumber >= this.m_maxVisibleLines))
					{
						flag4 = true;
					}
					this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstCharacterOfLine;
					this.m_textInfo.lineInfo[this.m_lineNumber].firstVisibleCharacterIndex = (this.m_firstVisibleCharacterOfLine = ((this.m_firstCharacterOfLine > this.m_firstVisibleCharacterOfLine) ? this.m_firstCharacterOfLine : this.m_firstVisibleCharacterOfLine));
					this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = (this.m_lastCharacterOfLine = this.m_characterCount);
					this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = (this.m_lastVisibleCharacterOfLine = ((this.m_lastVisibleCharacterOfLine < this.m_firstVisibleCharacterOfLine) ? this.m_firstVisibleCharacterOfLine : this.m_lastVisibleCharacterOfLine));
					this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
					this.m_textInfo.lineInfo[this.m_lineNumber].visibleCharacterCount = this.m_lineVisibleCharacterCount;
					this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num36);
					this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num35);
					this.m_textInfo.lineInfo[this.m_lineNumber].length = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - num3 * num2;
					this.m_textInfo.lineInfo[this.m_lineNumber].width = num8;
					if (this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1)
					{
						this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
					}
					if (this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].isVisible)
					{
						this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 - this.m_cSpacing;
					}
					else
					{
						this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastCharacterOfLine].xAdvance - (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 - this.m_cSpacing;
					}
					this.m_textInfo.lineInfo[this.m_lineNumber].baseline = 0f - this.m_lineOffset;
					this.m_textInfo.lineInfo[this.m_lineNumber].ascender = num35;
					this.m_textInfo.lineInfo[this.m_lineNumber].descender = num36;
					this.m_textInfo.lineInfo[this.m_lineNumber].lineHeight = num35 - num36 + num5 * num;
					this.m_firstCharacterOfLine = this.m_characterCount + 1;
					this.m_lineVisibleCharacterCount = 0;
					if (num14 == 10)
					{
						base.SaveWordWrappingState(ref this.m_SavedLineState, num13, this.m_characterCount);
						base.SaveWordWrappingState(ref this.m_SavedWordWrapState, num13, this.m_characterCount);
						this.m_lineNumber++;
						flag3 = true;
						if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
						{
							base.ResizeLineExtents(this.m_lineNumber);
						}
						if (this.m_lineHeight == 0f)
						{
							float num30 = 0f - this.m_maxLineDescender + num21 + (num5 + this.m_lineSpacing + this.m_paragraphSpacing + this.m_lineSpacingDelta) * num;
							this.m_lineOffset += num30;
						}
						else
						{
							this.m_lineOffset += this.m_lineHeight + (this.m_lineSpacing + this.m_paragraphSpacing) * num;
						}
						this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
						this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
						this.m_startOfLineAscender = num21;
						this.m_xAdvance = 0f + this.tag_LineIndent + this.tag_Indent;
						num7 = this.m_characterCount - 1;
						this.m_characterCount++;
						goto IL_2AA5;
					}
				}
				if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
				{
					this.m_meshExtents.min.x = Mathf.Min(this.m_meshExtents.min.x, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.x);
					this.m_meshExtents.min.y = Mathf.Min(this.m_meshExtents.min.y, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.y);
					this.m_meshExtents.max.x = Mathf.Max(this.m_meshExtents.max.x, this.m_textInfo.characterInfo[this.m_characterCount].topRight.x);
					this.m_meshExtents.max.y = Mathf.Max(this.m_meshExtents.max.y, this.m_textInfo.characterInfo[this.m_characterCount].topRight.y);
				}
				if (this.m_overflowMode == TextOverflowModes.Page && num14 != 13 && num14 != 10 && this.m_pageNumber < 16)
				{
					this.m_textInfo.pageInfo[this.m_pageNumber].ascender = num9;
					this.m_textInfo.pageInfo[this.m_pageNumber].descender = ((num22 < this.m_textInfo.pageInfo[this.m_pageNumber].descender) ? num22 : this.m_textInfo.pageInfo[this.m_pageNumber].descender);
					if (this.m_pageNumber == 0 && this.m_characterCount == 0)
					{
						this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
					}
					else if (this.m_characterCount > 0 && this.m_pageNumber != (int)this.m_textInfo.characterInfo[this.m_characterCount - 1].pageNumber)
					{
						this.m_textInfo.pageInfo[this.m_pageNumber - 1].lastCharacterIndex = this.m_characterCount - 1;
						this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
					}
					else if (this.m_characterCount == totalCharacterCount - 1)
					{
						this.m_textInfo.pageInfo[this.m_pageNumber].lastCharacterIndex = this.m_characterCount;
					}
				}
				if (this.m_enableWordWrapping || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis)
				{
					if ((char.IsWhiteSpace((char)num14) || num14 == 45 || num14 == 173) && !this.m_isNonBreakingSpace && num14 != 160 && num14 != 8209 && num14 != 8239 && num14 != 8288)
					{
						base.SaveWordWrappingState(ref this.m_SavedWordWrapState, num13, this.m_characterCount);
						this.m_isCharacterWrappingEnabled = false;
						flag5 = false;
					}
					else if (((num14 > 4352 && num14 < 4607) || (num14 > 11904 && num14 < 40959) || (num14 > 43360 && num14 < 43391) || (num14 > 44032 && num14 < 55295) || (num14 > 63744 && num14 < 64255) || (num14 > 65072 && num14 < 65103) || (num14 > 65280 && num14 < 65519)) && !this.m_isNonBreakingSpace)
					{
						if (flag5 || flag6 || (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num14) && this.m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey((int)this.m_textInfo.characterInfo[this.m_characterCount + 1].character)))
						{
							base.SaveWordWrappingState(ref this.m_SavedWordWrapState, num13, this.m_characterCount);
							this.m_isCharacterWrappingEnabled = false;
							flag5 = false;
						}
					}
					else if (flag5 || this.m_isCharacterWrappingEnabled || flag6)
					{
						base.SaveWordWrappingState(ref this.m_SavedWordWrapState, num13, this.m_characterCount);
					}
				}
				this.m_characterCount++;
				goto IL_2AA5;
			}
			float num37 = this.m_maxFontSize - this.m_minFontSize;
			if (!this.m_isCharacterWrappingEnabled && this.m_enableAutoSizing && num37 > 0.051f && this.m_fontSize < this.m_fontSizeMax)
			{
				this.m_minFontSize = this.m_fontSize;
				this.m_fontSize += Mathf.Max((this.m_maxFontSize - this.m_fontSize) / 2f, 0.05f);
				this.m_fontSize = (float)((int)(Mathf.Min(this.m_fontSize, this.m_fontSizeMax) * 20f + 0.5f)) / 20f;
				if (this.loopCountA > 20)
				{
					return;
				}
				this.GenerateTextMesh();
				return;
			}
			else
			{
				this.m_isCharacterWrappingEnabled = false;
				if (this.m_characterCount == 0)
				{
					this.ClearMesh();
					TMPro_EventManager.ON_TEXT_CHANGED(this);
					return;
				}
				int num38 = this.m_materialReferences[0].referenceCount * 4;
				this.m_textInfo.meshInfo[0].Clear(false);
				Vector3 a = Vector3.zero;
				Vector3[] rectTransformCorners = this.m_RectTransformCorners;
				switch (this.m_textAlignment)
				{
				case TextAlignmentOptions.TopLeft:
				case TextAlignmentOptions.Top:
				case TextAlignmentOptions.TopRight:
				case TextAlignmentOptions.TopJustified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						a = rectTransformCorners[1] + new Vector3(0f + margin.x, 0f - this.m_maxAscender - margin.y, 0f);
					}
					else
					{
						a = rectTransformCorners[1] + new Vector3(0f + margin.x, 0f - this.m_textInfo.pageInfo[num6].ascender - margin.y, 0f);
					}
					break;
				case TextAlignmentOptions.Left:
				case TextAlignmentOptions.Center:
				case TextAlignmentOptions.Right:
				case TextAlignmentOptions.Justified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_maxAscender + margin.y + num10 - margin.w) / 2f, 0f);
					}
					else
					{
						a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_textInfo.pageInfo[num6].ascender + margin.y + this.m_textInfo.pageInfo[num6].descender - margin.w) / 2f, 0f);
					}
					break;
				case TextAlignmentOptions.BottomLeft:
				case TextAlignmentOptions.Bottom:
				case TextAlignmentOptions.BottomRight:
				case TextAlignmentOptions.BottomJustified:
					if (this.m_overflowMode != TextOverflowModes.Page)
					{
						a = rectTransformCorners[0] + new Vector3(0f + margin.x, 0f - num10 + margin.w, 0f);
					}
					else
					{
						a = rectTransformCorners[0] + new Vector3(0f + margin.x, 0f - this.m_textInfo.pageInfo[num6].descender + margin.w, 0f);
					}
					break;
				case TextAlignmentOptions.BaselineLeft:
				case TextAlignmentOptions.Baseline:
				case TextAlignmentOptions.BaselineRight:
				case TextAlignmentOptions.BaselineJustified:
					a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f, 0f);
					break;
				case TextAlignmentOptions.MidlineLeft:
				case TextAlignmentOptions.Midline:
				case TextAlignmentOptions.MidlineRight:
				case TextAlignmentOptions.MidlineJustified:
					a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_meshExtents.max.y + margin.y + this.m_meshExtents.min.y - margin.w) / 2f, 0f);
					break;
				case TextAlignmentOptions.CaplineLeft:
				case TextAlignmentOptions.Capline:
				case TextAlignmentOptions.CaplineRight:
				case TextAlignmentOptions.CaplineJustified:
					a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_maxCapHeight - margin.y - margin.w) / 2f, 0f);
					break;
				}
				Vector3 vector5 = Vector3.zero;
				Vector3 vector6 = Vector3.zero;
				int index_X = 0;
				int index_X2 = 0;
				int num39 = 0;
				int num40 = 0;
				int num41 = 0;
				bool flag7 = false;
				int num42 = 0;
				bool flag8 = !(this.m_canvas.worldCamera == null);
				float num43 = this.m_previousLossyScaleY = base.transform.lossyScale.y;
				RenderMode renderMode = this.m_canvas.renderMode;
				float scaleFactor = this.m_canvas.scaleFactor;
				Color32 underlineColor = Color.white;
				Color32 underlineColor2 = Color.white;
				float num44 = 0f;
				float num45 = 0f;
				float num46 = 0f;
				float num47 = TMP_Text.k_LargePositiveFloat;
				int num48 = 0;
				float num49 = 0f;
				float num50 = 0f;
				float b3 = 0f;
				TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
				for (int i = 0; i < this.m_characterCount; i++)
				{
					char character2 = characterInfo[i].character;
					int lineNumber3 = (int)characterInfo[i].lineNumber;
					TMP_LineInfo tmp_LineInfo = this.m_textInfo.lineInfo[lineNumber3];
					num40 = lineNumber3 + 1;
					switch (tmp_LineInfo.alignment)
					{
					case TextAlignmentOptions.TopLeft:
					case TextAlignmentOptions.Left:
					case TextAlignmentOptions.BottomLeft:
					case TextAlignmentOptions.BaselineLeft:
					case TextAlignmentOptions.MidlineLeft:
					case TextAlignmentOptions.CaplineLeft:
						if (!this.m_isRightToLeft)
						{
							vector5 = new Vector3(0f + tmp_LineInfo.marginLeft, 0f, 0f);
						}
						else
						{
							vector5 = new Vector3(0f - tmp_LineInfo.maxAdvance, 0f, 0f);
						}
						break;
					case TextAlignmentOptions.Top:
					case TextAlignmentOptions.Center:
					case TextAlignmentOptions.Bottom:
					case TextAlignmentOptions.Baseline:
					case TextAlignmentOptions.Midline:
					case TextAlignmentOptions.Capline:
						vector5 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width / 2f - tmp_LineInfo.maxAdvance / 2f, 0f, 0f);
						break;
					case TextAlignmentOptions.TopRight:
					case TextAlignmentOptions.Right:
					case TextAlignmentOptions.BottomRight:
					case TextAlignmentOptions.BaselineRight:
					case TextAlignmentOptions.MidlineRight:
					case TextAlignmentOptions.CaplineRight:
						if (!this.m_isRightToLeft)
						{
							vector5 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width - tmp_LineInfo.maxAdvance, 0f, 0f);
						}
						else
						{
							vector5 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width, 0f, 0f);
						}
						break;
					case TextAlignmentOptions.TopJustified:
					case TextAlignmentOptions.Justified:
					case TextAlignmentOptions.BottomJustified:
					case TextAlignmentOptions.BaselineJustified:
					case TextAlignmentOptions.MidlineJustified:
					case TextAlignmentOptions.CaplineJustified:
						if (character2 != '­' && character2 != '​' && character2 != '⁠')
						{
							if (!char.IsControl(characterInfo[tmp_LineInfo.lastCharacterIndex].character) && lineNumber3 < this.m_lineNumber)
							{
								float num51 = (!this.m_isRightToLeft) ? (tmp_LineInfo.width - tmp_LineInfo.maxAdvance) : (tmp_LineInfo.width + tmp_LineInfo.maxAdvance);
								float num52 = (tmp_LineInfo.spaceCount > 2) ? this.m_wordWrappingRatios : 1f;
								if (lineNumber3 != num41 || i == 0)
								{
									if (!this.m_isRightToLeft)
									{
										vector5 = new Vector3(tmp_LineInfo.marginLeft, 0f, 0f);
									}
									else
									{
										vector5 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width, 0f, 0f);
									}
								}
								else if (character2 == '\t' || char.IsSeparator(character2))
								{
									int num53 = characterInfo[tmp_LineInfo.lastCharacterIndex].isVisible ? tmp_LineInfo.spaceCount : (tmp_LineInfo.spaceCount - 1);
									if (num53 < 1)
									{
										num53 = 1;
									}
									if (!this.m_isRightToLeft)
									{
										vector5 += new Vector3(num51 * (1f - num52) / (float)num53, 0f, 0f);
									}
									else
									{
										vector5 -= new Vector3(num51 * (1f - num52) / (float)num53, 0f, 0f);
									}
								}
								else if (!this.m_isRightToLeft)
								{
									vector5 += new Vector3(num51 * num52 / (float)(tmp_LineInfo.visibleCharacterCount - 1), 0f, 0f);
								}
								else
								{
									vector5 -= new Vector3(num51 * num52 / (float)(tmp_LineInfo.visibleCharacterCount - 1), 0f, 0f);
								}
							}
							else if (!this.m_isRightToLeft)
							{
								vector5 = new Vector3(tmp_LineInfo.marginLeft, 0f, 0f);
							}
							else
							{
								vector5 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width, 0f, 0f);
							}
						}
						break;
					}
					vector6 = a + vector5;
					bool isVisible = characterInfo[i].isVisible;
					if (isVisible)
					{
						TMP_TextElementType elementType = characterInfo[i].elementType;
						if (elementType != TMP_TextElementType.Character)
						{
							if (elementType != TMP_TextElementType.Sprite)
							{
							}
						}
						else
						{
							Extents lineExtents = tmp_LineInfo.lineExtents;
							float num54 = this.m_uvLineOffset * (float)lineNumber3 % 1f + this.m_uvOffset.x;
							switch (this.m_horizontalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[i].vertex_BL.uv2.x = 0f + this.m_uvOffset.x;
								characterInfo[i].vertex_TL.uv2.x = 0f + this.m_uvOffset.x;
								characterInfo[i].vertex_TR.uv2.x = 1f + this.m_uvOffset.x;
								characterInfo[i].vertex_BR.uv2.x = 1f + this.m_uvOffset.x;
								break;
							case TextureMappingOptions.Line:
								if (this.m_textAlignment != TextAlignmentOptions.Justified)
								{
									characterInfo[i].vertex_BL.uv2.x = (characterInfo[i].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num54;
									characterInfo[i].vertex_TL.uv2.x = (characterInfo[i].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num54;
									characterInfo[i].vertex_TR.uv2.x = (characterInfo[i].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num54;
									characterInfo[i].vertex_BR.uv2.x = (characterInfo[i].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num54;
								}
								else
								{
									characterInfo[i].vertex_BL.uv2.x = (characterInfo[i].vertex_BL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
									characterInfo[i].vertex_TL.uv2.x = (characterInfo[i].vertex_TL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
									characterInfo[i].vertex_TR.uv2.x = (characterInfo[i].vertex_TR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
									characterInfo[i].vertex_BR.uv2.x = (characterInfo[i].vertex_BR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
								}
								break;
							case TextureMappingOptions.Paragraph:
								characterInfo[i].vertex_BL.uv2.x = (characterInfo[i].vertex_BL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
								characterInfo[i].vertex_TL.uv2.x = (characterInfo[i].vertex_TL.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
								characterInfo[i].vertex_TR.uv2.x = (characterInfo[i].vertex_TR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
								characterInfo[i].vertex_BR.uv2.x = (characterInfo[i].vertex_BR.position.x + vector5.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num54;
								break;
							case TextureMappingOptions.MatchAspect:
							{
								switch (this.m_verticalMapping)
								{
								case TextureMappingOptions.Character:
									characterInfo[i].vertex_BL.uv2.y = 0f + this.m_uvOffset.y;
									characterInfo[i].vertex_TL.uv2.y = 1f + this.m_uvOffset.y;
									characterInfo[i].vertex_TR.uv2.y = 0f + this.m_uvOffset.y;
									characterInfo[i].vertex_BR.uv2.y = 1f + this.m_uvOffset.y;
									break;
								case TextureMappingOptions.Line:
									characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num54;
									characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num54;
									characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
									characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
									break;
								case TextureMappingOptions.Paragraph:
									characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num54;
									characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num54;
									characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
									characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
									break;
								case TextureMappingOptions.MatchAspect:
									Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
									break;
								}
								float num55 = (1f - (characterInfo[i].vertex_BL.uv2.y + characterInfo[i].vertex_TL.uv2.y) * characterInfo[i].aspectRatio) / 2f;
								characterInfo[i].vertex_BL.uv2.x = characterInfo[i].vertex_BL.uv2.y * characterInfo[i].aspectRatio + num55 + num54;
								characterInfo[i].vertex_TL.uv2.x = characterInfo[i].vertex_BL.uv2.x;
								characterInfo[i].vertex_TR.uv2.x = characterInfo[i].vertex_TL.uv2.y * characterInfo[i].aspectRatio + num55 + num54;
								characterInfo[i].vertex_BR.uv2.x = characterInfo[i].vertex_TR.uv2.x;
								break;
							}
							}
							switch (this.m_verticalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[i].vertex_BL.uv2.y = 0f + this.m_uvOffset.y;
								characterInfo[i].vertex_TL.uv2.y = 1f + this.m_uvOffset.y;
								characterInfo[i].vertex_TR.uv2.y = 1f + this.m_uvOffset.y;
								characterInfo[i].vertex_BR.uv2.y = 0f + this.m_uvOffset.y;
								break;
							case TextureMappingOptions.Line:
								characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - tmp_LineInfo.descender) / (tmp_LineInfo.ascender - tmp_LineInfo.descender) + this.m_uvOffset.y;
								characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - tmp_LineInfo.descender) / (tmp_LineInfo.ascender - tmp_LineInfo.descender) + this.m_uvOffset.y;
								characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
								characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
								break;
							case TextureMappingOptions.Paragraph:
								characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + this.m_uvOffset.y;
								characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + this.m_uvOffset.y;
								characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
								characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
								break;
							case TextureMappingOptions.MatchAspect:
							{
								float num56 = (1f - (characterInfo[i].vertex_BL.uv2.x + characterInfo[i].vertex_TR.uv2.x) / characterInfo[i].aspectRatio) / 2f;
								characterInfo[i].vertex_BL.uv2.y = num56 + characterInfo[i].vertex_BL.uv2.x / characterInfo[i].aspectRatio + this.m_uvOffset.y;
								characterInfo[i].vertex_TL.uv2.y = num56 + characterInfo[i].vertex_TR.uv2.x / characterInfo[i].aspectRatio + this.m_uvOffset.y;
								characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
								characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
								break;
							}
							}
							num44 = characterInfo[i].scale * (1f - this.m_charWidthAdjDelta);
							if (!characterInfo[i].isUsingAlternateTypeface && (characterInfo[i].style & FontStyles.Bold) == FontStyles.Bold)
							{
								num44 *= -1f;
							}
							switch (renderMode)
							{
							case RenderMode.ScreenSpaceOverlay:
								num44 *= num43 / scaleFactor;
								break;
							case RenderMode.ScreenSpaceCamera:
								num44 *= (flag8 ? num43 : 1f);
								break;
							case RenderMode.WorldSpace:
								num44 *= num43;
								break;
							}
							float num57 = characterInfo[i].vertex_BL.uv2.x;
							float num58 = characterInfo[i].vertex_BL.uv2.y;
							float num59 = characterInfo[i].vertex_TR.uv2.x;
							float num60 = characterInfo[i].vertex_TR.uv2.y;
							float num61 = Mathf.Floor(num57);
							float num62 = Mathf.Floor(num58);
							num57 -= num61;
							num59 -= num61;
							num58 -= num62;
							num60 -= num62;
							characterInfo[i].vertex_BL.uv2.x = base.PackUV(num57, num58);
							characterInfo[i].vertex_BL.uv2.y = num44;
							characterInfo[i].vertex_TL.uv2.x = base.PackUV(num57, num60);
							characterInfo[i].vertex_TL.uv2.y = num44;
							characterInfo[i].vertex_TR.uv2.x = base.PackUV(num59, num60);
							characterInfo[i].vertex_TR.uv2.y = num44;
							characterInfo[i].vertex_BR.uv2.x = base.PackUV(num59, num58);
							characterInfo[i].vertex_BR.uv2.y = num44;
						}
						if (i < this.m_maxVisibleCharacters && lineNumber3 < this.m_maxVisibleLines && this.m_overflowMode != TextOverflowModes.Page)
						{
							TMP_CharacterInfo[] array = characterInfo;
							int num63 = i;
							array[num63].vertex_BL.position = array[num63].vertex_BL.position + vector6;
							TMP_CharacterInfo[] array2 = characterInfo;
							int num64 = i;
							array2[num64].vertex_TL.position = array2[num64].vertex_TL.position + vector6;
							TMP_CharacterInfo[] array3 = characterInfo;
							int num65 = i;
							array3[num65].vertex_TR.position = array3[num65].vertex_TR.position + vector6;
							TMP_CharacterInfo[] array4 = characterInfo;
							int num66 = i;
							array4[num66].vertex_BR.position = array4[num66].vertex_BR.position + vector6;
						}
						else if (i < this.m_maxVisibleCharacters && lineNumber3 < this.m_maxVisibleLines && this.m_overflowMode == TextOverflowModes.Page && (int)characterInfo[i].pageNumber == num6)
						{
							TMP_CharacterInfo[] array5 = characterInfo;
							int num67 = i;
							array5[num67].vertex_BL.position = array5[num67].vertex_BL.position + vector6;
							TMP_CharacterInfo[] array6 = characterInfo;
							int num68 = i;
							array6[num68].vertex_TL.position = array6[num68].vertex_TL.position + vector6;
							TMP_CharacterInfo[] array7 = characterInfo;
							int num69 = i;
							array7[num69].vertex_TR.position = array7[num69].vertex_TR.position + vector6;
							TMP_CharacterInfo[] array8 = characterInfo;
							int num70 = i;
							array8[num70].vertex_BR.position = array8[num70].vertex_BR.position + vector6;
						}
						else
						{
							characterInfo[i].vertex_BL.position = Vector3.zero;
							characterInfo[i].vertex_TL.position = Vector3.zero;
							characterInfo[i].vertex_TR.position = Vector3.zero;
							characterInfo[i].vertex_BR.position = Vector3.zero;
						}
						if (elementType == TMP_TextElementType.Character)
						{
							this.FillCharacterVertexBuffers(i, index_X);
						}
						else if (elementType == TMP_TextElementType.Sprite)
						{
							this.FillSpriteVertexBuffers(i, index_X2);
						}
					}
					TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
					int num71 = i;
					characterInfo2[num71].bottomLeft = characterInfo2[num71].bottomLeft + vector6;
					TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
					int num72 = i;
					characterInfo3[num72].topLeft = characterInfo3[num72].topLeft + vector6;
					TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
					int num73 = i;
					characterInfo4[num73].topRight = characterInfo4[num73].topRight + vector6;
					TMP_CharacterInfo[] characterInfo5 = this.m_textInfo.characterInfo;
					int num74 = i;
					characterInfo5[num74].bottomRight = characterInfo5[num74].bottomRight + vector6;
					TMP_CharacterInfo[] characterInfo6 = this.m_textInfo.characterInfo;
					int num75 = i;
					characterInfo6[num75].origin = characterInfo6[num75].origin + vector6.x;
					TMP_CharacterInfo[] characterInfo7 = this.m_textInfo.characterInfo;
					int num76 = i;
					characterInfo7[num76].xAdvance = characterInfo7[num76].xAdvance + vector6.x;
					TMP_CharacterInfo[] characterInfo8 = this.m_textInfo.characterInfo;
					int num77 = i;
					characterInfo8[num77].ascender = characterInfo8[num77].ascender + vector6.y;
					TMP_CharacterInfo[] characterInfo9 = this.m_textInfo.characterInfo;
					int num78 = i;
					characterInfo9[num78].descender = characterInfo9[num78].descender + vector6.y;
					TMP_CharacterInfo[] characterInfo10 = this.m_textInfo.characterInfo;
					int num79 = i;
					characterInfo10[num79].baseLine = characterInfo10[num79].baseLine + vector6.y;
					if (lineNumber3 != num41 || i == this.m_characterCount - 1)
					{
						if (lineNumber3 != num41)
						{
							TMP_LineInfo[] lineInfo3 = this.m_textInfo.lineInfo;
							int num80 = num41;
							lineInfo3[num80].baseline = lineInfo3[num80].baseline + vector6.y;
							TMP_LineInfo[] lineInfo4 = this.m_textInfo.lineInfo;
							int num81 = num41;
							lineInfo4[num81].ascender = lineInfo4[num81].ascender + vector6.y;
							TMP_LineInfo[] lineInfo5 = this.m_textInfo.lineInfo;
							int num82 = num41;
							lineInfo5[num82].descender = lineInfo5[num82].descender + vector6.y;
							this.m_textInfo.lineInfo[num41].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num41].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[num41].descender);
							this.m_textInfo.lineInfo[num41].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num41].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[num41].ascender);
						}
						if (i == this.m_characterCount - 1)
						{
							TMP_LineInfo[] lineInfo6 = this.m_textInfo.lineInfo;
							int num83 = lineNumber3;
							lineInfo6[num83].baseline = lineInfo6[num83].baseline + vector6.y;
							TMP_LineInfo[] lineInfo7 = this.m_textInfo.lineInfo;
							int num84 = lineNumber3;
							lineInfo7[num84].ascender = lineInfo7[num84].ascender + vector6.y;
							TMP_LineInfo[] lineInfo8 = this.m_textInfo.lineInfo;
							int num85 = lineNumber3;
							lineInfo8[num85].descender = lineInfo8[num85].descender + vector6.y;
							this.m_textInfo.lineInfo[lineNumber3].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber3].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[lineNumber3].descender);
							this.m_textInfo.lineInfo[lineNumber3].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber3].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[lineNumber3].ascender);
						}
					}
					if (char.IsLetterOrDigit(character2) || character2 == '-' || character2 == '­' || character2 == '‐' || character2 == '‑')
					{
						if (!flag7)
						{
							flag7 = true;
							num42 = i;
						}
						if (flag7 && i == this.m_characterCount - 1)
						{
							int num86 = this.m_textInfo.wordInfo.Length;
							int wordCount = this.m_textInfo.wordCount;
							if (this.m_textInfo.wordCount + 1 > num86)
							{
								TMP_TextInfo.Resize<TMP_WordInfo>(ref this.m_textInfo.wordInfo, num86 + 1);
							}
							int num87 = i;
							this.m_textInfo.wordInfo[wordCount].firstCharacterIndex = num42;
							this.m_textInfo.wordInfo[wordCount].lastCharacterIndex = num87;
							this.m_textInfo.wordInfo[wordCount].characterCount = num87 - num42 + 1;
							this.m_textInfo.wordInfo[wordCount].textComponent = this;
							num39++;
							this.m_textInfo.wordCount++;
							TMP_LineInfo[] lineInfo9 = this.m_textInfo.lineInfo;
							int num88 = lineNumber3;
							lineInfo9[num88].wordCount = lineInfo9[num88].wordCount + 1;
						}
					}
					else if ((flag7 || (i == 0 && (!char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || i == this.m_characterCount - 1))) && (i <= 0 || i >= characterInfo.Length - 1 || i >= this.m_characterCount || (character2 != '\'' && character2 != '’') || !char.IsLetterOrDigit(characterInfo[i - 1].character) || !char.IsLetterOrDigit(characterInfo[i + 1].character)))
					{
						int num87 = (i == this.m_characterCount - 1 && char.IsLetterOrDigit(character2)) ? i : (i - 1);
						flag7 = false;
						int num89 = this.m_textInfo.wordInfo.Length;
						int wordCount2 = this.m_textInfo.wordCount;
						if (this.m_textInfo.wordCount + 1 > num89)
						{
							TMP_TextInfo.Resize<TMP_WordInfo>(ref this.m_textInfo.wordInfo, num89 + 1);
						}
						this.m_textInfo.wordInfo[wordCount2].firstCharacterIndex = num42;
						this.m_textInfo.wordInfo[wordCount2].lastCharacterIndex = num87;
						this.m_textInfo.wordInfo[wordCount2].characterCount = num87 - num42 + 1;
						this.m_textInfo.wordInfo[wordCount2].textComponent = this;
						num39++;
						this.m_textInfo.wordCount++;
						TMP_LineInfo[] lineInfo10 = this.m_textInfo.lineInfo;
						int num90 = lineNumber3;
						lineInfo10[num90].wordCount = lineInfo10[num90].wordCount + 1;
					}
					if ((this.m_textInfo.characterInfo[i].style & FontStyles.Underline) == FontStyles.Underline)
					{
						bool flag9 = true;
						int pageNumber = (int)this.m_textInfo.characterInfo[i].pageNumber;
						if (i > this.m_maxVisibleCharacters || lineNumber3 > this.m_maxVisibleLines || (this.m_overflowMode == TextOverflowModes.Page && pageNumber + 1 != this.m_pageToDisplay))
						{
							flag9 = false;
						}
						if (!char.IsWhiteSpace(character2))
						{
							num46 = Mathf.Max(num46, this.m_textInfo.characterInfo[i].scale);
							num47 = Mathf.Min((pageNumber == num48) ? num47 : TMP_Text.k_LargePositiveFloat, this.m_textInfo.characterInfo[i].baseLine + base.font.fontInfo.Underline * num46);
							num48 = pageNumber;
						}
						if (!flag && flag9 && i <= tmp_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (i != tmp_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character2)))
						{
							flag = true;
							num45 = this.m_textInfo.characterInfo[i].scale;
							if (num46 == 0f)
							{
								num46 = num45;
							}
							zero = new Vector3(this.m_textInfo.characterInfo[i].bottomLeft.x, num47, 0f);
							underlineColor = this.m_textInfo.characterInfo[i].color;
						}
						if (flag && this.m_characterCount == 1)
						{
							flag = false;
							zero2 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, num47, 0f);
							float scale = this.m_textInfo.characterInfo[i].scale;
							this.DrawUnderlineMesh(zero, zero2, ref num38, num45, scale, num46, num44, underlineColor);
							num46 = 0f;
							num47 = TMP_Text.k_LargePositiveFloat;
						}
						else if (flag && (i == tmp_LineInfo.lastCharacterIndex || i >= tmp_LineInfo.lastVisibleCharacterIndex))
						{
							float scale;
							if (char.IsWhiteSpace(character2))
							{
								int lastVisibleCharacterIndex = tmp_LineInfo.lastVisibleCharacterIndex;
								zero2 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, num47, 0f);
								scale = this.m_textInfo.characterInfo[lastVisibleCharacterIndex].scale;
							}
							else
							{
								zero2 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, num47, 0f);
								scale = this.m_textInfo.characterInfo[i].scale;
							}
							flag = false;
							this.DrawUnderlineMesh(zero, zero2, ref num38, num45, scale, num46, num44, underlineColor);
							num46 = 0f;
							num47 = TMP_Text.k_LargePositiveFloat;
						}
						else if (flag && !flag9)
						{
							flag = false;
							zero2 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, num47, 0f);
							float scale = this.m_textInfo.characterInfo[i - 1].scale;
							this.DrawUnderlineMesh(zero, zero2, ref num38, num45, scale, num46, num44, underlineColor);
							num46 = 0f;
							num47 = TMP_Text.k_LargePositiveFloat;
						}
					}
					else if (flag)
					{
						flag = false;
						zero2 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, num47, 0f);
						float scale = this.m_textInfo.characterInfo[i - 1].scale;
						this.DrawUnderlineMesh(zero, zero2, ref num38, num45, scale, num46, num44, underlineColor);
						num46 = 0f;
						num47 = TMP_Text.k_LargePositiveFloat;
					}
					if ((this.m_textInfo.characterInfo[i].style & FontStyles.Strikethrough) == FontStyles.Strikethrough)
					{
						bool flag10 = true;
						if (i > this.m_maxVisibleCharacters || lineNumber3 > this.m_maxVisibleLines || (this.m_overflowMode == TextOverflowModes.Page && (int)(this.m_textInfo.characterInfo[i].pageNumber + 1) != this.m_pageToDisplay))
						{
							flag10 = false;
						}
						if (!flag2 && flag10 && i <= tmp_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (i != tmp_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character2)))
						{
							flag2 = true;
							num49 = this.m_textInfo.characterInfo[i].pointSize;
							num50 = this.m_textInfo.characterInfo[i].scale;
							zero3 = new Vector3(this.m_textInfo.characterInfo[i].bottomLeft.x, this.m_textInfo.characterInfo[i].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2.75f * num50, 0f);
							underlineColor2 = this.m_textInfo.characterInfo[i].color;
							b3 = this.m_textInfo.characterInfo[i].baseLine;
						}
						if (flag2 && this.m_characterCount == 1)
						{
							flag2 = false;
							zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num50, 0f);
							this.DrawUnderlineMesh(zero3, zero4, ref num38, num50, num50, num50, num44, underlineColor2);
						}
						else if (flag2 && i == tmp_LineInfo.lastCharacterIndex)
						{
							if (char.IsWhiteSpace(character2))
							{
								int lastVisibleCharacterIndex2 = tmp_LineInfo.lastVisibleCharacterIndex;
								zero4 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex2].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex2].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num50, 0f);
							}
							else
							{
								zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num50, 0f);
							}
							flag2 = false;
							this.DrawUnderlineMesh(zero3, zero4, ref num38, num50, num50, num50, num44, underlineColor2);
						}
						else if (flag2 && i < this.m_characterCount && (this.m_textInfo.characterInfo[i + 1].pointSize != num49 || !TMP_Math.Approximately(this.m_textInfo.characterInfo[i + 1].baseLine + vector6.y, b3)))
						{
							flag2 = false;
							int lastVisibleCharacterIndex3 = tmp_LineInfo.lastVisibleCharacterIndex;
							if (i > lastVisibleCharacterIndex3)
							{
								zero4 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex3].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex3].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num50, 0f);
							}
							else
							{
								zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num50, 0f);
							}
							this.DrawUnderlineMesh(zero3, zero4, ref num38, num50, num50, num50, num44, underlineColor2);
						}
						else if (flag2 && !flag10)
						{
							flag2 = false;
							zero4 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, this.m_textInfo.characterInfo[i - 1].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num50, 0f);
							this.DrawUnderlineMesh(zero3, zero4, ref num38, num50, num50, num50, num44, underlineColor2);
						}
					}
					else if (flag2)
					{
						flag2 = false;
						zero4 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, this.m_textInfo.characterInfo[i - 1].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * this.m_fontScale, 0f);
						this.DrawUnderlineMesh(zero3, zero4, ref num38, num50, num50, num50, num44, underlineColor2);
					}
					num41 = lineNumber3;
				}
				this.m_textInfo.characterCount = (int)((short)this.m_characterCount);
				this.m_textInfo.spriteCount = this.m_spriteCount;
				this.m_textInfo.lineCount = (int)((short)num40);
				this.m_textInfo.wordCount = (int)((num39 != 0 && this.m_characterCount > 0) ? ((short)num39) : 1);
				this.m_textInfo.pageCount = this.m_pageNumber + 1;
				if (this.m_renderMode == TextRenderFlags.Render)
				{
					this.m_mesh.MarkDynamic();
					this.m_mesh.vertices = this.m_textInfo.meshInfo[0].vertices;
					this.m_mesh.uv = this.m_textInfo.meshInfo[0].uvs0;
					this.m_mesh.uv2 = this.m_textInfo.meshInfo[0].uvs2;
					this.m_mesh.colors32 = this.m_textInfo.meshInfo[0].colors32;
					this.m_mesh.RecalculateBounds();
					this.m_canvasRenderer.SetMesh(this.m_mesh);
					for (int j = 1; j < this.m_textInfo.materialCount; j++)
					{
						this.m_textInfo.meshInfo[j].ClearUnusedVertices();
						if (!(this.m_subTextObjects[j] == null))
						{
							this.m_subTextObjects[j].mesh.MarkDynamic();
							this.m_subTextObjects[j].mesh.vertices = this.m_textInfo.meshInfo[j].vertices;
							this.m_subTextObjects[j].mesh.uv = this.m_textInfo.meshInfo[j].uvs0;
							this.m_subTextObjects[j].mesh.uv2 = this.m_textInfo.meshInfo[j].uvs2;
							this.m_subTextObjects[j].mesh.colors32 = this.m_textInfo.meshInfo[j].colors32;
							this.m_subTextObjects[j].mesh.RecalculateBounds();
							this.m_subTextObjects[j].canvasRenderer.SetMesh(this.m_subTextObjects[j].mesh);
						}
					}
				}
				TMPro_EventManager.ON_TEXT_CHANGED(this);
				return;
			}
		}

		// Token: 0x060047A7 RID: 18343 RVA: 0x0014CF80 File Offset: 0x0014B180
		protected override Vector3[] GetTextContainerLocalCorners()
		{
			if (this.m_rectTransform == null)
			{
				this.m_rectTransform = base.rectTransform;
			}
			this.m_rectTransform.GetLocalCorners(this.m_RectTransformCorners);
			return this.m_RectTransformCorners;
		}

		// Token: 0x060047A8 RID: 18344 RVA: 0x0014CFB4 File Offset: 0x0014B1B4
		private void ClearMesh()
		{
			this.m_canvasRenderer.SetMesh(null);
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				this.m_subTextObjects[num].canvasRenderer.SetMesh(null);
				num++;
			}
		}

		// Token: 0x060047A9 RID: 18345 RVA: 0x0014D004 File Offset: 0x0014B204
		protected override void SetActiveSubMeshes(bool state)
		{
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				if (this.m_subTextObjects[num].enabled != state)
				{
					this.m_subTextObjects[num].enabled = state;
				}
				num++;
			}
		}

		// Token: 0x060047AA RID: 18346 RVA: 0x0014D054 File Offset: 0x0014B254
		protected override Bounds GetCompoundBounds()
		{
			Bounds bounds = this.m_mesh.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				Bounds bounds2 = this.m_subTextObjects[num].mesh.bounds;
				vector.x = ((vector.x < bounds2.min.x) ? vector.x : bounds2.min.x);
				vector.y = ((vector.y < bounds2.min.y) ? vector.y : bounds2.min.y);
				vector2.x = ((vector2.x > bounds2.max.x) ? vector2.x : bounds2.max.x);
				vector2.y = ((vector2.y > bounds2.max.y) ? vector2.y : bounds2.max.y);
				num++;
			}
			Vector2 v = (vector + vector2) / 2f;
			Vector2 v2 = vector2 - vector;
			return new Bounds(v, v2);
		}

		// Token: 0x060047AB RID: 18347 RVA: 0x0014D1B0 File Offset: 0x0014B3B0
		private void UpdateSDFScale(float lossyScale)
		{
			lossyScale = ((lossyScale == 0f) ? 1f : lossyScale);
			float scaleFactor = this.m_canvas.scaleFactor;
			float num;
			if (this.m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				num = lossyScale / scaleFactor;
			}
			else if (this.m_canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				num = ((this.m_canvas.worldCamera != null) ? lossyScale : 1f);
			}
			else
			{
				num = lossyScale;
			}
			for (int i = 0; i < this.m_textInfo.characterCount; i++)
			{
				if (this.m_textInfo.characterInfo[i].isVisible && this.m_textInfo.characterInfo[i].elementType == TMP_TextElementType.Character)
				{
					float num2 = num * this.m_textInfo.characterInfo[i].scale * (1f - this.m_charWidthAdjDelta);
					if (!this.m_textInfo.characterInfo[i].isUsingAlternateTypeface && (this.m_textInfo.characterInfo[i].style & FontStyles.Bold) == FontStyles.Bold)
					{
						num2 *= -1f;
					}
					int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
					int vertexIndex = this.m_textInfo.characterInfo[i].vertexIndex;
					this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex].y = num2;
					this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 1].y = num2;
					this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 2].y = num2;
					this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 3].y = num2;
				}
			}
			for (int j = 0; j < this.m_textInfo.materialCount; j++)
			{
				if (j == 0)
				{
					this.m_mesh.uv2 = this.m_textInfo.meshInfo[0].uvs2;
					this.m_canvasRenderer.SetMesh(this.m_mesh);
				}
				else
				{
					this.m_subTextObjects[j].mesh.uv2 = this.m_textInfo.meshInfo[j].uvs2;
					this.m_subTextObjects[j].canvasRenderer.SetMesh(this.m_subTextObjects[j].mesh);
				}
			}
		}

		// Token: 0x060047AC RID: 18348 RVA: 0x0014D440 File Offset: 0x0014B640
		protected override void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 vector = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
				int num = i;
				characterInfo[num].bottomLeft = characterInfo[num].bottomLeft - vector;
				TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
				int num2 = i;
				characterInfo2[num2].topLeft = characterInfo2[num2].topLeft - vector;
				TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
				int num3 = i;
				characterInfo3[num3].topRight = characterInfo3[num3].topRight - vector;
				TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
				int num4 = i;
				characterInfo4[num4].bottomRight = characterInfo4[num4].bottomRight - vector;
				TMP_CharacterInfo[] characterInfo5 = this.m_textInfo.characterInfo;
				int num5 = i;
				characterInfo5[num5].ascender = characterInfo5[num5].ascender - vector.y;
				TMP_CharacterInfo[] characterInfo6 = this.m_textInfo.characterInfo;
				int num6 = i;
				characterInfo6[num6].baseLine = characterInfo6[num6].baseLine - vector.y;
				TMP_CharacterInfo[] characterInfo7 = this.m_textInfo.characterInfo;
				int num7 = i;
				characterInfo7[num7].descender = characterInfo7[num7].descender - vector.y;
				if (this.m_textInfo.characterInfo[i].isVisible)
				{
					TMP_CharacterInfo[] characterInfo8 = this.m_textInfo.characterInfo;
					int num8 = i;
					characterInfo8[num8].vertex_BL.position = characterInfo8[num8].vertex_BL.position - vector;
					TMP_CharacterInfo[] characterInfo9 = this.m_textInfo.characterInfo;
					int num9 = i;
					characterInfo9[num9].vertex_TL.position = characterInfo9[num9].vertex_TL.position - vector;
					TMP_CharacterInfo[] characterInfo10 = this.m_textInfo.characterInfo;
					int num10 = i;
					characterInfo10[num10].vertex_TR.position = characterInfo10[num10].vertex_TR.position - vector;
					TMP_CharacterInfo[] characterInfo11 = this.m_textInfo.characterInfo;
					int num11 = i;
					characterInfo11[num11].vertex_BR.position = characterInfo11[num11].vertex_BR.position - vector;
				}
			}
		}

		// Token: 0x04004767 RID: 18279
		private bool m_isRebuildingLayout;

		// Token: 0x04004768 RID: 18280
		[SerializeField]
		private Vector2 m_uvOffset = Vector2.zero;

		// Token: 0x04004769 RID: 18281
		[SerializeField]
		private float m_uvLineOffset;

		// Token: 0x0400476A RID: 18282
		[SerializeField]
		private bool m_hasFontAssetChanged;

		// Token: 0x0400476B RID: 18283
		[SerializeField]
		protected TMP_SubMeshUI[] m_subTextObjects = new TMP_SubMeshUI[16];

		// Token: 0x0400476C RID: 18284
		private float m_previousLossyScaleY = -1f;

		// Token: 0x0400476D RID: 18285
		private Vector3[] m_RectTransformCorners = new Vector3[4];

		// Token: 0x0400476E RID: 18286
		private CanvasRenderer m_canvasRenderer;

		// Token: 0x0400476F RID: 18287
		private Canvas m_canvas;

		// Token: 0x04004770 RID: 18288
		private bool m_isFirstAllocation;

		// Token: 0x04004771 RID: 18289
		private int m_max_characters = 8;

		// Token: 0x04004772 RID: 18290
		private WordWrapState m_SavedWordWrapState;

		// Token: 0x04004773 RID: 18291
		private WordWrapState m_SavedLineState;

		// Token: 0x04004774 RID: 18292
		private bool m_isMaskingEnabled;

		// Token: 0x04004775 RID: 18293
		[SerializeField]
		private Material m_baseMaterial;

		// Token: 0x04004776 RID: 18294
		private bool m_isScrollRegionSet;

		// Token: 0x04004777 RID: 18295
		private int m_stencilID;

		// Token: 0x04004778 RID: 18296
		[SerializeField]
		private Vector4 m_maskOffset;

		// Token: 0x04004779 RID: 18297
		private Matrix4x4 m_EnvMapMatrix;

		// Token: 0x0400477A RID: 18298
		[NonSerialized]
		private bool m_isRegisteredForEvents;

		// Token: 0x0400477B RID: 18299
		private int m_recursiveCount;

		// Token: 0x0400477C RID: 18300
		private int m_recursiveCountA;

		// Token: 0x0400477D RID: 18301
		private int loopCountA;
	}
}
