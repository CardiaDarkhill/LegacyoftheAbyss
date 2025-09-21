using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001032 RID: 4146
	[ActionCategory("RectTransform")]
	[Tooltip("Set the screen rect of a RectTransform using 2 Vector2 points.")]
	public class RectTransformSetScreenRectFromPoints : BaseUpdateAction
	{
		// Token: 0x060071B8 RID: 29112 RVA: 0x0022FD04 File Offset: 0x0022DF04
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.point1 = new FsmVector2
			{
				UseVariable = true
			};
			this.point2 = new FsmVector2
			{
				UseVariable = true
			};
			this.normalized = null;
			this.storeScreenRect = null;
		}

		// Token: 0x060071B9 RID: 29113 RVA: 0x0022FD50 File Offset: 0x0022DF50
		private bool UpdateCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != this.cachedGameObject)
			{
				this.cachedGameObject = ownerDefaultTarget;
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
				this.rootCanvas = ownerDefaultTarget.transform.GetComponentInParent<Canvas>().rootCanvas;
				this.rootRectTransform = this.rootCanvas.GetComponent<RectTransform>();
				this.canvasCamera = ((this.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : this.rootCanvas.worldCamera);
			}
			return this._rt != null;
		}

		// Token: 0x060071BA RID: 29114 RVA: 0x0022FDE4 File Offset: 0x0022DFE4
		public override void OnEnter()
		{
			if (!this.UpdateCache())
			{
				base.Finish();
				return;
			}
			this.DoSetValues();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071BB RID: 29115 RVA: 0x0022FE09 File Offset: 0x0022E009
		public override void OnActionUpdate()
		{
			this.DoSetValues();
		}

		// Token: 0x060071BC RID: 29116 RVA: 0x0022FE14 File Offset: 0x0022E014
		private void DoSetValues()
		{
			if (!this.UpdateCache())
			{
				base.Finish();
				return;
			}
			Rect value = new Rect
			{
				x = Mathf.Min(this.point1.Value.x, this.point2.Value.x),
				y = Mathf.Min(this.point1.Value.y, this.point2.Value.y),
				width = Mathf.Abs(this.point2.Value.x - this.point1.Value.x),
				height = Mathf.Abs(this.point2.Value.y - this.point1.Value.y)
			};
			this.storeScreenRect.Value = value;
			Vector2 min = value.min;
			Vector2 size = value.size;
			if (this.normalized.Value)
			{
				min.x *= (float)Screen.width;
				min.y *= (float)Screen.height;
				size.x *= (float)Screen.width;
				size.y *= (float)Screen.height;
			}
			Vector2 v;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rootRectTransform, min, this.canvasCamera, out v);
			this._rt.localPosition = v;
			this._rt.sizeDelta = size;
		}

		// Token: 0x04007180 RID: 29056
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007181 RID: 29057
		[RequiredField]
		[Tooltip("The screen position of the first point to define the rect.")]
		public FsmVector2 point1;

		// Token: 0x04007182 RID: 29058
		[RequiredField]
		[Tooltip("The screen position of the second point to define the rect.")]
		public FsmVector2 point2;

		// Token: 0x04007183 RID: 29059
		[Tooltip("Screen points use normalized coordinates (0-1).")]
		public FsmBool normalized;

		// Token: 0x04007184 RID: 29060
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the resulting screen rect.")]
		public FsmRect storeScreenRect;

		// Token: 0x04007185 RID: 29061
		private GameObject cachedGameObject;

		// Token: 0x04007186 RID: 29062
		private RectTransform _rt;

		// Token: 0x04007187 RID: 29063
		private Canvas rootCanvas;

		// Token: 0x04007188 RID: 29064
		private RectTransform rootRectTransform;

		// Token: 0x04007189 RID: 29065
		private Camera canvasCamera;
	}
}
