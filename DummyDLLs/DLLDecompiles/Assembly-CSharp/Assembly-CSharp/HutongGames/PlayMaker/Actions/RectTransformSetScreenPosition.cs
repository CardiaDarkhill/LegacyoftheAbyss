using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001031 RID: 4145
	[ActionCategory("RectTransform")]
	[Tooltip("Set the screen position of this RectTransform.")]
	public class RectTransformSetScreenPosition : BaseUpdateAction
	{
		// Token: 0x060071B2 RID: 29106 RVA: 0x0022FAE4 File Offset: 0x0022DCE4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.screenPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.normalized = null;
		}

		// Token: 0x060071B3 RID: 29107 RVA: 0x0022FB3C File Offset: 0x0022DD3C
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

		// Token: 0x060071B4 RID: 29108 RVA: 0x0022FBD0 File Offset: 0x0022DDD0
		public override void OnEnter()
		{
			if (!this.UpdateCache())
			{
				base.Finish();
				return;
			}
			this.DoSetScreenPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071B5 RID: 29109 RVA: 0x0022FBF5 File Offset: 0x0022DDF5
		public override void OnActionUpdate()
		{
			this.DoSetScreenPosition();
		}

		// Token: 0x060071B6 RID: 29110 RVA: 0x0022FC00 File Offset: 0x0022DE00
		private void DoSetScreenPosition()
		{
			if (!this.UpdateCache())
			{
				base.Finish();
				return;
			}
			Vector2 screenPoint = this.screenPosition.Value;
			if (this.screenPosition.IsNone && (this.x.IsNone || this.y.IsNone))
			{
				screenPoint = RectTransformUtility.WorldToScreenPoint(this.canvasCamera, this._rt.position);
			}
			if (!this.x.IsNone)
			{
				screenPoint.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				screenPoint.y = this.y.Value;
			}
			if (this.normalized.Value)
			{
				screenPoint.x *= (float)Screen.width;
				screenPoint.y *= (float)Screen.height;
			}
			Vector2 v;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rootRectTransform, screenPoint, this.canvasCamera, out v);
			this._rt.localPosition = v;
		}

		// Token: 0x04007176 RID: 29046
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007177 RID: 29047
		[Tooltip("The position in screen coordinates.")]
		public FsmVector2 screenPosition;

		// Token: 0x04007178 RID: 29048
		[Tooltip("Set the x component of the position. Set to None for no effect.")]
		public FsmFloat x;

		// Token: 0x04007179 RID: 29049
		[Tooltip("Set the y component of the position. Set to None for no effect.")]
		public FsmFloat y;

		// Token: 0x0400717A RID: 29050
		[Tooltip("Screen coordinates are normalized (0-1).")]
		public FsmBool normalized;

		// Token: 0x0400717B RID: 29051
		private GameObject cachedGameObject;

		// Token: 0x0400717C RID: 29052
		private RectTransform _rt;

		// Token: 0x0400717D RID: 29053
		private Canvas rootCanvas;

		// Token: 0x0400717E RID: 29054
		private RectTransform rootRectTransform;

		// Token: 0x0400717F RID: 29055
		private Camera canvasCamera;
	}
}
