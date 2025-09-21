using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x020007E2 RID: 2018
	public class InlineGraphic : MaskableGraphic
	{
		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x060046C3 RID: 18115 RVA: 0x0013B735 File Offset: 0x00139935
		public override Texture mainTexture
		{
			get
			{
				if (this.texture == null)
				{
					return Graphic.s_WhiteTexture;
				}
				return this.texture;
			}
		}

		// Token: 0x060046C4 RID: 18116 RVA: 0x0013B751 File Offset: 0x00139951
		protected override void Awake()
		{
			this.m_manager = base.GetComponentInParent<InlineGraphicManager>();
		}

		// Token: 0x060046C5 RID: 18117 RVA: 0x0013B760 File Offset: 0x00139960
		protected override void OnEnable()
		{
			if (this.m_RectTransform == null)
			{
				this.m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			if (this.m_manager != null && this.m_manager.spriteAsset != null)
			{
				this.texture = this.m_manager.spriteAsset.spriteSheet;
			}
		}

		// Token: 0x060046C6 RID: 18118 RVA: 0x0013B7C3 File Offset: 0x001399C3
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x060046C7 RID: 18119 RVA: 0x0013B7CB File Offset: 0x001399CB
		protected override void OnTransformParentChanged()
		{
		}

		// Token: 0x060046C8 RID: 18120 RVA: 0x0013B7D0 File Offset: 0x001399D0
		protected override void OnRectTransformDimensionsChange()
		{
			if (this.m_RectTransform == null)
			{
				this.m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			if (this.m_ParentRectTransform == null)
			{
				this.m_ParentRectTransform = this.m_RectTransform.parent.GetComponent<RectTransform>();
			}
			if (this.m_RectTransform.pivot != this.m_ParentRectTransform.pivot)
			{
				this.m_RectTransform.pivot = this.m_ParentRectTransform.pivot;
			}
		}

		// Token: 0x060046C9 RID: 18121 RVA: 0x0013B853 File Offset: 0x00139A53
		public new void UpdateMaterial()
		{
			base.UpdateMaterial();
		}

		// Token: 0x060046CA RID: 18122 RVA: 0x0013B85B File Offset: 0x00139A5B
		protected override void UpdateGeometry()
		{
		}

		// Token: 0x0400471E RID: 18206
		public Texture texture;

		// Token: 0x0400471F RID: 18207
		private InlineGraphicManager m_manager;

		// Token: 0x04004720 RID: 18208
		private RectTransform m_RectTransform;

		// Token: 0x04004721 RID: 18209
		private RectTransform m_ParentRectTransform;
	}
}
