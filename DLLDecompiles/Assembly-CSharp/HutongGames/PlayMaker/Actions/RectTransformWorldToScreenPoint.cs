using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001034 RID: 4148
	[ActionCategory("RectTransform")]
	[Tooltip("RectTransforms position from world space into screen space. Leave the camera to none for default behavior.")]
	public class RectTransformWorldToScreenPoint : BaseUpdateAction
	{
		// Token: 0x060071C3 RID: 29123 RVA: 0x002300A4 File Offset: 0x0022E2A4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.camera = new FsmOwnerDefault();
			this.camera.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
			this.camera.GameObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.screenPoint = null;
			this.screenX = null;
			this.screenY = null;
			this.everyFrame = false;
		}

		// Token: 0x060071C4 RID: 29124 RVA: 0x00230108 File Offset: 0x0022E308
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			if (base.Fsm.GetOwnerDefaultTarget(this.camera) != null)
			{
				this._cam = ownerDefaultTarget.GetComponent<Camera>();
			}
			this.DoWorldToScreenPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071C5 RID: 29125 RVA: 0x00230175 File Offset: 0x0022E375
		public override void OnActionUpdate()
		{
			this.DoWorldToScreenPoint();
		}

		// Token: 0x060071C6 RID: 29126 RVA: 0x00230180 File Offset: 0x0022E380
		private void DoWorldToScreenPoint()
		{
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(this._cam, this._rt.position);
			if (this.normalize.Value)
			{
				vector.x /= (float)Screen.width;
				vector.y /= (float)Screen.height;
			}
			this.screenPoint.Value = vector;
			this.screenX.Value = vector.x;
			this.screenY.Value = vector.y;
		}

		// Token: 0x0400718F RID: 29071
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007190 RID: 29072
		[CheckForComponent(typeof(Camera))]
		[Tooltip("The camera to perform the calculation. Leave as None for default behavior.")]
		public FsmOwnerDefault camera;

		// Token: 0x04007191 RID: 29073
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
		public FsmVector3 screenPoint;

		// Token: 0x04007192 RID: 29074
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen X position in a Float Variable.")]
		public FsmFloat screenX;

		// Token: 0x04007193 RID: 29075
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen Y position in a Float Variable.")]
		public FsmFloat screenY;

		// Token: 0x04007194 RID: 29076
		[Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public FsmBool normalize;

		// Token: 0x04007195 RID: 29077
		private RectTransform _rt;

		// Token: 0x04007196 RID: 29078
		private Camera _cam;
	}
}
