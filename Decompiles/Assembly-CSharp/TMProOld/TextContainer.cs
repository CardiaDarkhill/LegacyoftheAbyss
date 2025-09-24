using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMProOld
{
	// Token: 0x020007E7 RID: 2023
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Layout/Text Container")]
	public class TextContainer : UIBehaviour
	{
		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060046F7 RID: 18167 RVA: 0x0013C050 File Offset: 0x0013A250
		// (set) Token: 0x060046F8 RID: 18168 RVA: 0x0013C058 File Offset: 0x0013A258
		public bool hasChanged
		{
			get
			{
				return this.m_hasChanged;
			}
			set
			{
				this.m_hasChanged = value;
			}
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060046F9 RID: 18169 RVA: 0x0013C061 File Offset: 0x0013A261
		// (set) Token: 0x060046FA RID: 18170 RVA: 0x0013C069 File Offset: 0x0013A269
		public Vector2 pivot
		{
			get
			{
				return this.m_pivot;
			}
			set
			{
				if (this.m_pivot != value)
				{
					this.m_pivot = value;
					this.m_anchorPosition = this.GetAnchorPosition(this.m_pivot);
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060046FB RID: 18171 RVA: 0x0013C09F File Offset: 0x0013A29F
		// (set) Token: 0x060046FC RID: 18172 RVA: 0x0013C0A7 File Offset: 0x0013A2A7
		public TextContainerAnchors anchorPosition
		{
			get
			{
				return this.m_anchorPosition;
			}
			set
			{
				if (this.m_anchorPosition != value)
				{
					this.m_anchorPosition = value;
					this.m_pivot = this.GetPivot(this.m_anchorPosition);
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060046FD RID: 18173 RVA: 0x0013C0D8 File Offset: 0x0013A2D8
		// (set) Token: 0x060046FE RID: 18174 RVA: 0x0013C0E0 File Offset: 0x0013A2E0
		public Rect rect
		{
			get
			{
				return this.m_rect;
			}
			set
			{
				if (this.m_rect != value)
				{
					this.m_rect = value;
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060046FF RID: 18175 RVA: 0x0013C104 File Offset: 0x0013A304
		// (set) Token: 0x06004700 RID: 18176 RVA: 0x0013C124 File Offset: 0x0013A324
		public Vector2 size
		{
			get
			{
				return new Vector2(this.m_rect.width, this.m_rect.height);
			}
			set
			{
				if (new Vector2(this.m_rect.width, this.m_rect.height) != value)
				{
					this.SetRect(value);
					this.m_hasChanged = true;
					this.m_isDefaultWidth = false;
					this.m_isDefaultHeight = false;
					this.OnContainerChanged();
				}
			}
		}

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x06004701 RID: 18177 RVA: 0x0013C176 File Offset: 0x0013A376
		// (set) Token: 0x06004702 RID: 18178 RVA: 0x0013C183 File Offset: 0x0013A383
		public float width
		{
			get
			{
				return this.m_rect.width;
			}
			set
			{
				this.SetRect(new Vector2(value, this.m_rect.height));
				this.m_hasChanged = true;
				this.m_isDefaultWidth = false;
				this.OnContainerChanged();
			}
		}

		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x06004703 RID: 18179 RVA: 0x0013C1B0 File Offset: 0x0013A3B0
		// (set) Token: 0x06004704 RID: 18180 RVA: 0x0013C1BD File Offset: 0x0013A3BD
		public float height
		{
			get
			{
				return this.m_rect.height;
			}
			set
			{
				this.SetRect(new Vector2(this.m_rect.width, value));
				this.m_hasChanged = true;
				this.m_isDefaultHeight = false;
				this.OnContainerChanged();
			}
		}

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x06004705 RID: 18181 RVA: 0x0013C1EA File Offset: 0x0013A3EA
		public bool isDefaultWidth
		{
			get
			{
				return this.m_isDefaultWidth;
			}
		}

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x06004706 RID: 18182 RVA: 0x0013C1F2 File Offset: 0x0013A3F2
		public bool isDefaultHeight
		{
			get
			{
				return this.m_isDefaultHeight;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x06004707 RID: 18183 RVA: 0x0013C1FA File Offset: 0x0013A3FA
		// (set) Token: 0x06004708 RID: 18184 RVA: 0x0013C202 File Offset: 0x0013A402
		public bool isAutoFitting
		{
			get
			{
				return this.m_isAutoFitting;
			}
			set
			{
				this.m_isAutoFitting = value;
			}
		}

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x06004709 RID: 18185 RVA: 0x0013C20B File Offset: 0x0013A40B
		public Vector3[] corners
		{
			get
			{
				return this.m_corners;
			}
		}

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x0600470A RID: 18186 RVA: 0x0013C213 File Offset: 0x0013A413
		public Vector3[] worldCorners
		{
			get
			{
				return this.m_worldCorners;
			}
		}

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x0600470B RID: 18187 RVA: 0x0013C21B File Offset: 0x0013A41B
		// (set) Token: 0x0600470C RID: 18188 RVA: 0x0013C223 File Offset: 0x0013A423
		public Vector4 margins
		{
			get
			{
				return this.m_margins;
			}
			set
			{
				if (this.m_margins != value)
				{
					this.m_margins = value;
					this.m_hasChanged = true;
					this.OnContainerChanged();
				}
			}
		}

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x0600470D RID: 18189 RVA: 0x0013C247 File Offset: 0x0013A447
		public RectTransform rectTransform
		{
			get
			{
				if (this.m_rectTransform == null)
				{
					this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
			}
		}

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x0600470E RID: 18190 RVA: 0x0013C269 File Offset: 0x0013A469
		public TextMeshPro textMeshPro
		{
			get
			{
				if (this.m_textMeshPro == null)
				{
					this.m_textMeshPro = base.GetComponent<TextMeshPro>();
				}
				return this.m_textMeshPro;
			}
		}

		// Token: 0x0600470F RID: 18191 RVA: 0x0013C28C File Offset: 0x0013A48C
		protected override void Awake()
		{
			this.m_rectTransform = this.rectTransform;
			if (this.m_rectTransform == null)
			{
				Vector2 pivot = this.m_pivot;
				this.m_rectTransform = base.gameObject.AddComponent<RectTransform>();
				this.m_pivot = pivot;
			}
			this.m_textMeshPro = (base.GetComponent(typeof(TextMeshPro)) as TextMeshPro);
			if (this.m_rect.width == 0f || this.m_rect.height == 0f)
			{
				if (this.m_textMeshPro != null && this.m_textMeshPro.anchor != TMP_Compatibility.AnchorPositions.None)
				{
					Debug.LogWarning("Converting from using anchor and lineLength properties to Text Container.", this);
					this.m_isDefaultHeight = true;
					int num = (int)this.m_textMeshPro.anchor;
					this.m_textMeshPro.anchor = TMP_Compatibility.AnchorPositions.None;
					if (num == 9)
					{
						switch (this.m_textMeshPro.alignment)
						{
						case TextAlignmentOptions.TopLeft:
							this.m_textMeshPro.alignment = TextAlignmentOptions.BaselineLeft;
							break;
						case TextAlignmentOptions.Top:
							this.m_textMeshPro.alignment = TextAlignmentOptions.Baseline;
							break;
						case TextAlignmentOptions.TopRight:
							this.m_textMeshPro.alignment = TextAlignmentOptions.BaselineRight;
							break;
						case TextAlignmentOptions.TopJustified:
							this.m_textMeshPro.alignment = TextAlignmentOptions.BaselineJustified;
							break;
						}
						num = 3;
					}
					this.m_anchorPosition = (TextContainerAnchors)num;
					this.m_pivot = this.GetPivot(this.m_anchorPosition);
					if (this.m_textMeshPro.lineLength == 72f)
					{
						this.m_rect.size = this.m_textMeshPro.GetPreferredValues(this.m_textMeshPro.text);
					}
					else
					{
						this.m_rect.width = this.m_textMeshPro.lineLength;
						this.m_rect.height = this.m_textMeshPro.GetPreferredValues(this.m_rect.width, float.PositiveInfinity).y;
					}
				}
				else
				{
					this.m_isDefaultWidth = true;
					this.m_isDefaultHeight = true;
					this.m_pivot = this.GetPivot(this.m_anchorPosition);
					this.m_rect.width = 20f;
					this.m_rect.height = 5f;
					this.m_rectTransform.sizeDelta = this.size;
				}
				this.m_margins = new Vector4(0f, 0f, 0f, 0f);
				this.UpdateCorners();
			}
		}

		// Token: 0x06004710 RID: 18192 RVA: 0x0013C4D1 File Offset: 0x0013A6D1
		protected override void OnEnable()
		{
			this.OnContainerChanged();
		}

		// Token: 0x06004711 RID: 18193 RVA: 0x0013C4D9 File Offset: 0x0013A6D9
		protected override void OnDisable()
		{
		}

		// Token: 0x06004712 RID: 18194 RVA: 0x0013C4DC File Offset: 0x0013A6DC
		private void OnContainerChanged()
		{
			this.UpdateCorners();
			if (this.m_rectTransform != null)
			{
				this.m_rectTransform.sizeDelta = this.size;
				this.m_rectTransform.hasChanged = true;
			}
			if (this.textMeshPro != null)
			{
				this.m_textMeshPro.SetVerticesDirty();
				this.m_textMeshPro.margin = this.m_margins;
			}
		}

		// Token: 0x06004713 RID: 18195 RVA: 0x0013C544 File Offset: 0x0013A744
		protected override void OnRectTransformDimensionsChange()
		{
			if (this.rectTransform == null)
			{
				this.m_rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			if (this.m_rectTransform.sizeDelta != TextContainer.k_defaultSize)
			{
				this.size = this.m_rectTransform.sizeDelta;
			}
			this.pivot = this.m_rectTransform.pivot;
			this.m_hasChanged = true;
			this.OnContainerChanged();
		}

		// Token: 0x06004714 RID: 18196 RVA: 0x0013C5B6 File Offset: 0x0013A7B6
		private void SetRect(Vector2 size)
		{
			this.m_rect = new Rect(this.m_rect.x, this.m_rect.y, size.x, size.y);
		}

		// Token: 0x06004715 RID: 18197 RVA: 0x0013C5E8 File Offset: 0x0013A7E8
		private void UpdateCorners()
		{
			this.m_corners[0] = new Vector3(-this.m_pivot.x * this.m_rect.width, -this.m_pivot.y * this.m_rect.height);
			this.m_corners[1] = new Vector3(-this.m_pivot.x * this.m_rect.width, (1f - this.m_pivot.y) * this.m_rect.height);
			this.m_corners[2] = new Vector3((1f - this.m_pivot.x) * this.m_rect.width, (1f - this.m_pivot.y) * this.m_rect.height);
			this.m_corners[3] = new Vector3((1f - this.m_pivot.x) * this.m_rect.width, -this.m_pivot.y * this.m_rect.height);
			if (this.m_rectTransform != null)
			{
				this.m_rectTransform.pivot = this.m_pivot;
			}
		}

		// Token: 0x06004716 RID: 18198 RVA: 0x0013C72C File Offset: 0x0013A92C
		private Vector2 GetPivot(TextContainerAnchors anchor)
		{
			Vector2 zero = Vector2.zero;
			switch (anchor)
			{
			case TextContainerAnchors.TopLeft:
				zero = new Vector2(0f, 1f);
				break;
			case TextContainerAnchors.Top:
				zero = new Vector2(0.5f, 1f);
				break;
			case TextContainerAnchors.TopRight:
				zero = new Vector2(1f, 1f);
				break;
			case TextContainerAnchors.Left:
				zero = new Vector2(0f, 0.5f);
				break;
			case TextContainerAnchors.Middle:
				zero = new Vector2(0.5f, 0.5f);
				break;
			case TextContainerAnchors.Right:
				zero = new Vector2(1f, 0.5f);
				break;
			case TextContainerAnchors.BottomLeft:
				zero = new Vector2(0f, 0f);
				break;
			case TextContainerAnchors.Bottom:
				zero = new Vector2(0.5f, 0f);
				break;
			case TextContainerAnchors.BottomRight:
				zero = new Vector2(1f, 0f);
				break;
			}
			return zero;
		}

		// Token: 0x06004717 RID: 18199 RVA: 0x0013C820 File Offset: 0x0013AA20
		private TextContainerAnchors GetAnchorPosition(Vector2 pivot)
		{
			if (pivot == new Vector2(0f, 1f))
			{
				return TextContainerAnchors.TopLeft;
			}
			if (pivot == new Vector2(0.5f, 1f))
			{
				return TextContainerAnchors.Top;
			}
			if (pivot == new Vector2(1f, 1f))
			{
				return TextContainerAnchors.TopRight;
			}
			if (pivot == new Vector2(0f, 0.5f))
			{
				return TextContainerAnchors.Left;
			}
			if (pivot == new Vector2(0.5f, 0.5f))
			{
				return TextContainerAnchors.Middle;
			}
			if (pivot == new Vector2(1f, 0.5f))
			{
				return TextContainerAnchors.Right;
			}
			if (pivot == new Vector2(0f, 0f))
			{
				return TextContainerAnchors.BottomLeft;
			}
			if (pivot == new Vector2(0.5f, 0f))
			{
				return TextContainerAnchors.Bottom;
			}
			if (pivot == new Vector2(1f, 0f))
			{
				return TextContainerAnchors.BottomRight;
			}
			return TextContainerAnchors.Custom;
		}

		// Token: 0x04004741 RID: 18241
		private bool m_hasChanged;

		// Token: 0x04004742 RID: 18242
		[SerializeField]
		private Vector2 m_pivot;

		// Token: 0x04004743 RID: 18243
		[SerializeField]
		private TextContainerAnchors m_anchorPosition = TextContainerAnchors.Middle;

		// Token: 0x04004744 RID: 18244
		[SerializeField]
		private Rect m_rect;

		// Token: 0x04004745 RID: 18245
		private bool m_isDefaultWidth;

		// Token: 0x04004746 RID: 18246
		private bool m_isDefaultHeight;

		// Token: 0x04004747 RID: 18247
		private bool m_isAutoFitting;

		// Token: 0x04004748 RID: 18248
		private Vector3[] m_corners = new Vector3[4];

		// Token: 0x04004749 RID: 18249
		private Vector3[] m_worldCorners = new Vector3[4];

		// Token: 0x0400474A RID: 18250
		[SerializeField]
		private Vector4 m_margins;

		// Token: 0x0400474B RID: 18251
		private RectTransform m_rectTransform;

		// Token: 0x0400474C RID: 18252
		private static Vector2 k_defaultSize = new Vector2(100f, 100f);

		// Token: 0x0400474D RID: 18253
		private TextMeshPro m_textMeshPro;
	}
}
