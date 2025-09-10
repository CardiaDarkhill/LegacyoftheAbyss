using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200101F RID: 4127
	[ActionCategory("RectTransform")]
	[Tooltip("Get the offset of the lower left corner of the rectangle relative to the lower left anchor")]
	public class RectTransformGetOffsetMin : BaseUpdateAction
	{
		// Token: 0x06007158 RID: 29016 RVA: 0x0022E0D8 File Offset: 0x0022C2D8
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.offsetMin = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x06007159 RID: 29017 RVA: 0x0022E0FC File Offset: 0x0022C2FC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoGetValues();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600715A RID: 29018 RVA: 0x0022E144 File Offset: 0x0022C344
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x0600715B RID: 29019 RVA: 0x0022E14C File Offset: 0x0022C34C
		private void DoGetValues()
		{
			if (!this.offsetMin.IsNone)
			{
				this.offsetMin.Value = this._rt.offsetMin;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.offsetMin.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.offsetMin.y;
			}
		}

		// Token: 0x040070FF RID: 28927
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007100 RID: 28928
		[Tooltip("The offsetMin")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 offsetMin;

		// Token: 0x04007101 RID: 28929
		[Tooltip("The x component of the offsetMin")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x04007102 RID: 28930
		[Tooltip("The y component of the offsetMin")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x04007103 RID: 28931
		private RectTransform _rt;
	}
}
