using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E04 RID: 3588
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the position and rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).\nThe position and rotation are only valid when a frame has being evaluated after the SetTarget call")]
	public class GetAnimatorTarget : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x0600676C RID: 26476 RVA: 0x0020A031 File Offset: 0x00208231
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x0600676D RID: 26477 RVA: 0x0020A039 File Offset: 0x00208239
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.targetPosition = null;
			this.targetRotation = null;
			this.targetGameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x0600676E RID: 26478 RVA: 0x0020A064 File Offset: 0x00208264
		public override void OnEnter()
		{
			this.DoGetTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600676F RID: 26479 RVA: 0x0020A07A File Offset: 0x0020827A
		public override void OnActionUpdate()
		{
			this.DoGetTarget();
		}

		// Token: 0x06006770 RID: 26480 RVA: 0x0020A084 File Offset: 0x00208284
		private void DoGetTarget()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedTargetGameObject != this.targetGameObject.Value)
			{
				this.cachedTargetGameObject = this.targetGameObject.Value;
				this._transform = ((this.cachedTargetGameObject != null) ? this.cachedTargetGameObject.transform : null);
			}
			this.targetPosition.Value = this.animator.targetPosition;
			this.targetRotation.Value = this.animator.targetRotation;
			if (this._transform != null)
			{
				this._transform.position = this.animator.targetPosition;
				this._transform.rotation = this.animator.targetRotation;
			}
		}

		// Token: 0x040066B0 RID: 26288
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066B1 RID: 26289
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The target position")]
		public FsmVector3 targetPosition;

		// Token: 0x040066B2 RID: 26290
		[UIHint(UIHint.Variable)]
		[Tooltip("The target rotation")]
		public FsmQuaternion targetRotation;

		// Token: 0x040066B3 RID: 26291
		[Tooltip("If set, apply the position and rotation to this gameObject")]
		public FsmGameObject targetGameObject;

		// Token: 0x040066B4 RID: 26292
		private GameObject cachedTargetGameObject;

		// Token: 0x040066B5 RID: 26293
		private Transform _transform;
	}
}
