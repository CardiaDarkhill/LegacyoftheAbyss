using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200102E RID: 4142
	[ActionCategory("RectTransform")]
	[Tooltip("\tThe offset of the upper right corner of the rectangle relative to the upper right anchor.")]
	public class RectTransformSetOffsetMax : BaseUpdateAction
	{
		// Token: 0x060071A3 RID: 29091 RVA: 0x0022F7B6 File Offset: 0x0022D9B6
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.offsetMax = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060071A4 RID: 29092 RVA: 0x0022F7F0 File Offset: 0x0022D9F0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetOffsetMax();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071A5 RID: 29093 RVA: 0x0022F838 File Offset: 0x0022DA38
		public override void OnActionUpdate()
		{
			this.DoSetOffsetMax();
		}

		// Token: 0x060071A6 RID: 29094 RVA: 0x0022F840 File Offset: 0x0022DA40
		private void DoSetOffsetMax()
		{
			Vector2 value = this._rt.offsetMax;
			if (!this.offsetMax.IsNone)
			{
				value = this.offsetMax.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this._rt.offsetMax = value;
		}

		// Token: 0x04007167 RID: 29031
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007168 RID: 29032
		[Tooltip("The Vector2 offsetMax. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 offsetMax;

		// Token: 0x04007169 RID: 29033
		[Tooltip("Setting only the x value. Overrides offsetMax x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x0400716A RID: 29034
		[Tooltip("Setting only the y value. Overrides offsetMax y value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x0400716B RID: 29035
		private RectTransform _rt;
	}
}
