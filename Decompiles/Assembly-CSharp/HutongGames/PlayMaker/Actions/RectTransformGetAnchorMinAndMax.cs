using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200101B RID: 4123
	[ActionCategory("RectTransform")]
	[Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
	public class RectTransformGetAnchorMinAndMax : BaseUpdateAction
	{
		// Token: 0x06007144 RID: 28996 RVA: 0x0022DB3C File Offset: 0x0022BD3C
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.anchorMax = null;
			this.anchorMin = null;
			this.xMax = null;
			this.yMax = null;
			this.xMin = null;
			this.yMin = null;
		}

		// Token: 0x06007145 RID: 28997 RVA: 0x0022DB78 File Offset: 0x0022BD78
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

		// Token: 0x06007146 RID: 28998 RVA: 0x0022DBC0 File Offset: 0x0022BDC0
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007147 RID: 28999 RVA: 0x0022DBC8 File Offset: 0x0022BDC8
		private void DoGetValues()
		{
			if (!this.anchorMax.IsNone)
			{
				this.anchorMax.Value = this._rt.anchorMax;
			}
			if (!this.anchorMin.IsNone)
			{
				this.anchorMin.Value = this._rt.anchorMax;
			}
			if (!this.xMax.IsNone)
			{
				this.xMax.Value = this._rt.anchorMax.x;
			}
			if (!this.yMax.IsNone)
			{
				this.yMax.Value = this._rt.anchorMax.y;
			}
			if (!this.xMin.IsNone)
			{
				this.xMin.Value = this._rt.anchorMin.x;
			}
			if (!this.yMin.IsNone)
			{
				this.yMin.Value = this._rt.anchorMin.y;
			}
		}

		// Token: 0x040070E4 RID: 28900
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070E5 RID: 28901
		[Tooltip("The Vector2 anchor max. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMax;

		// Token: 0x040070E6 RID: 28902
		[Tooltip("The Vector2 anchor min. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMin;

		// Token: 0x040070E7 RID: 28903
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat xMax;

		// Token: 0x040070E8 RID: 28904
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat yMax;

		// Token: 0x040070E9 RID: 28905
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat xMin;

		// Token: 0x040070EA RID: 28906
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat yMin;

		// Token: 0x040070EB RID: 28907
		private RectTransform _rt;
	}
}
