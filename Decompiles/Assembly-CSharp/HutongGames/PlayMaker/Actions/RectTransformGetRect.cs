using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001021 RID: 4129
	[ActionCategory("RectTransform")]
	[Tooltip("The calculated rectangle in the local space of the Transform.")]
	public class RectTransformGetRect : BaseUpdateAction
	{
		// Token: 0x06007162 RID: 29026 RVA: 0x0022E2D0 File Offset: 0x0022C4D0
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.rect = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.width = new FsmFloat
			{
				UseVariable = true
			};
			this.height = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007163 RID: 29027 RVA: 0x0022E33C File Offset: 0x0022C53C
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

		// Token: 0x06007164 RID: 29028 RVA: 0x0022E384 File Offset: 0x0022C584
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007165 RID: 29029 RVA: 0x0022E38C File Offset: 0x0022C58C
		private void DoGetValues()
		{
			if (!this.rect.IsNone)
			{
				this.rect.Value = this._rt.rect;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.rect.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.rect.y;
			}
			if (!this.width.IsNone)
			{
				this.width.Value = this._rt.rect.width;
			}
			if (!this.height.IsNone)
			{
				this.height.Value = this._rt.rect.height;
			}
		}

		// Token: 0x04007109 RID: 28937
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400710A RID: 28938
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Rect in a Rect variable.")]
		public FsmRect rect;

		// Token: 0x0400710B RID: 28939
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the x coordinate in a float variable.")]
		public FsmFloat x;

		// Token: 0x0400710C RID: 28940
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the y\u00a0coordinate in a float variable.")]
		public FsmFloat y;

		// Token: 0x0400710D RID: 28941
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the width in a float variable.")]
		public FsmFloat width;

		// Token: 0x0400710E RID: 28942
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the height in a float variable.")]
		public FsmFloat height;

		// Token: 0x0400710F RID: 28943
		private RectTransform _rt;
	}
}
