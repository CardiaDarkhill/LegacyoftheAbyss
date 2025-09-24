using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF6 RID: 3574
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if automatic matching is active. Can also send events")]
	public class GetAnimatorIsMatchingTarget : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x06006726 RID: 26406 RVA: 0x00209681 File Offset: 0x00207881
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006727 RID: 26407 RVA: 0x00209689 File Offset: 0x00207889
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.isMatchingActive = null;
			this.matchingActivatedEvent = null;
			this.matchingDeactivedEvent = null;
		}

		// Token: 0x06006728 RID: 26408 RVA: 0x002096AD File Offset: 0x002078AD
		public override void OnEnter()
		{
			this.DoCheckIsMatchingActive();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006729 RID: 26409 RVA: 0x002096C3 File Offset: 0x002078C3
		public override void OnActionUpdate()
		{
			this.DoCheckIsMatchingActive();
		}

		// Token: 0x0600672A RID: 26410 RVA: 0x002096CC File Offset: 0x002078CC
		private void DoCheckIsMatchingActive()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			bool isMatchingTarget = this.animator.isMatchingTarget;
			this.isMatchingActive.Value = isMatchingTarget;
			base.Fsm.Event(isMatchingTarget ? this.matchingActivatedEvent : this.matchingDeactivedEvent);
		}

		// Token: 0x04006678 RID: 26232
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006679 RID: 26233
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if automatic matching is active")]
		public FsmBool isMatchingActive;

		// Token: 0x0400667A RID: 26234
		[Tooltip("Event send if automatic matching is active")]
		public FsmEvent matchingActivatedEvent;

		// Token: 0x0400667B RID: 26235
		[Tooltip("Event send if automatic matching is not active")]
		public FsmEvent matchingDeactivedEvent;
	}
}
