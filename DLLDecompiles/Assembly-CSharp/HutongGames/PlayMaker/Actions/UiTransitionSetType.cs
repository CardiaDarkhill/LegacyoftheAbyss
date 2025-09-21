using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001138 RID: 4408
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the transition type of a UI Selectable component.")]
	public class UiTransitionSetType : ComponentAction<Selectable>
	{
		// Token: 0x060076BE RID: 30398 RVA: 0x00243516 File Offset: 0x00241716
		public override void Reset()
		{
			this.gameObject = null;
			this.transition = Selectable.Transition.ColorTint;
			this.resetOnExit = false;
		}

		// Token: 0x060076BF RID: 30399 RVA: 0x00243534 File Offset: 0x00241734
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			if (this.selectable != null && this.resetOnExit.Value)
			{
				this.originalTransition = this.selectable.transition;
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x060076C0 RID: 30400 RVA: 0x002435A0 File Offset: 0x002417A0
		private void DoSetValue()
		{
			if (this.selectable != null)
			{
				this.selectable.transition = this.transition;
			}
		}

		// Token: 0x060076C1 RID: 30401 RVA: 0x002435C1 File Offset: 0x002417C1
		public override void OnExit()
		{
			if (this.selectable == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.selectable.transition = this.originalTransition;
			}
		}

		// Token: 0x04007731 RID: 30513
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007732 RID: 30514
		[Tooltip("The transition value")]
		public Selectable.Transition transition;

		// Token: 0x04007733 RID: 30515
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007734 RID: 30516
		private Selectable selectable;

		// Token: 0x04007735 RID: 30517
		private Selectable.Transition originalTransition;
	}
}
