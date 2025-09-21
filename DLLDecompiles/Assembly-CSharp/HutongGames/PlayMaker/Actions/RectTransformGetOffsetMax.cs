using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200101E RID: 4126
	[ActionCategory("RectTransform")]
	[Tooltip("Get the offset of the upper right corner of the rectangle relative to the upper right anchor")]
	public class RectTransformGetOffsetMax : BaseUpdateAction
	{
		// Token: 0x06007153 RID: 29011 RVA: 0x0022DFDB File Offset: 0x0022C1DB
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.offsetMax = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x06007154 RID: 29012 RVA: 0x0022E000 File Offset: 0x0022C200
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

		// Token: 0x06007155 RID: 29013 RVA: 0x0022E048 File Offset: 0x0022C248
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007156 RID: 29014 RVA: 0x0022E050 File Offset: 0x0022C250
		private void DoGetValues()
		{
			if (!this.offsetMax.IsNone)
			{
				this.offsetMax.Value = this._rt.offsetMax;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.offsetMax.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.offsetMax.y;
			}
		}

		// Token: 0x040070FA RID: 28922
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070FB RID: 28923
		[Tooltip("The offsetMax")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 offsetMax;

		// Token: 0x040070FC RID: 28924
		[Tooltip("The x component of the offsetMax")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x040070FD RID: 28925
		[Tooltip("The y component of the offsetMax")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x040070FE RID: 28926
		private RectTransform _rt;
	}
}
