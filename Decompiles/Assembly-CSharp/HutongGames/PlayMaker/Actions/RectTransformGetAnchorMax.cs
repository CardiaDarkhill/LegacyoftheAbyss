using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001019 RID: 4121
	[ActionCategory("RectTransform")]
	[Tooltip("Get the normalized position in the parent RectTransform that the upper right corner is anchored to.")]
	public class RectTransformGetAnchorMax : BaseUpdateAction
	{
		// Token: 0x0600713A RID: 28986 RVA: 0x0022D944 File Offset: 0x0022BB44
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.anchorMax = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x0600713B RID: 28987 RVA: 0x0022D968 File Offset: 0x0022BB68
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

		// Token: 0x0600713C RID: 28988 RVA: 0x0022D9B0 File Offset: 0x0022BBB0
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x0600713D RID: 28989 RVA: 0x0022D9B8 File Offset: 0x0022BBB8
		private void DoGetValues()
		{
			if (!this.anchorMax.IsNone)
			{
				this.anchorMax.Value = this._rt.anchorMax;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.anchorMax.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.anchorMax.y;
			}
		}

		// Token: 0x040070DA RID: 28890
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070DB RID: 28891
		[Tooltip("The anchorMax")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 anchorMax;

		// Token: 0x040070DC RID: 28892
		[Tooltip("The x component of the anchorMax")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x040070DD RID: 28893
		[Tooltip("The y component of the anchorMax")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x040070DE RID: 28894
		private RectTransform _rt;
	}
}
