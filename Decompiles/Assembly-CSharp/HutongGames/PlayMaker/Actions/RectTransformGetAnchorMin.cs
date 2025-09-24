using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200101A RID: 4122
	[ActionCategory("RectTransform")]
	[Tooltip("Get the normalized position in the parent RectTransform that the lower left corner is anchored to.")]
	public class RectTransformGetAnchorMin : BaseUpdateAction
	{
		// Token: 0x0600713F RID: 28991 RVA: 0x0022DA40 File Offset: 0x0022BC40
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.anchorMin = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x06007140 RID: 28992 RVA: 0x0022DA64 File Offset: 0x0022BC64
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

		// Token: 0x06007141 RID: 28993 RVA: 0x0022DAAC File Offset: 0x0022BCAC
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007142 RID: 28994 RVA: 0x0022DAB4 File Offset: 0x0022BCB4
		private void DoGetValues()
		{
			if (!this.anchorMin.IsNone)
			{
				this.anchorMin.Value = this._rt.anchorMin;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.anchorMin.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.anchorMin.y;
			}
		}

		// Token: 0x040070DF RID: 28895
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070E0 RID: 28896
		[Tooltip("The anchorMin")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 anchorMin;

		// Token: 0x040070E1 RID: 28897
		[Tooltip("The x component of the anchorMin")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x040070E2 RID: 28898
		[Tooltip("The y component of the anchorMin")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x040070E3 RID: 28899
		private RectTransform _rt;
	}
}
