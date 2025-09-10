using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007E3 RID: 2019
	[ExecuteInEditMode]
	public class InlineGraphicManager : MonoBehaviour
	{
		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x060046CC RID: 18124 RVA: 0x0013B865 File Offset: 0x00139A65
		// (set) Token: 0x060046CD RID: 18125 RVA: 0x0013B86D File Offset: 0x00139A6D
		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return this.m_spriteAsset;
			}
			set
			{
				this.LoadSpriteAsset(value);
			}
		}

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x060046CE RID: 18126 RVA: 0x0013B876 File Offset: 0x00139A76
		// (set) Token: 0x060046CF RID: 18127 RVA: 0x0013B87E File Offset: 0x00139A7E
		public InlineGraphic inlineGraphic
		{
			get
			{
				return this.m_inlineGraphic;
			}
			set
			{
				if (this.m_inlineGraphic != value)
				{
					this.m_inlineGraphic = value;
				}
			}
		}

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x060046D0 RID: 18128 RVA: 0x0013B895 File Offset: 0x00139A95
		public CanvasRenderer canvasRenderer
		{
			get
			{
				return this.m_inlineGraphicCanvasRenderer;
			}
		}

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x060046D1 RID: 18129 RVA: 0x0013B89D File Offset: 0x00139A9D
		public UIVertex[] uiVertex
		{
			get
			{
				return this.m_uiVertex;
			}
		}

		// Token: 0x060046D2 RID: 18130 RVA: 0x0013B8A8 File Offset: 0x00139AA8
		private void Awake()
		{
			if (!TMP_Settings.warningsDisabled)
			{
				Debug.LogWarning("InlineGraphicManager component is now Obsolete and has been removed from [" + base.gameObject.name + "] along with its InlineGraphic child.", this);
			}
			if (this.inlineGraphic.gameObject != null)
			{
				Object.DestroyImmediate(this.inlineGraphic.gameObject);
				this.inlineGraphic = null;
			}
			Object.DestroyImmediate(this);
		}

		// Token: 0x060046D3 RID: 18131 RVA: 0x0013B90C File Offset: 0x00139B0C
		private void OnEnable()
		{
			base.enabled = false;
		}

		// Token: 0x060046D4 RID: 18132 RVA: 0x0013B915 File Offset: 0x00139B15
		private void OnDisable()
		{
		}

		// Token: 0x060046D5 RID: 18133 RVA: 0x0013B917 File Offset: 0x00139B17
		private void OnDestroy()
		{
		}

		// Token: 0x060046D6 RID: 18134 RVA: 0x0013B91C File Offset: 0x00139B1C
		private void LoadSpriteAsset(TMP_SpriteAsset spriteAsset)
		{
			if (spriteAsset == null)
			{
				if (TMP_Settings.defaultSpriteAsset != null)
				{
					spriteAsset = TMP_Settings.defaultSpriteAsset;
				}
				else
				{
					spriteAsset = (Resources.Load("Sprite Assets/Default Sprite Asset") as TMP_SpriteAsset);
				}
			}
			this.m_spriteAsset = spriteAsset;
			this.m_inlineGraphic.texture = this.m_spriteAsset.spriteSheet;
			if (this.m_textComponent != null && this.m_isInitialized)
			{
				this.m_textComponent.havePropertiesChanged = true;
				this.m_textComponent.SetVerticesDirty();
			}
		}

		// Token: 0x060046D7 RID: 18135 RVA: 0x0013B9A4 File Offset: 0x00139BA4
		public void AddInlineGraphicsChild()
		{
			if (this.m_inlineGraphic != null)
			{
				return;
			}
			GameObject gameObject = new GameObject("Inline Graphic");
			this.m_inlineGraphic = gameObject.AddComponent<InlineGraphic>();
			this.m_inlineGraphicRectTransform = gameObject.GetComponent<RectTransform>();
			this.m_inlineGraphicCanvasRenderer = gameObject.GetComponent<CanvasRenderer>();
			this.m_inlineGraphicRectTransform.SetParent(base.transform, false);
			this.m_inlineGraphicRectTransform.localPosition = Vector3.zero;
			this.m_inlineGraphicRectTransform.anchoredPosition3D = Vector3.zero;
			this.m_inlineGraphicRectTransform.sizeDelta = Vector2.zero;
			this.m_inlineGraphicRectTransform.anchorMin = Vector2.zero;
			this.m_inlineGraphicRectTransform.anchorMax = Vector2.one;
			this.m_textComponent = base.GetComponent<TMP_Text>();
		}

		// Token: 0x060046D8 RID: 18136 RVA: 0x0013BA60 File Offset: 0x00139C60
		public void AllocatedVertexBuffers(int size)
		{
			if (this.m_inlineGraphic == null)
			{
				this.AddInlineGraphicsChild();
				this.LoadSpriteAsset(this.m_spriteAsset);
			}
			if (this.m_uiVertex == null)
			{
				this.m_uiVertex = new UIVertex[4];
			}
			int num = size * 4;
			if (num > this.m_uiVertex.Length)
			{
				this.m_uiVertex = new UIVertex[Mathf.NextPowerOfTwo(num)];
			}
		}

		// Token: 0x060046D9 RID: 18137 RVA: 0x0013BAC1 File Offset: 0x00139CC1
		public void UpdatePivot(Vector2 pivot)
		{
			if (this.m_inlineGraphicRectTransform == null)
			{
				this.m_inlineGraphicRectTransform = this.m_inlineGraphic.GetComponent<RectTransform>();
			}
			this.m_inlineGraphicRectTransform.pivot = pivot;
		}

		// Token: 0x060046DA RID: 18138 RVA: 0x0013BAEE File Offset: 0x00139CEE
		public void ClearUIVertex()
		{
			if (this.uiVertex != null && this.uiVertex.Length != 0)
			{
				Array.Clear(this.uiVertex, 0, this.uiVertex.Length);
				this.m_inlineGraphicCanvasRenderer.Clear();
			}
		}

		// Token: 0x060046DB RID: 18139 RVA: 0x0013BB20 File Offset: 0x00139D20
		public void DrawSprite(UIVertex[] uiVertices, int spriteCount)
		{
			if (this.m_inlineGraphicCanvasRenderer == null)
			{
				this.m_inlineGraphicCanvasRenderer = this.m_inlineGraphic.GetComponent<CanvasRenderer>();
			}
			this.m_inlineGraphicCanvasRenderer.SetVertices(uiVertices, spriteCount * 4);
			this.m_inlineGraphic.UpdateMaterial();
		}

		// Token: 0x060046DC RID: 18140 RVA: 0x0013BB5C File Offset: 0x00139D5C
		public TMP_Sprite GetSprite(int index)
		{
			if (this.m_spriteAsset == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.", this);
				return null;
			}
			if (this.m_spriteAsset.spriteInfoList == null || index > this.m_spriteAsset.spriteInfoList.Count - 1)
			{
				Debug.LogWarning("Sprite index exceeds the number of sprites in this Sprite Asset.", this);
				return null;
			}
			return this.m_spriteAsset.spriteInfoList[index];
		}

		// Token: 0x060046DD RID: 18141 RVA: 0x0013BBC4 File Offset: 0x00139DC4
		public int GetSpriteIndexByHashCode(int hashCode)
		{
			if (this.m_spriteAsset == null || this.m_spriteAsset.spriteInfoList == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.", this);
				return -1;
			}
			return this.m_spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.hashCode == hashCode);
		}

		// Token: 0x060046DE RID: 18142 RVA: 0x0013BC24 File Offset: 0x00139E24
		public int GetSpriteIndexByIndex(int index)
		{
			if (this.m_spriteAsset == null || this.m_spriteAsset.spriteInfoList == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.", this);
				return -1;
			}
			return this.m_spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.id == index);
		}

		// Token: 0x060046DF RID: 18143 RVA: 0x0013BC82 File Offset: 0x00139E82
		public void SetUIVertex(UIVertex[] uiVertex)
		{
			this.m_uiVertex = uiVertex;
		}

		// Token: 0x04004722 RID: 18210
		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		// Token: 0x04004723 RID: 18211
		[SerializeField]
		[HideInInspector]
		private InlineGraphic m_inlineGraphic;

		// Token: 0x04004724 RID: 18212
		[SerializeField]
		[HideInInspector]
		private CanvasRenderer m_inlineGraphicCanvasRenderer;

		// Token: 0x04004725 RID: 18213
		private UIVertex[] m_uiVertex;

		// Token: 0x04004726 RID: 18214
		private RectTransform m_inlineGraphicRectTransform;

		// Token: 0x04004727 RID: 18215
		private TMP_Text m_textComponent;

		// Token: 0x04004728 RID: 18216
		private bool m_isInitialized;
	}
}
