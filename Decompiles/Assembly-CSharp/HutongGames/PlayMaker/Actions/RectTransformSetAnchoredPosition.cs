using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001027 RID: 4135
	[ActionCategory("RectTransform")]
	[Tooltip("The position of the pivot of this RectTransform relative to the anchor reference point.The anchor reference point is where the anchors are. If the anchor are not together, the four anchor positions are interpolated according to the pivot normalized values.")]
	public class RectTransformSetAnchoredPosition : BaseUpdateAction
	{
		// Token: 0x06007180 RID: 29056 RVA: 0x0022EBA3 File Offset: 0x0022CDA3
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.position = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007181 RID: 29057 RVA: 0x0022EBE0 File Offset: 0x0022CDE0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetAnchoredPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007182 RID: 29058 RVA: 0x0022EC28 File Offset: 0x0022CE28
		public override void OnActionUpdate()
		{
			this.DoSetAnchoredPosition();
		}

		// Token: 0x06007183 RID: 29059 RVA: 0x0022EC30 File Offset: 0x0022CE30
		private void DoSetAnchoredPosition()
		{
			Vector2 anchoredPosition = this._rt.anchoredPosition;
			if (!this.position.IsNone)
			{
				anchoredPosition = this.position.Value;
			}
			if (!this.x.IsNone)
			{
				anchoredPosition.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				anchoredPosition.y = this.y.Value;
			}
			this._rt.anchoredPosition = anchoredPosition;
		}

		// Token: 0x0400713A RID: 28986
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400713B RID: 28987
		[Tooltip("The Vector2 position. Set to none for no effect, and/or set individual axis below. ")]
		public FsmVector2 position;

		// Token: 0x0400713C RID: 28988
		[Tooltip("Setting only the x value. Overrides position x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x0400713D RID: 28989
		[Tooltip("Setting only the y value. Overrides position x value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x0400713E RID: 28990
		private RectTransform _rt;
	}
}
