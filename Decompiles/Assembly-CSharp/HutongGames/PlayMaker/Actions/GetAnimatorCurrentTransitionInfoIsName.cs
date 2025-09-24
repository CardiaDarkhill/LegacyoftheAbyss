using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEB RID: 3563
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Check the active Transition name on a specified layer. Format is 'CURRENT_STATE -> NEXT_STATE'.")]
	public class GetAnimatorCurrentTransitionInfoIsName : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x060066ED RID: 26349 RVA: 0x00208DF7 File Offset: 0x00206FF7
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066EE RID: 26350 RVA: 0x00208DFF File Offset: 0x00206FFF
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameMatch = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
		}

		// Token: 0x060066EF RID: 26351 RVA: 0x00208E31 File Offset: 0x00207031
		public override void OnEnter()
		{
			this.IsName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066F0 RID: 26352 RVA: 0x00208E47 File Offset: 0x00207047
		public override void OnActionUpdate()
		{
			this.IsName();
		}

		// Token: 0x060066F1 RID: 26353 RVA: 0x00208E50 File Offset: 0x00207050
		private void IsName()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsName(this.name.Value))
			{
				this.nameMatch.Value = true;
				base.Fsm.Event(this.nameMatchEvent);
				return;
			}
			this.nameMatch.Value = false;
			base.Fsm.Event(this.nameDoNotMatchEvent);
		}

		// Token: 0x04006646 RID: 26182
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006647 RID: 26183
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x04006648 RID: 26184
		[Tooltip("The name to check the transition against.")]
		public FsmString name;

		// Token: 0x04006649 RID: 26185
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if name matches")]
		public FsmBool nameMatch;

		// Token: 0x0400664A RID: 26186
		[Tooltip("Event send if name matches")]
		public FsmEvent nameMatchEvent;

		// Token: 0x0400664B RID: 26187
		[Tooltip("Event send if name doesn't match")]
		public FsmEvent nameDoNotMatchEvent;
	}
}
