using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF5 RID: 3573
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if the specified layer is in a transition. Can also send events")]
	public class GetAnimatorIsLayerInTransition : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06006720 RID: 26400 RVA: 0x002095B5 File Offset: 0x002077B5
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006721 RID: 26401 RVA: 0x002095BD File Offset: 0x002077BD
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.isInTransition = null;
			this.isInTransitionEvent = null;
			this.isNotInTransitionEvent = null;
		}

		// Token: 0x06006722 RID: 26402 RVA: 0x002095E1 File Offset: 0x002077E1
		public override void OnEnter()
		{
			this.DoCheckIsInTransition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006723 RID: 26403 RVA: 0x002095F7 File Offset: 0x002077F7
		public override void OnActionUpdate()
		{
			this.DoCheckIsInTransition();
		}

		// Token: 0x06006724 RID: 26404 RVA: 0x00209600 File Offset: 0x00207800
		private void DoCheckIsInTransition()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			bool flag = this.animator.IsInTransition(this.layerIndex.Value);
			if (!this.isInTransition.IsNone)
			{
				this.isInTransition.Value = flag;
			}
			base.Fsm.Event(flag ? this.isInTransitionEvent : this.isNotInTransitionEvent);
		}

		// Token: 0x04006673 RID: 26227
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006674 RID: 26228
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x04006675 RID: 26229
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if automatic matching is active")]
		public FsmBool isInTransition;

		// Token: 0x04006676 RID: 26230
		[Tooltip("Event send if automatic matching is active")]
		public FsmEvent isInTransitionEvent;

		// Token: 0x04006677 RID: 26231
		[Tooltip("Event send if automatic matching is not active")]
		public FsmEvent isNotInTransitionEvent;
	}
}
