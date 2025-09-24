using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200102B RID: 4139
	[ActionCategory("RectTransform")]
	[Tooltip("The position ( normalized or not) in the parent RectTransform keeping the anchor rect size intact. This lets you position the whole Rect in one go. Use this to easily animate movement (like IOS sliding UIView)")]
	public class RectTransformSetAnchorRectPosition : BaseUpdateAction
	{
		// Token: 0x06007194 RID: 29076 RVA: 0x0022F07C File Offset: 0x0022D27C
		public override void Reset()
		{
			base.Reset();
			this.normalized = true;
			this.gameObject = null;
			this.anchorReference = RectTransformSetAnchorRectPosition.AnchorReference.BottomLeft;
			this.anchor = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007195 RID: 29077 RVA: 0x0022F0D4 File Offset: 0x0022D2D4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetAnchor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007196 RID: 29078 RVA: 0x0022F11C File Offset: 0x0022D31C
		public override void OnActionUpdate()
		{
			this.DoSetAnchor();
		}

		// Token: 0x06007197 RID: 29079 RVA: 0x0022F124 File Offset: 0x0022D324
		private void DoSetAnchor()
		{
			this._anchorRect = default(Rect);
			this._anchorRect.min = this._rt.anchorMin;
			this._anchorRect.max = this._rt.anchorMax;
			Vector2 vector = Vector2.zero;
			vector = this._anchorRect.min;
			if (!this.anchor.IsNone)
			{
				if (this.normalized.Value)
				{
					vector = this.anchor.Value;
				}
				else
				{
					vector.x = this.anchor.Value.x / (float)Screen.width;
					vector.y = this.anchor.Value.y / (float)Screen.height;
				}
			}
			if (!this.x.IsNone)
			{
				if (this.normalized.Value)
				{
					vector.x = this.x.Value;
				}
				else
				{
					vector.x = this.x.Value / (float)Screen.width;
				}
			}
			if (!this.y.IsNone)
			{
				if (this.normalized.Value)
				{
					vector.y = this.y.Value;
				}
				else
				{
					vector.y = this.y.Value / (float)Screen.height;
				}
			}
			if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.BottomLeft)
			{
				this._anchorRect.x = vector.x;
				this._anchorRect.y = vector.y;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.Left)
			{
				this._anchorRect.x = vector.x;
				this._anchorRect.y = vector.y - 0.5f;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.TopLeft)
			{
				this._anchorRect.x = vector.x;
				this._anchorRect.y = vector.y - 1f;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.Top)
			{
				this._anchorRect.x = vector.x - 0.5f;
				this._anchorRect.y = vector.y - 1f;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.TopRight)
			{
				this._anchorRect.x = vector.x - 1f;
				this._anchorRect.y = vector.y - 1f;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.Right)
			{
				this._anchorRect.x = vector.x - 1f;
				this._anchorRect.y = vector.y - 0.5f;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.BottomRight)
			{
				this._anchorRect.x = vector.x - 1f;
				this._anchorRect.y = vector.y;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.Bottom)
			{
				this._anchorRect.x = vector.x - 0.5f;
				this._anchorRect.y = vector.y;
			}
			else if (this.anchorReference == RectTransformSetAnchorRectPosition.AnchorReference.Center)
			{
				this._anchorRect.x = vector.x - 0.5f;
				this._anchorRect.y = vector.y - 0.5f;
			}
			this._rt.anchorMin = this._anchorRect.min;
			this._rt.anchorMax = this._anchorRect.max;
		}

		// Token: 0x04007151 RID: 29009
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007152 RID: 29010
		[Tooltip("The reference for the given position")]
		public RectTransformSetAnchorRectPosition.AnchorReference anchorReference;

		// Token: 0x04007153 RID: 29011
		[Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
		public FsmBool normalized;

		// Token: 0x04007154 RID: 29012
		[Tooltip("The Vector2 position, and/or set individual axis below.")]
		public FsmVector2 anchor;

		// Token: 0x04007155 RID: 29013
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Anchor X position.")]
		public FsmFloat x;

		// Token: 0x04007156 RID: 29014
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Anchor Y position.")]
		public FsmFloat y;

		// Token: 0x04007157 RID: 29015
		private RectTransform _rt;

		// Token: 0x04007158 RID: 29016
		private Rect _anchorRect;

		// Token: 0x02001BBB RID: 7099
		public enum AnchorReference
		{
			// Token: 0x04009E6C RID: 40556
			TopLeft,
			// Token: 0x04009E6D RID: 40557
			Top,
			// Token: 0x04009E6E RID: 40558
			TopRight,
			// Token: 0x04009E6F RID: 40559
			Right,
			// Token: 0x04009E70 RID: 40560
			BottomRight,
			// Token: 0x04009E71 RID: 40561
			Bottom,
			// Token: 0x04009E72 RID: 40562
			BottomLeft,
			// Token: 0x04009E73 RID: 40563
			Left,
			// Token: 0x04009E74 RID: 40564
			Center
		}
	}
}
