using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001029 RID: 4137
	[ActionCategory("RectTransform")]
	[Tooltip("The normalized position in the parent RectTransform that the lower left corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
	public class RectTransformSetAnchorMin : BaseUpdateAction
	{
		// Token: 0x0600718A RID: 29066 RVA: 0x0022EDC4 File Offset: 0x0022CFC4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.anchorMin = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x0600718B RID: 29067 RVA: 0x0022EE00 File Offset: 0x0022D000
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetAnchorMin();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600718C RID: 29068 RVA: 0x0022EE48 File Offset: 0x0022D048
		public override void OnActionUpdate()
		{
			this.DoSetAnchorMin();
		}

		// Token: 0x0600718D RID: 29069 RVA: 0x0022EE50 File Offset: 0x0022D050
		private void DoSetAnchorMin()
		{
			Vector2 value = this._rt.anchorMin;
			if (!this.anchorMin.IsNone)
			{
				value = this.anchorMin.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this._rt.anchorMin = value;
		}

		// Token: 0x04007144 RID: 28996
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007145 RID: 28997
		[Tooltip("The Vector2 anchor. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMin;

		// Token: 0x04007146 RID: 28998
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x04007147 RID: 28999
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x04007148 RID: 29000
		private RectTransform _rt;
	}
}
