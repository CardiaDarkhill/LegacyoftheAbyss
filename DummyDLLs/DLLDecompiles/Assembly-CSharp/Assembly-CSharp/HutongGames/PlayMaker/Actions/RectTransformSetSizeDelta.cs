using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001033 RID: 4147
	[ActionCategory("RectTransform")]
	[Tooltip("Set the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
	public class RectTransformSetSizeDelta : BaseUpdateAction
	{
		// Token: 0x060071BE RID: 29118 RVA: 0x0022FF94 File Offset: 0x0022E194
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.sizeDelta = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060071BF RID: 29119 RVA: 0x0022FFD0 File Offset: 0x0022E1D0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetSizeDelta();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071C0 RID: 29120 RVA: 0x00230018 File Offset: 0x0022E218
		public override void OnActionUpdate()
		{
			this.DoSetSizeDelta();
		}

		// Token: 0x060071C1 RID: 29121 RVA: 0x00230020 File Offset: 0x0022E220
		private void DoSetSizeDelta()
		{
			Vector2 value = this._rt.sizeDelta;
			if (!this.sizeDelta.IsNone)
			{
				value = this.sizeDelta.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this._rt.sizeDelta = value;
		}

		// Token: 0x0400718A RID: 29066
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400718B RID: 29067
		[Tooltip("TheVector2 sizeDelta. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 sizeDelta;

		// Token: 0x0400718C RID: 29068
		[Tooltip("Setting only the x value. Overrides sizeDelta x value if set. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x0400718D RID: 29069
		[Tooltip("Setting only the x value. Overrides sizeDelta y value if set. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x0400718E RID: 29070
		private RectTransform _rt;
	}
}
