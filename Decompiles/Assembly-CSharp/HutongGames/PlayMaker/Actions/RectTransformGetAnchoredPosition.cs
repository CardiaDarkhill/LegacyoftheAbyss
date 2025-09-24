using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001018 RID: 4120
	[ActionCategory("RectTransform")]
	[Tooltip("Get the position of the pivot of this RectTransform relative to the anchor reference point.")]
	public class RectTransformGetAnchoredPosition : BaseUpdateAction
	{
		// Token: 0x06007135 RID: 28981 RVA: 0x0022D845 File Offset: 0x0022BA45
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.position = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x06007136 RID: 28982 RVA: 0x0022D86C File Offset: 0x0022BA6C
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

		// Token: 0x06007137 RID: 28983 RVA: 0x0022D8B4 File Offset: 0x0022BAB4
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007138 RID: 28984 RVA: 0x0022D8BC File Offset: 0x0022BABC
		private void DoGetValues()
		{
			if (!this.position.IsNone)
			{
				this.position.Value = this._rt.anchoredPosition;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.anchoredPosition.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.anchoredPosition.y;
			}
		}

		// Token: 0x040070D5 RID: 28885
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070D6 RID: 28886
		[Tooltip("The anchored Position")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 position;

		// Token: 0x040070D7 RID: 28887
		[Tooltip("The x component of the anchored Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x040070D8 RID: 28888
		[Tooltip("The y component of the anchored Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x040070D9 RID: 28889
		private RectTransform _rt;
	}
}
