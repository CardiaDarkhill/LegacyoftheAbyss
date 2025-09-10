using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200101D RID: 4125
	[ActionCategory("RectTransform")]
	[Tooltip("Gets the local rotation of this RectTransform.")]
	public class RectTransformGetLocalRotation : BaseUpdateAction
	{
		// Token: 0x0600714E RID: 29006 RVA: 0x0022DE68 File Offset: 0x0022C068
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.rotation = new FsmVector3
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x0600714F RID: 29007 RVA: 0x0022DECC File Offset: 0x0022C0CC
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

		// Token: 0x06007150 RID: 29008 RVA: 0x0022DF14 File Offset: 0x0022C114
		public override void OnActionUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007151 RID: 29009 RVA: 0x0022DF1C File Offset: 0x0022C11C
		private void DoGetValues()
		{
			if (this._rt == null)
			{
				return;
			}
			if (!this.rotation.IsNone)
			{
				this.rotation.Value = this._rt.eulerAngles;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this._rt.eulerAngles.x;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this._rt.eulerAngles.y;
			}
			if (!this.z.IsNone)
			{
				this.z.Value = this._rt.eulerAngles.z;
			}
		}

		// Token: 0x040070F4 RID: 28916
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070F5 RID: 28917
		[Tooltip("The rotation")]
		public FsmVector3 rotation;

		// Token: 0x040070F6 RID: 28918
		[Tooltip("The x component of the rotation")]
		public FsmFloat x;

		// Token: 0x040070F7 RID: 28919
		[Tooltip("The y component of the rotation")]
		public FsmFloat y;

		// Token: 0x040070F8 RID: 28920
		[Tooltip("The z component of the rotation")]
		public FsmFloat z;

		// Token: 0x040070F9 RID: 28921
		private RectTransform _rt;
	}
}
