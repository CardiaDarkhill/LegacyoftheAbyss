using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200101C RID: 4124
	[ActionCategory("RectTransform")]
	[Tooltip("Get the Local position of this RectTransform. This is Screen Space values using the anchoring as reference, so 0,0 is the center of the screen if the anchor is te center of the screen.")]
	public class RectTransformGetLocalPosition : BaseUpdateAction
	{
		// Token: 0x06007149 RID: 29001 RVA: 0x0022DCC3 File Offset: 0x0022BEC3
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.reference = RectTransformGetLocalPosition.LocalPositionReference.Anchor;
			this.position = null;
			this.position2d = null;
			this.x = null;
			this.y = null;
			this.z = null;
		}

		// Token: 0x0600714A RID: 29002 RVA: 0x0022DCFC File Offset: 0x0022BEFC
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

		// Token: 0x0600714B RID: 29003 RVA: 0x0022DD44 File Offset: 0x0022BF44
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x0600714C RID: 29004 RVA: 0x0022DD4C File Offset: 0x0022BF4C
		private void DoGetValues()
		{
			if (this._rt == null)
			{
				return;
			}
			Vector3 localPosition = this._rt.localPosition;
			if (this.reference == RectTransformGetLocalPosition.LocalPositionReference.CenterPosition)
			{
				localPosition.x += this._rt.rect.center.x;
				localPosition.y += this._rt.rect.center.y;
			}
			if (!this.position.IsNone)
			{
				this.position.Value = localPosition;
			}
			if (!this.position2d.IsNone)
			{
				this.position2d.Value = new Vector2(localPosition.x, localPosition.y);
			}
			if (!this.x.IsNone)
			{
				this.x.Value = localPosition.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = localPosition.y;
			}
			if (!this.z.IsNone)
			{
				this.z.Value = localPosition.z;
			}
		}

		// Token: 0x040070EC RID: 28908
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070ED RID: 28909
		[Tooltip("Get local position relative to Anchor or Center.")]
		public RectTransformGetLocalPosition.LocalPositionReference reference;

		// Token: 0x040070EE RID: 28910
		[Tooltip("The position")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 position;

		// Token: 0x040070EF RID: 28911
		[Tooltip("The position in a Vector 2d ")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 position2d;

		// Token: 0x040070F0 RID: 28912
		[Tooltip("The x component of the Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x040070F1 RID: 28913
		[Tooltip("The y component of the Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x040070F2 RID: 28914
		[Tooltip("The z component of the Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat z;

		// Token: 0x040070F3 RID: 28915
		private RectTransform _rt;

		// Token: 0x02001BBA RID: 7098
		public enum LocalPositionReference
		{
			// Token: 0x04009E69 RID: 40553
			Anchor,
			// Token: 0x04009E6A RID: 40554
			CenterPosition
		}
	}
}
