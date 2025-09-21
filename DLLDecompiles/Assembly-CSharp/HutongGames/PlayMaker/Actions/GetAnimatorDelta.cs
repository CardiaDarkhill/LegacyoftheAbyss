using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DED RID: 3565
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
	public class GetAnimatorDelta : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x060066F9 RID: 26361 RVA: 0x00208FD8 File Offset: 0x002071D8
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066FA RID: 26362 RVA: 0x00208FE0 File Offset: 0x002071E0
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.deltaPosition = null;
			this.deltaRotation = null;
		}

		// Token: 0x060066FB RID: 26363 RVA: 0x00208FFD File Offset: 0x002071FD
		public override void OnEnter()
		{
			this.DoGetDeltaPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066FC RID: 26364 RVA: 0x00209013 File Offset: 0x00207213
		public override void OnActionUpdate()
		{
			this.DoGetDeltaPosition();
		}

		// Token: 0x060066FD RID: 26365 RVA: 0x0020901C File Offset: 0x0020721C
		private void DoGetDeltaPosition()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.deltaPosition.Value = this.animator.deltaPosition;
			this.deltaRotation.Value = this.animator.deltaRotation;
		}

		// Token: 0x04006652 RID: 26194
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006653 RID: 26195
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar delta position for the last evaluated frame")]
		public FsmVector3 deltaPosition;

		// Token: 0x04006654 RID: 26196
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar delta position for the last evaluated frame")]
		public FsmQuaternion deltaRotation;
	}
}
