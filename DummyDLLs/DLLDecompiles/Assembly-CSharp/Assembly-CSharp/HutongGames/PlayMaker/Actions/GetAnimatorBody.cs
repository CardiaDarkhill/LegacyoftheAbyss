using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE3 RID: 3555
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the avatar body mass center position and rotation. Optionally accepts a GameObject to get the body transform. \nThe position and rotation are local to the GameObject")]
	public class GetAnimatorBody : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x060066C2 RID: 26306 RVA: 0x00208550 File Offset: 0x00206750
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066C3 RID: 26307 RVA: 0x00208558 File Offset: 0x00206758
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.bodyPosition = null;
			this.bodyRotation = null;
			this.bodyGameObject = null;
			this.everyFrame = false;
			this.everyFrameOption = FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK;
		}

		// Token: 0x060066C4 RID: 26308 RVA: 0x0020858A File Offset: 0x0020678A
		public override void OnEnter()
		{
			this.everyFrameOption = FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK;
		}

		// Token: 0x060066C5 RID: 26309 RVA: 0x00208593 File Offset: 0x00206793
		public override void OnActionUpdate()
		{
			this.DoGetBodyPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066C6 RID: 26310 RVA: 0x002085AC File Offset: 0x002067AC
		private void DoGetBodyPosition()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.bodyPosition.Value = this.animator.bodyPosition;
			this.bodyRotation.Value = this.animator.bodyRotation;
			if (this.cachedBodyGameObject != this.bodyGameObject.Value)
			{
				this.cachedBodyGameObject = this.bodyGameObject.Value;
				this._transform = ((this.cachedBodyGameObject != null) ? this.cachedBodyGameObject.transform : null);
			}
			if (this._transform != null)
			{
				this._transform.position = this.animator.bodyPosition;
				this._transform.rotation = this.animator.bodyRotation;
			}
		}

		// Token: 0x060066C7 RID: 26311 RVA: 0x0020868A File Offset: 0x0020688A
		public override string ErrorCheck()
		{
			if (this.everyFrameOption != FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK)
			{
				return "Getting Body Position should only be done in OnAnimatorIK";
			}
			return string.Empty;
		}

		// Token: 0x04006616 RID: 26134
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006617 RID: 26135
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmVector3 bodyPosition;

		// Token: 0x04006618 RID: 26136
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmQuaternion bodyRotation;

		// Token: 0x04006619 RID: 26137
		[Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

		// Token: 0x0400661A RID: 26138
		private GameObject cachedBodyGameObject;

		// Token: 0x0400661B RID: 26139
		private Transform _transform;
	}
}
