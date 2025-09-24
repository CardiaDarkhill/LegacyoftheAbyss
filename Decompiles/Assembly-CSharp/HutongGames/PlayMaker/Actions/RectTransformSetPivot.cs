using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001030 RID: 4144
	[ActionCategory("RectTransform")]
	[Tooltip("The normalized position in this RectTransform that it rotates around.")]
	public class RectTransformSetPivot : BaseUpdateAction
	{
		// Token: 0x060071AD RID: 29101 RVA: 0x0022F9D4 File Offset: 0x0022DBD4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.pivot = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060071AE RID: 29102 RVA: 0x0022FA10 File Offset: 0x0022DC10
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetPivotPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071AF RID: 29103 RVA: 0x0022FA58 File Offset: 0x0022DC58
		public override void OnActionUpdate()
		{
			this.DoSetPivotPosition();
		}

		// Token: 0x060071B0 RID: 29104 RVA: 0x0022FA60 File Offset: 0x0022DC60
		private void DoSetPivotPosition()
		{
			Vector2 value = this._rt.pivot;
			if (!this.pivot.IsNone)
			{
				value = this.pivot.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this._rt.pivot = value;
		}

		// Token: 0x04007171 RID: 29041
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007172 RID: 29042
		[Tooltip("The Vector2 pivot. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 pivot;

		// Token: 0x04007173 RID: 29043
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides pivot x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x04007174 RID: 29044
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides pivot y value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x04007175 RID: 29045
		private RectTransform _rt;
	}
}
