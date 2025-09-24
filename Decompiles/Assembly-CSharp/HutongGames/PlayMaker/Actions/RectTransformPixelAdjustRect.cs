using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001024 RID: 4132
	[ActionCategory("RectTransform")]
	[Tooltip("Given a rect transform, return the corner points in pixel accurate coordinates.")]
	public class RectTransformPixelAdjustRect : BaseUpdateAction
	{
		// Token: 0x06007171 RID: 29041 RVA: 0x0022E671 File Offset: 0x0022C871
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.canvas = new FsmGameObject
			{
				UseVariable = true
			};
			this.pixelRect = null;
		}

		// Token: 0x06007172 RID: 29042 RVA: 0x0022E69C File Offset: 0x0022C89C
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

		// Token: 0x06007173 RID: 29043 RVA: 0x0022E738 File Offset: 0x0022C938
		public override void OnActionUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007174 RID: 29044 RVA: 0x0022E740 File Offset: 0x0022C940
		private void DoAction()
		{
			this.pixelRect.Value = RectTransformUtility.PixelAdjustRect(this._rt, this._canvas);
		}

		// Token: 0x0400711B RID: 28955
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400711C RID: 28956
		[RequiredField]
		[CheckForComponent(typeof(Canvas))]
		[Tooltip("The canvas. Leave to none to use the canvas of the gameObject")]
		public FsmGameObject canvas;

		// Token: 0x0400711D RID: 28957
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("Pixel adjusted rect.")]
		[UIHint(UIHint.Variable)]
		public FsmRect pixelRect;

		// Token: 0x0400711E RID: 28958
		private RectTransform _rt;

		// Token: 0x0400711F RID: 28959
		private Canvas _canvas;
	}
}
