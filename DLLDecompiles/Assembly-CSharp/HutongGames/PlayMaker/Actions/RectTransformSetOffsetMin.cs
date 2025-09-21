using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200102F RID: 4143
	[ActionCategory("RectTransform")]
	[Tooltip("The offset of the lower left corner of the rectangle relative to the lower left anchor.")]
	public class RectTransformSetOffsetMin : BaseUpdateAction
	{
		// Token: 0x060071A8 RID: 29096 RVA: 0x0022F8C4 File Offset: 0x0022DAC4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.offsetMin = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060071A9 RID: 29097 RVA: 0x0022F900 File Offset: 0x0022DB00
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetOffsetMin();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071AA RID: 29098 RVA: 0x0022F948 File Offset: 0x0022DB48
		public override void OnActionUpdate()
		{
			this.DoSetOffsetMin();
		}

		// Token: 0x060071AB RID: 29099 RVA: 0x0022F950 File Offset: 0x0022DB50
		private void DoSetOffsetMin()
		{
			Vector2 value = this._rt.offsetMin;
			if (!this.offsetMin.IsNone)
			{
				value = this.offsetMin.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this._rt.offsetMin = value;
		}

		// Token: 0x0400716C RID: 29036
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400716D RID: 29037
		[Tooltip("The Vector2 offsetMin. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 offsetMin;

		// Token: 0x0400716E RID: 29038
		[Tooltip("Setting only the x value. Overrides offsetMin x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x0400716F RID: 29039
		[Tooltip("Setting only the x value. Overrides offsetMin y value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x04007170 RID: 29040
		private RectTransform _rt;
	}
}
