using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200102A RID: 4138
	[ActionCategory("RectTransform")]
	[Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
	public class RectTransformSetAnchorMinAndMax : BaseUpdateAction
	{
		// Token: 0x0600718F RID: 29071 RVA: 0x0022EED4 File Offset: 0x0022D0D4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.anchorMax = null;
			this.anchorMin = null;
			this.xMax = new FsmFloat
			{
				UseVariable = true
			};
			this.yMax = new FsmFloat
			{
				UseVariable = true
			};
			this.xMin = new FsmFloat
			{
				UseVariable = true
			};
			this.yMin = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007190 RID: 29072 RVA: 0x0022EF44 File Offset: 0x0022D144
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

		// Token: 0x06007191 RID: 29073 RVA: 0x0022EF8C File Offset: 0x0022D18C
		public override void OnActionUpdate()
		{
			this.DoSetAnchorMax();
		}

		// Token: 0x06007192 RID: 29074 RVA: 0x0022EF94 File Offset: 0x0022D194
		private void DoSetAnchorMax()
		{
			Vector2 value = this._rt.anchorMax;
			Vector2 value2 = this._rt.anchorMin;
			if (!this.anchorMax.IsNone)
			{
				value = this.anchorMax.Value;
				value2 = this.anchorMin.Value;
			}
			if (!this.xMax.IsNone)
			{
				value.x = this.xMax.Value;
			}
			if (!this.yMax.IsNone)
			{
				value.y = this.yMax.Value;
			}
			if (!this.xMin.IsNone)
			{
				value2.x = this.xMin.Value;
			}
			if (!this.yMin.IsNone)
			{
				value2.y = this.yMin.Value;
			}
			this._rt.anchorMax = value;
			this._rt.anchorMin = value2;
		}

		// Token: 0x04007149 RID: 29001
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400714A RID: 29002
		[Tooltip("The Vector2 anchor max. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMax;

		// Token: 0x0400714B RID: 29003
		[Tooltip("The Vector2 anchor min. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMin;

		// Token: 0x0400714C RID: 29004
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat xMax;

		// Token: 0x0400714D RID: 29005
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat yMax;

		// Token: 0x0400714E RID: 29006
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat xMin;

		// Token: 0x0400714F RID: 29007
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat yMin;

		// Token: 0x04007150 RID: 29008
		private RectTransform _rt;
	}
}
