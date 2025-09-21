using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001137 RID: 4407
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the transition type of a UI Selectable component.")]
	public class UiTransitionGetType : ComponentAction<Selectable>
	{
		// Token: 0x060076BA RID: 30394 RVA: 0x002433E1 File Offset: 0x002415E1
		public override void Reset()
		{
			this.gameObject = null;
			this.transition = null;
			this.colorTintEvent = null;
			this.spriteSwapEvent = null;
			this.animationEvent = null;
			this.noTransitionEvent = null;
		}

		// Token: 0x060076BB RID: 30395 RVA: 0x00243410 File Offset: 0x00241610
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x060076BC RID: 30396 RVA: 0x00243450 File Offset: 0x00241650
		private void DoGetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			this.transition.Value = this.selectable.transition.ToString();
			if (this.selectable.transition == Selectable.Transition.None)
			{
				base.Fsm.Event(this.noTransitionEvent);
				return;
			}
			if (this.selectable.transition == Selectable.Transition.ColorTint)
			{
				base.Fsm.Event(this.colorTintEvent);
				return;
			}
			if (this.selectable.transition == Selectable.Transition.SpriteSwap)
			{
				base.Fsm.Event(this.spriteSwapEvent);
				return;
			}
			if (this.selectable.transition == Selectable.Transition.Animation)
			{
				base.Fsm.Event(this.animationEvent);
			}
		}

		// Token: 0x0400772A RID: 30506
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400772B RID: 30507
		[Tooltip("The transition value")]
		public FsmString transition;

		// Token: 0x0400772C RID: 30508
		[Tooltip("Event sent if transition is ColorTint")]
		public FsmEvent colorTintEvent;

		// Token: 0x0400772D RID: 30509
		[Tooltip("Event sent if transition is SpriteSwap")]
		public FsmEvent spriteSwapEvent;

		// Token: 0x0400772E RID: 30510
		[Tooltip("Event sent if transition is Animation")]
		public FsmEvent animationEvent;

		// Token: 0x0400772F RID: 30511
		[Tooltip("Event sent if transition is none")]
		public FsmEvent noTransitionEvent;

		// Token: 0x04007730 RID: 30512
		private Selectable selectable;
	}
}
