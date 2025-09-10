using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001023 RID: 4131
	[ActionCategory("RectTransform")]
	[Tooltip("Convert a given point in screen space into a pixel correct point.")]
	public class RectTransformPixelAdjustPoint : BaseUpdateAction
	{
		// Token: 0x0600716C RID: 29036 RVA: 0x0022E56C File Offset: 0x0022C76C
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.canvas = new FsmGameObject
			{
				UseVariable = true
			};
			this.screenPoint = null;
			this.pixelPoint = null;
		}

		// Token: 0x0600716D RID: 29037 RVA: 0x0022E59C File Offset: 0x0022C79C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			GameObject value = this.canvas.Value;
			if (value != null)
			{
				this._canvas = value.GetComponent<Canvas>();
			}
			if (this._canvas == null && ownerDefaultTarget != null)
			{
				Graphic component = ownerDefaultTarget.GetComponent<Graphic>();
				if (component != null)
				{
					this._canvas = component.canvas;
				}
			}
			this.DoAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600716E RID: 29038 RVA: 0x0022E638 File Offset: 0x0022C838
		public override void OnActionUpdate()
		{
			this.DoAction();
		}

		// Token: 0x0600716F RID: 29039 RVA: 0x0022E640 File Offset: 0x0022C840
		private void DoAction()
		{
			this.pixelPoint.Value = RectTransformUtility.PixelAdjustPoint(this.screenPoint.Value, this._rt, this._canvas);
		}

		// Token: 0x04007115 RID: 28949
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007116 RID: 28950
		[RequiredField]
		[CheckForComponent(typeof(Canvas))]
		[Tooltip("The canvas. Leave to none to use the canvas of the gameObject")]
		public FsmGameObject canvas;

		// Token: 0x04007117 RID: 28951
		[Tooltip("The screen position.")]
		public FsmVector2 screenPoint;

		// Token: 0x04007118 RID: 28952
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("Pixel adjusted point from the screen position.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 pixelPoint;

		// Token: 0x04007119 RID: 28953
		private RectTransform _rt;

		// Token: 0x0400711A RID: 28954
		private Canvas _canvas;
	}
}
