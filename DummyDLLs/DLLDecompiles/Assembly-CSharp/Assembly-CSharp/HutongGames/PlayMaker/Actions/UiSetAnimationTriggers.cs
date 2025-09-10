using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001134 RID: 4404
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the Animation Triggers of a UI Selectable component. Modifications will not be visible if transition is not Animation")]
	public class UiSetAnimationTriggers : ComponentAction<Selectable>
	{
		// Token: 0x060076AA RID: 30378 RVA: 0x00242ED4 File Offset: 0x002410D4
		public override void Reset()
		{
			this.gameObject = null;
			this.normalTrigger = new FsmString
			{
				UseVariable = true
			};
			this.highlightedTrigger = new FsmString
			{
				UseVariable = true
			};
			this.pressedTrigger = new FsmString
			{
				UseVariable = true
			};
			this.disabledTrigger = new FsmString
			{
				UseVariable = true
			};
			this.resetOnExit = null;
		}

		// Token: 0x060076AB RID: 30379 RVA: 0x00242F38 File Offset: 0x00241138
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			if (this.selectable != null && this.resetOnExit.Value)
			{
				this.originalAnimationTriggers = this.selectable.animationTriggers;
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x060076AC RID: 30380 RVA: 0x00242FA4 File Offset: 0x002411A4
		private void DoSetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			this._animationTriggers = this.selectable.animationTriggers;
			if (!this.normalTrigger.IsNone)
			{
				this._animationTriggers.normalTrigger = this.normalTrigger.Value;
			}
			if (!this.highlightedTrigger.IsNone)
			{
				this._animationTriggers.highlightedTrigger = this.highlightedTrigger.Value;
			}
			if (!this.pressedTrigger.IsNone)
			{
				this._animationTriggers.pressedTrigger = this.pressedTrigger.Value;
			}
			if (!this.disabledTrigger.IsNone)
			{
				this._animationTriggers.disabledTrigger = this.disabledTrigger.Value;
			}
			this.selectable.animationTriggers = this._animationTriggers;
		}

		// Token: 0x060076AD RID: 30381 RVA: 0x0024306E File Offset: 0x0024126E
		public override void OnExit()
		{
			if (this.selectable == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.selectable.animationTriggers = this.originalAnimationTriggers;
			}
		}

		// Token: 0x04007710 RID: 30480
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007711 RID: 30481
		[Tooltip("The normal trigger value. Leave as None for no effect")]
		public FsmString normalTrigger;

		// Token: 0x04007712 RID: 30482
		[Tooltip("The highlighted trigger value. Leave as None for no effect")]
		public FsmString highlightedTrigger;

		// Token: 0x04007713 RID: 30483
		[Tooltip("The pressed trigger value. Leave as None for no effect")]
		public FsmString pressedTrigger;

		// Token: 0x04007714 RID: 30484
		[Tooltip("The disabled trigger value. Leave as None for no effect")]
		public FsmString disabledTrigger;

		// Token: 0x04007715 RID: 30485
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007716 RID: 30486
		private Selectable selectable;

		// Token: 0x04007717 RID: 30487
		private AnimationTriggers _animationTriggers;

		// Token: 0x04007718 RID: 30488
		private AnimationTriggers originalAnimationTriggers;
	}
}
