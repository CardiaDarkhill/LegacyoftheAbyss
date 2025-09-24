using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001022 RID: 4130
	[ActionCategory("RectTransform")]
	[Tooltip("Get the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
	public class RectTransformGetSizeDelta : BaseUpdateAction
	{
		// Token: 0x06007167 RID: 29031 RVA: 0x0022E470 File Offset: 0x0022C670
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.sizeDelta = null;
			this.width = null;
			this.height = null;
		}

		// Token: 0x06007168 RID: 29032 RVA: 0x0022E494 File Offset: 0x0022C694
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

		// Token: 0x06007169 RID: 29033 RVA: 0x0022E4DC File Offset: 0x0022C6DC
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x0600716A RID: 29034 RVA: 0x0022E4E4 File Offset: 0x0022C6E4
		private void DoGetValues()
		{
			if (!this.sizeDelta.IsNone)
			{
				this.sizeDelta.Value = this._rt.sizeDelta;
			}
			if (!this.width.IsNone)
			{
				this.width.Value = this._rt.sizeDelta.x;
			}
			if (!this.height.IsNone)
			{
				this.height.Value = this._rt.sizeDelta.y;
			}
		}

		// Token: 0x04007110 RID: 28944
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007111 RID: 28945
		[Tooltip("The sizeDelta")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 sizeDelta;

		// Token: 0x04007112 RID: 28946
		[Tooltip("The x component of the sizeDelta, the width.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat width;

		// Token: 0x04007113 RID: 28947
		[Tooltip("The y component of the sizeDelta, the height")]
		[UIHint(UIHint.Variable)]
		public FsmFloat height;

		// Token: 0x04007114 RID: 28948
		private RectTransform _rt;
	}
}
