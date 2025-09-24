using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200102D RID: 4141
	[ActionCategory("RectTransform")]
	[Tooltip("Set the local rotation of this RectTransform.")]
	public class RectTransformSetLocalRotation : BaseUpdateAction
	{
		// Token: 0x0600719E RID: 29086 RVA: 0x0022F650 File Offset: 0x0022D850
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

		// Token: 0x0600719F RID: 29087 RVA: 0x0022F6B4 File Offset: 0x0022D8B4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetValues();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071A0 RID: 29088 RVA: 0x0022F6FC File Offset: 0x0022D8FC
		public override void OnActionUpdate()
		{
			this.DoSetValues();
		}

		// Token: 0x060071A1 RID: 29089 RVA: 0x0022F704 File Offset: 0x0022D904
		private void DoSetValues()
		{
			if (this._rt == null)
			{
				return;
			}
			Vector3 eulerAngles = this._rt.eulerAngles;
			if (!this.rotation.IsNone)
			{
				eulerAngles = this.rotation.Value;
			}
			if (!this.x.IsNone)
			{
				eulerAngles.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				eulerAngles.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				eulerAngles.z = this.z.Value;
			}
			this._rt.eulerAngles = eulerAngles;
		}

		// Token: 0x04007161 RID: 29025
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007162 RID: 29026
		[Tooltip("The rotation. Set to none for no effect")]
		public FsmVector3 rotation;

		// Token: 0x04007163 RID: 29027
		[Tooltip("The x component of the rotation. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x04007164 RID: 29028
		[Tooltip("The y component of the rotation. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x04007165 RID: 29029
		[Tooltip("The z component of the rotation. Set to none for no effect")]
		public FsmFloat z;

		// Token: 0x04007166 RID: 29030
		private RectTransform _rt;
	}
}
