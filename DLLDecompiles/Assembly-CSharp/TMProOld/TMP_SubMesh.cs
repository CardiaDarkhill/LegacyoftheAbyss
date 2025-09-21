using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x0200081C RID: 2076
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class TMP_SubMesh : MonoBehaviour
	{
		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x06004947 RID: 18759 RVA: 0x00156730 File Offset: 0x00154930
		// (set) Token: 0x06004948 RID: 18760 RVA: 0x00156738 File Offset: 0x00154938
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

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06004949 RID: 18761 RVA: 0x00156741 File Offset: 0x00154941
		// (set) Token: 0x0600494A RID: 18762 RVA: 0x00156749 File Offset: 0x00154949
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

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600494B RID: 18763 RVA: 0x00156752 File Offset: 0x00154952
		// (set) Token: 0x0600494C RID: 18764 RVA: 0x00156760 File Offset: 0x00154960
		public Material material
		{
			get
			{
				return this.GetMaterial(this.m_sharedMaterial);
			}
			set
			{
				if (this.m_sharedMaterial.GetInstanceID() == value.GetInstanceID())
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

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600494D RID: 18765 RVA: 0x001567A9 File Offset: 0x001549A9
		// (set) Token: 0x0600494E RID: 18766 RVA: 0x001567B1 File Offset: 0x001549B1
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

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x0600494F RID: 18767 RVA: 0x001567BA File Offset: 0x001549BA
		// (set) Token: 0x06004950 RID: 18768 RVA: 0x001567C4 File Offset: 0x001549C4
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

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06004951 RID: 18769 RVA: 0x00156825 File Offset: 0x00154A25
		// (set) Token: 0x06004952 RID: 18770 RVA: 0x0015682D File Offset: 0x00154A2D
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

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06004953 RID: 18771 RVA: 0x00156836 File Offset: 0x00154A36
		// (set) Token: 0x06004954 RID: 18772 RVA: 0x0015683E File Offset: 0x00154A3E
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

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x06004955 RID: 18773 RVA: 0x00156847 File Offset: 0x00154A47
		// (set) Token: 0x06004956 RID: 18774 RVA: 0x0015684F File Offset: 0x00154A4F
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

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06004957 RID: 18775 RVA: 0x00156858 File Offset: 0x00154A58
		public Renderer renderer
		{
			get
			{
				if (this.m_renderer == null)
				{
					this.m_renderer = base.GetComponent<Renderer>();
				}
				return this.m_renderer;
			}
		}

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06004958 RID: 18776 RVA: 0x0015687C File Offset: 0x00154A7C
		public MeshFilter meshFilter
		{
			get
			{
				if (this.m_meshFilter == null)
				{
					this.m_meshFilter = base.GetComponent<MeshFilter>();
					if (this.m_meshFilter == null)
					{
						this.m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
						this.m_meshFilter.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
					}
				}
				return this.m_meshFilter;
			}
		}

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06004959 RID: 18777 RVA: 0x001568D8 File Offset: 0x00154AD8
		// (set) Token: 0x0600495A RID: 18778 RVA: 0x00156932 File Offset: 0x00154B32
		public Mesh mesh
		{
			get
			{
				if (this.m_mesh == null)
				{
					this.m_mesh = new Mesh();
					this.m_mesh.name = "TMP_SubMesh_Mesh";
					this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
					this.meshFilter.mesh = this.m_mesh;
				}
				return this.m_mesh;
			}
			set
			{
				this.m_mesh = value;
			}
		}

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x0600495B RID: 18779 RVA: 0x0015693B File Offset: 0x00154B3B
		public TMP_Text textComponent
		{
			get
			{
				if (this.m_TextComponent == null)
				{
					this.m_TextComponent = base.GetComponentInParent<TextMeshPro>();
				}
				return this.m_TextComponent;
			}
		}

		// Token: 0x0600495C RID: 18780 RVA: 0x00156960 File Offset: 0x00154B60
		public static TMP_SubMesh AddSubTextObject(TextMeshPro textComponent, MaterialReference materialReference)
		{
			GameObject gameObject = new GameObject();
			gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild);
			gameObject.transform.SetParent(textComponent.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = textComponent.gameObject.layer;
			TMP_SubMesh tmp_SubMesh = gameObject.AddComponent<TMP_SubMesh>();
			tmp_SubMesh.m_TextComponent = textComponent;
			tmp_SubMesh.m_fontAsset = materialReference.fontAsset;
			tmp_SubMesh.m_spriteAsset = materialReference.spriteAsset;
			tmp_SubMesh.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			tmp_SubMesh.SetSharedMaterial(materialReference.material);
			tmp_SubMesh.renderer.sortingLayerID = textComponent.renderer.sortingLayerID;
			tmp_SubMesh.renderer.sortingOrder = textComponent.renderer.sortingOrder;
			LinkRendererState linkRendererState = tmp_SubMesh.gameObject.AddComponent<LinkRendererState>();
			linkRendererState.parent = textComponent.renderer;
			linkRendererState.child = tmp_SubMesh.renderer;
			linkRendererState.UpdateLink();
			return tmp_SubMesh;
		}

		// Token: 0x0600495D RID: 18781 RVA: 0x00156A5F File Offset: 0x00154C5F
		private void Awake()
		{
			this.clipRect = base.gameObject.GetComponentInParent<TextMeshProClipRect>();
			if (this.clipRect)
			{
				this.clipRect.AddSubMesh(this);
			}
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x00156A8C File Offset: 0x00154C8C
		private void OnEnable()
		{
			if (!this.m_isRegisteredForEvents)
			{
				this.m_isRegisteredForEvents = true;
			}
			if (base.hideFlags != (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild))
			{
				base.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild);
			}
			this.meshFilter.sharedMesh = this.mesh;
			if (this.m_sharedMaterial != null)
			{
				this.m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, new Vector4(-10000f, -10000f, 10000f, 10000f));
			}
		}

		// Token: 0x0600495F RID: 18783 RVA: 0x00156B02 File Offset: 0x00154D02
		private void OnDisable()
		{
			this.m_meshFilter.sharedMesh = null;
			if (this.m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				this.m_fallbackMaterial = null;
			}
		}

		// Token: 0x06004960 RID: 18784 RVA: 0x00156B30 File Offset: 0x00154D30
		private void OnDestroy()
		{
			if (this.m_mesh != null)
			{
				Object.DestroyImmediate(this.m_mesh);
			}
			if (this.m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				this.m_fallbackMaterial = null;
			}
			if (this.clipRect != null)
			{
				this.clipRect.RemoveSubMesh(this);
			}
			this.m_isRegisteredForEvents = false;
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.havePropertiesChanged = true;
				this.m_TextComponent.SetAllDirty();
			}
		}

		// Token: 0x06004961 RID: 18785 RVA: 0x00156BBC File Offset: 0x00154DBC
		public void DestroySelf()
		{
			Object.Destroy(base.gameObject, 1f);
		}

		// Token: 0x06004962 RID: 18786 RVA: 0x00156BD0 File Offset: 0x00154DD0
		private Material GetMaterial(Material mat)
		{
			if (this.m_renderer == null)
			{
				this.m_renderer = base.GetComponent<Renderer>();
			}
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

		// Token: 0x06004963 RID: 18787 RVA: 0x00156C4F File Offset: 0x00154E4F
		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.name += " (Instance)";
			return material;
		}

		// Token: 0x06004964 RID: 18788 RVA: 0x00156C79 File Offset: 0x00154E79
		private Material GetSharedMaterial()
		{
			if (this.m_renderer == null)
			{
				this.m_renderer = base.GetComponent<Renderer>();
			}
			return this.m_renderer.sharedMaterial;
		}

		// Token: 0x06004965 RID: 18789 RVA: 0x00156CA0 File Offset: 0x00154EA0
		private void SetSharedMaterial(Material mat)
		{
			this.m_sharedMaterial = mat;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		// Token: 0x06004966 RID: 18790 RVA: 0x00156CBB File Offset: 0x00154EBB
		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_TextComponent.extraPadding, this.m_TextComponent.isUsingBold);
		}

		// Token: 0x06004967 RID: 18791 RVA: 0x00156CDE File Offset: 0x00154EDE
		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		// Token: 0x06004968 RID: 18792 RVA: 0x00156CF3 File Offset: 0x00154EF3
		public void SetVerticesDirty()
		{
			if (!base.enabled)
			{
				return;
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.havePropertiesChanged = true;
				this.m_TextComponent.SetVerticesDirty();
			}
		}

		// Token: 0x06004969 RID: 18793 RVA: 0x00156D23 File Offset: 0x00154F23
		public void SetMaterialDirty()
		{
			this.UpdateMaterial();
		}

		// Token: 0x0600496A RID: 18794 RVA: 0x00156D2C File Offset: 0x00154F2C
		protected void UpdateMaterial()
		{
			if (this.renderer == null || this.m_sharedMaterial == null)
			{
				return;
			}
			this.m_renderer.sharedMaterial = this.m_sharedMaterial;
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ShaderTag_CullMode) && this.textComponent.fontSharedMaterial != null)
			{
				float @float = this.textComponent.fontSharedMaterial.GetFloat(ShaderUtilities.ShaderTag_CullMode);
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_CullMode, @float);
			}
		}

		// Token: 0x04004940 RID: 18752
		[SerializeField]
		private TMP_FontAsset m_fontAsset;

		// Token: 0x04004941 RID: 18753
		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		// Token: 0x04004942 RID: 18754
		[SerializeField]
		private Material m_material;

		// Token: 0x04004943 RID: 18755
		[SerializeField]
		private Material m_sharedMaterial;

		// Token: 0x04004944 RID: 18756
		private Material m_fallbackMaterial;

		// Token: 0x04004945 RID: 18757
		private Material m_fallbackSourceMaterial;

		// Token: 0x04004946 RID: 18758
		[SerializeField]
		private bool m_isDefaultMaterial;

		// Token: 0x04004947 RID: 18759
		[SerializeField]
		private float m_padding;

		// Token: 0x04004948 RID: 18760
		[SerializeField]
		private Renderer m_renderer;

		// Token: 0x04004949 RID: 18761
		[SerializeField]
		private MeshFilter m_meshFilter;

		// Token: 0x0400494A RID: 18762
		private Mesh m_mesh;

		// Token: 0x0400494B RID: 18763
		[SerializeField]
		private TextMeshPro m_TextComponent;

		// Token: 0x0400494C RID: 18764
		[NonSerialized]
		private bool m_isRegisteredForEvents;

		// Token: 0x0400494D RID: 18765
		private const HideFlags TARGET_HIDE_FLAGS = HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;

		// Token: 0x0400494E RID: 18766
		private TextMeshProClipRect clipRect;
	}
}
