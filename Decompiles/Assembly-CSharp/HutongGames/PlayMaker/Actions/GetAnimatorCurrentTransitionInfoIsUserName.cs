using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEC RID: 3564
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Check the active Transition user-specified name on a specified layer.")]
	public class GetAnimatorCurrentTransitionInfoIsUserName : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x060066F3 RID: 26355 RVA: 0x00208EEB File Offset: 0x002070EB
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066F4 RID: 26356 RVA: 0x00208EF3 File Offset: 0x002070F3
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.userName = null;
			this.nameMatch = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
		}

		// Token: 0x060066F5 RID: 26357 RVA: 0x00208F25 File Offset: 0x00207125
		public override void OnEnter()
		{
			this.IsName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066F6 RID: 26358 RVA: 0x00208F3B File Offset: 0x0020713B
		public override void OnActionUpdate()
		{
			this.IsName();
		}

		// Token: 0x060066F7 RID: 26359 RVA: 0x00208F44 File Offset: 0x00207144
		private void IsName()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			bool flag = this.animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsUserName(this.userName.Value);
			if (!this.nameMatch.IsNone)
			{
				this.nameMatch.Value = flag;
			}
			base.Fsm.Event(flag ? this.nameMatchEvent : this.nameDoNotMatchEvent);
		}

		// Token: 0x0400664C RID: 26188
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400664D RID: 26189
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x0400664E RID: 26190
		[Tooltip("The user-specified name to check the transition against.")]
		public FsmString userName;

		// Token: 0x0400664F RID: 26191
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if name matches")]
		public FsmBool nameMatch;

		// Token: 0x04006650 RID: 26192
		[Tooltip("Event send if name matches")]
		public FsmEvent nameMatchEvent;

		// Token: 0x04006651 RID: 26193
		[Tooltip("Event send if name doesn't match")]
		public FsmEvent nameDoNotMatchEvent;
	}
}
