using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001020 RID: 4128
	[ActionCategory("RectTransform")]
	[Tooltip("Get the normalized position in this RectTransform that it rotates around.")]
	public class RectTransformGetPivot : BaseUpdateAction
	{
		// Token: 0x0600715D RID: 29021 RVA: 0x0022E1D4 File Offset: 0x0022C3D4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.pivot = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x0600715E RID: 29022 RVA: 0x0022E1F8 File Offset: 0x0022C3F8
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

		// Token: 0x0600715F RID: 29023 RVA: 0x0022E240 File Offset: 0x0022C440
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007160 RID: 29024 RVA: 0x0022E248 File Offset: 0x0022C448
		private void DoGetValues()
		{
			if (!this.pivot.IsNone)
			{
				this.pivot.Value = this._rt.pivot;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.pivot.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.pivot.y;
			}
		}

		// Token: 0x04007104 RID: 28932
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007105 RID: 28933
		[Tooltip("The pivot")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 pivot;

		// Token: 0x04007106 RID: 28934
		[Tooltip("The x component of the pivot")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x04007107 RID: 28935
		[Tooltip("The y component of the pivot")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x04007108 RID: 28936
		private RectTransform _rt;
	}
}
