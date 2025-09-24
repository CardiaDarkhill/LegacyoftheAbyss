using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200102C RID: 4140
	[ActionCategory("RectTransform")]
	[Tooltip("Set the local position of this RectTransform.")]
	public class RectTransformSetLocalPosition : BaseUpdateAction
	{
		// Token: 0x06007199 RID: 29081 RVA: 0x0022F490 File Offset: 0x0022D690
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.position2d = new FsmVector2
			{
				UseVariable = true
			};
			this.position = new FsmVector3
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

		// Token: 0x0600719A RID: 29082 RVA: 0x0022F504 File Offset: 0x0022D704
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != this.cachedGameObject)
			{
				this.cachedGameObject = ownerDefaultTarget;
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this.DoSetValues();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600719B RID: 29083 RVA: 0x0022F558 File Offset: 0x0022D758
		public override void OnActionUpdate()
		{
			this.DoSetValues();
		}

		// Token: 0x0600719C RID: 29084 RVA: 0x0022F560 File Offset: 0x0022D760
		private void DoSetValues()
		{
			if (this._rt == null)
			{
				return;
			}
			Vector3 localPosition = this._rt.localPosition;
			if (!this.position.IsNone)
			{
				localPosition = this.position.Value;
			}
			if (!this.position2d.IsNone)
			{
				localPosition.x = this.position2d.Value.x;
				localPosition.y = this.position2d.Value.y;
			}
			if (!this.x.IsNone)
			{
				localPosition.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				localPosition.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				localPosition.z = this.z.Value;
			}
			this._rt.localPosition = localPosition;
		}

		// Token: 0x04007159 RID: 29017
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400715A RID: 29018
		[Tooltip("The position. Set to none for no effect")]
		public FsmVector2 position2d;

		// Token: 0x0400715B RID: 29019
		[Tooltip("Or the 3d position. Set to none for no effect")]
		public FsmVector3 position;

		// Token: 0x0400715C RID: 29020
		[Tooltip("The x component of the position. Set to none for no effect")]
		public FsmFloat x;

		// Token: 0x0400715D RID: 29021
		[Tooltip("The y component of the position. Set to none for no effect")]
		public FsmFloat y;

		// Token: 0x0400715E RID: 29022
		[Tooltip("The z component of the position. Set to none for no effect")]
		public FsmFloat z;

		// Token: 0x0400715F RID: 29023
		private GameObject cachedGameObject;

		// Token: 0x04007160 RID: 29024
		private RectTransform _rt;
	}
}
