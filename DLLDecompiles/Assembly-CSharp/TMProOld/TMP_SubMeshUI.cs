using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x0200081D RID: 2077
	[ExecuteInEditMode]
	public class TMP_SubMeshUI : MaskableGraphic, ITextElement, IClippable, IMaskable, IMaterialModifier
	{
		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x0600496C RID: 18796 RVA: 0x00156DBB File Offset: 0x00154FBB
		// (set) Token: 0x0600496D RID: 18797 RVA: 0x00156DC3 File Offset: 0x00154FC3
		public TMP_FontAsset fontAsset
		{
			get
			{
				return this.m_fontAsset;
			}
			set
			{
				this.m_fontAsset = value;
			}
		}

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x0600496E RID: 18798 RVA: 0x00156DCC File Offset: 0x00154FCC
		// (set) Token: 0x0600496F RID: 18799 RVA: 0x00156DD4 File Offset: 0x00154FD4
		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return this.m_spriteAsset;
			}
			set
			{
				this.m_spriteAsset = value;
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x06004970 RID: 18800 RVA: 0x00156DDD File Offset: 0x00154FDD
		public override Texture mainTexture
		{
			get
			{
				if (this.sharedMaterial != null)
				{
					return this.sharedMaterial.mainTexture;
				}
				return null;
			}
		}

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x06004971 RID: 18801 RVA: 0x00156DFA File Offset: 0x00154FFA
		// (set) Token: 0x06004972 RID: 18802 RVA: 0x00156E08 File Offset: 0x00155008
		public override Material material
		{
			get
			{
				return this.GetMaterial(this.m_sharedMaterial);
			}
			set
			{
				if (this.m_sharedMaterial != null && this.m_sharedMaterial.GetInstanceID() == value.GetInstanceID())
				{
					return;
				}
				this.m_material = value;
				this.m_sharedMaterial = value;
				this.m_padding = this.GetPaddingForMaterial();
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x06004973 RID: 18803 RVA: 0x00156E5F File Offset: 0x0015505F
		// (set) Token: 0x06004974 RID: 18804 RVA: 0x00156E67 File Offset: 0x00155067
		public Material sharedMaterial
		{
			get
			{
				return this.m_sharedMaterial;
			}
			set
			{
				this.SetSharedMaterial(value);
			}
		}

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06004975 RID: 18805 RVA: 0x00156E70 File Offset: 0x00155070
		// (set) Token: 0x06004976 RID: 18806 RVA: 0x00156E78 File Offset: 0x00155078
		public Material fallbackMaterial
		{
			get
			{
				return this.m_fallbackMaterial;
			}
			set
			{
				if (this.m_fallbackMaterial == value)
				{
					return;
				}
				if (this.m_fallbackMaterial != null && this.m_fallbackMaterial != value)
				{
					TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				}
				this.m_fallbackMaterial = value;
				TMP_MaterialManager.AddFallbackMaterialReference(this.m_fallbackMaterial);
				this.SetSharedMaterial(this.m_fallbackMaterial);
			}
		}

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06004977 RID: 18807 RVA: 0x00156ED9 File Offset: 0x001550D9
		// (set) Token: 0x06004978 RID: 18808 RVA: 0x00156EE1 File Offset: 0x001550E1
		public Material fallbackSourceMaterial
		{
			get
			{
				return this.m_fallbackSourceMaterial;
			}
			set
			{
				this.m_fallbackSourceMaterial = value;
			}
		}

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x06004979 RID: 18809 RVA: 0x00156EEA File Offset: 0x001550EA
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

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x0600497A RID: 18810 RVA: 0x00156F08 File Offset: 0x00155108
		// (set) Token: 0x0600497B RID: 18811 RVA: 0x00156F10 File Offset: 0x00155110
		public bool isDefaultMaterial
		{
			get
			{
				return this.m_isDefaultMaterial;
			}
			set
			{
				this.m_isDefaultMaterial = value;
			}
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x0600497C RID: 18812 RVA: 0x00156F19 File Offset: 0x00155119
		// (set) Token: 0x0600497D RID: 18813 RVA: 0x00156F21 File Offset: 0x00155121
		public float padding
		{
			get
			{
				return this.m_padding;
			}
			set
			{
				this.m_padding = value;
			}
		}

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x0600497E RID: 18814 RVA: 0x00156F2A File Offset: 0x0015512A
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

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x0600497F RID: 18815 RVA: 0x00156F4C File Offset: 0x0015514C
		// (set) Token: 0x06004980 RID: 18816 RVA: 0x00156F8A File Offset: 0x0015518A
		public Mesh mesh
		{
			get
			{
				if (this.m_mesh == null)
				{
					this.m_mesh = new Mesh();
					this.m_mesh.name = "TMP_SubMeshUI_Mesh";
					this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_mesh;
			}
			set
			{
				this.m_mesh = value;
			}
		}

		// Token: 0x06004981 RID: 18817 RVA: 0x00156F94 File Offset: 0x00155194
		public static TMP_SubMeshUI AddSubTextObject(TextMeshProUGUI textComponent, MaterialReference materialReference)
		{
			GameObject gameObject = new GameObject("TMP UI SubObject [" + materialReference.material.name + "]");
			gameObject.transform.SetParent(textComponent.transform, false);
			gameObject.layer = textComponent.gameObject.layer;
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.pivot = textComponent.rectTransform.pivot;
			TMP_SubMeshUI tmp_SubMeshUI = gameObject.AddComponent<TMP_SubMeshUI>();
			tmp_SubMeshUI.m_canvasRenderer = tmp_SubMeshUI.canvasRenderer;
			tmp_SubMeshUI.m_TextComponent = textComponent;
			tmp_SubMeshUI.m_materialReferenceIndex = materialReference.index;
			tmp_SubMeshUI.m_fontAsset = materialReference.fontAsset;
			tmp_SubMeshUI.m_spriteAsset = materialReference.spriteAsset;
			tmp_SubMeshUI.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			tmp_SubMeshUI.SetSharedMaterial(materialReference.material);
			return tmp_SubMeshUI;
		}

		// Token: 0x06004982 RID: 18818 RVA: 0x0015706E File Offset: 0x0015526E
		protected override void OnEnable()
		{
			if (!this.m_isRegisteredForEvents)
			{
				this.m_isRegisteredForEvents = true;
			}
			this.m_ShouldRecalculateStencil = true;
			this.RecalculateClipping();
			this.RecalculateMasking();
		}

		// Token: 0x06004983 RID: 18819 RVA: 0x00157094 File Offset: 0x00155294
		protected override void OnDisable()
		{
			TMP_UpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
				this.m_MaskMaterial = null;
			}
			if (this.m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				this.m_fallbackMaterial = null;
			}
			base.OnDisable();
		}

		// Token: 0x06004984 RID: 18820 RVA: 0x001570F0 File Offset: 0x001552F0
		protected override void OnDestroy()
		{
			if (this.m_mesh != null)
			{
				Object.DestroyImmediate(this.m_mesh);
			}
			if (this.m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
			}
			if (this.m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				this.m_fallbackMaterial = null;
			}
			this.m_isRegisteredForEvents = false;
			this.RecalculateClipping();
		}

		// Token: 0x06004985 RID: 18821 RVA: 0x0015715C File Offset: 0x0015535C
		protected override void OnTransformParentChanged()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.m_ShouldRecalculateStencil = true;
			this.RecalculateClipping();
			this.RecalculateMasking();
		}

		// Token: 0x06004986 RID: 18822 RVA: 0x0015717C File Offset: 0x0015537C
		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (this.m_ShouldRecalculateStencil)
			{
				this.m_StencilValue = TMP_MaterialManager.GetStencilID(base.gameObject);
				this.m_ShouldRecalculateStencil = false;
			}
			if (this.m_StencilValue > 0)
			{
				material = TMP_MaterialManager.GetStencilMaterial(baseMaterial, this.m_StencilValue);
				if (this.m_MaskMaterial != null)
				{
					TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
				}
				this.m_MaskMaterial = material;
			}
			return material;
		}

		// Token: 0x06004987 RID: 18823 RVA: 0x001571E2 File Offset: 0x001553E2
		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_TextComponent.extraPadding, this.m_TextComponent.isUsingBold);
		}

		// Token: 0x06004988 RID: 18824 RVA: 0x00157205 File Offset: 0x00155405
		public float GetPaddingForMaterial(Material mat)
		{
			return ShaderUtilities.GetPadding(mat, this.m_TextComponent.extraPadding, this.m_TextComponent.isUsingBold);
		}

		// Token: 0x06004989 RID: 18825 RVA: 0x00157223 File Offset: 0x00155423
		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		// Token: 0x0600498A RID: 18826 RVA: 0x00157238 File Offset: 0x00155438
		public override void SetAllDirty()
		{
		}

		// Token: 0x0600498B RID: 18827 RVA: 0x0015723A File Offset: 0x0015543A
		public override void SetVerticesDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.havePropertiesChanged = true;
				this.m_TextComponent.SetVerticesDirty();
			}
		}

		// Token: 0x0600498C RID: 18828 RVA: 0x0015726A File Offset: 0x0015546A
		public override void SetLayoutDirty()
		{
		}

		// Token: 0x0600498D RID: 18829 RVA: 0x0015726C File Offset: 0x0015546C
		public override void SetMaterialDirty()
		{
			this.m_materialDirty = true;
			this.UpdateMaterial();
		}

		// Token: 0x0600498E RID: 18830 RVA: 0x0015727B File Offset: 0x0015547B
		public void SetPivotDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			base.rectTransform.pivot = this.m_TextComponent.rectTransform.pivot;
		}

		// Token: 0x0600498F RID: 18831 RVA: 0x001572A1 File Offset: 0x001554A1
		protected override void UpdateGeometry()
		{
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x001572A3 File Offset: 0x001554A3
		public override void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.PreRender)
			{
				if (!this.m_materialDirty)
				{
					return;
				}
				this.UpdateMaterial();
				this.m_materialDirty = false;
			}
		}

		// Token: 0x06004991 RID: 18833 RVA: 0x001572BF File Offset: 0x001554BF
		public void RefreshMaterial()
		{
			this.UpdateMaterial();
		}

		// Token: 0x06004992 RID: 18834 RVA: 0x001572C8 File Offset: 0x001554C8
		protected override void UpdateMaterial()
		{
			if (this.m_canvasRenderer == null)
			{
				this.m_canvasRenderer = this.canvasRenderer;
			}
			this.m_canvasRenderer.materialCount = 1;
			this.m_canvasRenderer.SetMaterial(this.materialForRendering, 0);
			this.m_canvasRenderer.SetTexture(this.mainTexture);
		}

		// Token: 0x06004993 RID: 18835 RVA: 0x0015731E File Offset: 0x0015551E
		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		// Token: 0x06004994 RID: 18836 RVA: 0x00157326 File Offset: 0x00155526
		public override void RecalculateMasking()
		{
			this.m_ShouldRecalculateStencil = true;
			this.SetMaterialDirty();
		}

		// Token: 0x06004995 RID: 18837 RVA: 0x00157335 File Offset: 0x00155535
		private Material GetMaterial()
		{
			return this.m_sharedMaterial;
		}

		// Token: 0x06004996 RID: 18838 RVA: 0x00157340 File Offset: 0x00155540
		private Material GetMaterial(Material mat)
		{
			if (this.m_material == null || this.m_material.GetInstanceID() != mat.GetInstanceID())
			{
				this.m_material = this.CreateMaterialInstance(mat);
			}
			this.m_sharedMaterial = this.m_material;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetVerticesDirty();
			this.SetMaterialDirty();
			return this.m_sharedMaterial;
		}

		// Token: 0x06004997 RID: 18839 RVA: 0x001573A5 File Offset: 0x001555A5
		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.name += " (Instance)";
			return material;
		}

		// Token: 0x06004998 RID: 18840 RVA: 0x001573CF File Offset: 0x001555CF
		private Material GetSharedMaterial()
		{
			if (this.m_canvasRenderer == null)
			{
				this.m_canvasRenderer = base.GetComponent<CanvasRenderer>();
			}
			return this.m_canvasRenderer.GetMaterial();
		}

		// Token: 0x06004999 RID: 18841 RVA: 0x001573F6 File Offset: 0x001555F6
		private void SetSharedMaterial(Material mat)
		{
			this.m_sharedMaterial = mat;
			this.m_Material = this.m_sharedMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		// Token: 0x0600499B RID: 18843 RVA: 0x00157425 File Offset: 0x00155625
		int ITextElement.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400494F RID: 18767
		[SerializeField]
		private TMP_FontAsset m_fontAsset;

		// Token: 0x04004950 RID: 18768
		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		// Token: 0x04004951 RID: 18769
		[SerializeField]
		private Material m_material;

		// Token: 0x04004952 RID: 18770
		[SerializeField]
		private Material m_sharedMaterial;

		// Token: 0x04004953 RID: 18771
		private Material m_fallbackMaterial;

		// Token: 0x04004954 RID: 18772
		private Material m_fallbackSourceMaterial;

		// Token: 0x04004955 RID: 18773
		[SerializeField]
		private bool m_isDefaultMaterial;

		// Token: 0x04004956 RID: 18774
		[SerializeField]
		private float m_padding;

		// Token: 0x04004957 RID: 18775
		[SerializeField]
		private CanvasRenderer m_canvasRenderer;

		// Token: 0x04004958 RID: 18776
		private Mesh m_mesh;

		// Token: 0x04004959 RID: 18777
		[SerializeField]
		private TextMeshProUGUI m_TextComponent;

		// Token: 0x0400495A RID: 18778
		[NonSerialized]
		private bool m_isRegisteredForEvents;

		// Token: 0x0400495B RID: 18779
		private bool m_materialDirty;

		// Token: 0x0400495C RID: 18780
		[SerializeField]
		private int m_materialReferenceIndex;
	}
}
