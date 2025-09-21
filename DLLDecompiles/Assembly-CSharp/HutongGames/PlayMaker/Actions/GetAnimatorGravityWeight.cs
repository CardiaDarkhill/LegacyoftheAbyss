using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF0 RID: 3568
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns The current gravity weight based on current animations that are played")]
	public class GetAnimatorGravityWeight : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD4 RID: 3028
		// (get) Token: 0x06006708 RID: 26376 RVA: 0x002091A0 File Offset: 0x002073A0
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006709 RID: 26377 RVA: 0x002091A8 File Offset: 0x002073A8
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.gravityWeight = null;
			this.everyFrame = false;
		}

		// Token: 0x0600670A RID: 26378 RVA: 0x002091C5 File Offset: 0x002073C5
		public override void OnEnter()
		{
			this.DoGetGravityWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600670B RID: 26379 RVA: 0x002091DB File Offset: 0x002073DB
		public override void OnActionUpdate()
		{
			this.DoGetGravityWeight();
		}

		// Token: 0x0600670C RID: 26380 RVA: 0x002091E3 File Offset: 0x002073E3
		private void DoGetGravityWeight()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.gravityWeight.Value = this.animator.gravityWeight;
		}

		// Token: 0x0400665C RID: 26204
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400665D RID: 26205
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The current gravity weight based on current animations that are played")]
		public FsmFloat gravityWeight;
	}
}
