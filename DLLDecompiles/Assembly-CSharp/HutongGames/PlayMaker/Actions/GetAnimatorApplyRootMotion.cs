using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE2 RID: 3554
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the value of ApplyRootMotion of an avatar. If true, root is controlled by animations")]
	public class GetAnimatorApplyRootMotion : ComponentAction<Animator>
	{
		// Token: 0x060066BF RID: 26303 RVA: 0x002084CA File Offset: 0x002066CA
		public override void Reset()
		{
			this.gameObject = null;
			this.rootMotionApplied = null;
			this.rootMotionIsAppliedEvent = null;
			this.rootMotionIsNotAppliedEvent = null;
		}

		// Token: 0x060066C0 RID: 26304 RVA: 0x002084E8 File Offset: 0x002066E8
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				bool applyRootMotion = this.cachedComponent.applyRootMotion;
				this.rootMotionApplied.Value = applyRootMotion;
				base.Fsm.Event(applyRootMotion ? this.rootMotionIsAppliedEvent : this.rootMotionIsNotAppliedEvent);
			}
			base.Finish();
		}

		// Token: 0x04006612 RID: 26130
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006613 RID: 26131
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Is the rootMotionapplied. If true, root is controlled by animations")]
		public FsmBool rootMotionApplied;

		// Token: 0x04006614 RID: 26132
		[Tooltip("Event send if the root motion is applied")]
		public FsmEvent rootMotionIsAppliedEvent;

		// Token: 0x04006615 RID: 26133
		[Tooltip("Event send if the root motion is not applied")]
		public FsmEvent rootMotionIsNotAppliedEvent;
	}
}
