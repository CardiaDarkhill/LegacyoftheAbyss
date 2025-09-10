using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001028 RID: 4136
	[ActionCategory("RectTransform")]
	[Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
	public class RectTransformSetAnchorMax : BaseUpdateAction
	{
		// Token: 0x06007185 RID: 29061 RVA: 0x0022ECB4 File Offset: 0x0022CEB4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.anchorMax = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007186 RID: 29062 RVA: 0x0022ECF0 File Offset: 0x0022CEF0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetAnchorMax();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007187 RID: 29063 RVA: 0x0022ED38 File Offset: 0x0022CF38
		public override void OnActionUpdate()
		{
			this.DoSetAnchorMax();
		}

		// Token: 0x06007188 RID: 29064 RVA: 0x0022ED40 File Offset: 0x0022CF40
		private void DoSetAnchorMax()
		{
			Vector2 value = this._rt.anchorMax;
			if (!this.anchorMax.IsNone)
			{
				value = this.anchorMax.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this._rt.anchorMax = value;
		}

		// Token: 0x0400713F RID: 28991
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007140 RID: 28992
		[Tooltip("The Vector2 anchor. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMax;

		// Token: 0x04007141 RID: 28993
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x04007142 RID: 28994
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x04007143 RID: 28995
		private RectTransform _rt;
	}
}
