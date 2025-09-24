using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001135 RID: 4405
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the Color Block of a UI Selectable component. Modifications will not be visible if transition is not ColorTint")]
	public class UiSetColorBlock : ComponentAction<Selectable>
	{
		// Token: 0x060076AF RID: 30383 RVA: 0x002430A8 File Offset: 0x002412A8
		public override void Reset()
		{
			this.gameObject = null;
			this.fadeDuration = new FsmFloat
			{
				UseVariable = true
			};
			this.colorMultiplier = new FsmFloat
			{
				UseVariable = true
			};
			this.normalColor = new FsmColor
			{
				UseVariable = true
			};
			this.highlightedColor = new FsmColor
			{
				UseVariable = true
			};
			this.pressedColor = new FsmColor
			{
				UseVariable = true
			};
			this.disabledColor = new FsmColor
			{
				UseVariable = true
			};
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060076B0 RID: 30384 RVA: 0x00243138 File Offset: 0x00241338
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			if (this.selectable != null && this.resetOnExit.Value)
			{
				this.originalColorBlock = this.selectable.colors;
			}
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076B1 RID: 30385 RVA: 0x002431AC File Offset: 0x002413AC
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060076B2 RID: 30386 RVA: 0x002431B4 File Offset: 0x002413B4
		private void DoSetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			this._colorBlock = this.selectable.colors;
			if (!this.colorMultiplier.IsNone)
			{
				this._colorBlock.colorMultiplier = this.colorMultiplier.Value;
			}
			if (!this.fadeDuration.IsNone)
			{
				this._colorBlock.fadeDuration = this.fadeDuration.Value;
			}
			if (!this.normalColor.IsNone)
			{
				this._colorBlock.normalColor = this.normalColor.Value;
			}
			if (!this.pressedColor.IsNone)
			{
				this._colorBlock.pressedColor = this.pressedColor.Value;
			}
			if (!this.highlightedColor.IsNone)
			{
				this._colorBlock.highlightedColor = this.highlightedColor.Value;
			}
			if (!this.disabledColor.IsNone)
			{
				this._colorBlock.disabledColor = this.disabledColor.Value;
			}
			this.selectable.colors = this._colorBlock;
		}

		// Token: 0x060076B3 RID: 30387 RVA: 0x002432C4 File Offset: 0x002414C4
		public override void OnExit()
		{
			if (this.selectable == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.selectable.colors = this.originalColorBlock;
			}
		}

		// Token: 0x04007719 RID: 30489
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400771A RID: 30490
		[Tooltip("The fade duration value. Leave as None for no effect")]
		public FsmFloat fadeDuration;

		// Token: 0x0400771B RID: 30491
		[Tooltip("The color multiplier value. Leave as None for no effect")]
		public FsmFloat colorMultiplier;

		// Token: 0x0400771C RID: 30492
		[Tooltip("The normal color value. Leave as None for no effect")]
		public FsmColor normalColor;

		// Token: 0x0400771D RID: 30493
		[Tooltip("The pressed color value. Leave as None for no effect")]
		public FsmColor pressedColor;

		// Token: 0x0400771E RID: 30494
		[Tooltip("The highlighted color value. Leave as None for no effect")]
		public FsmColor highlightedColor;

		// Token: 0x0400771F RID: 30495
		[Tooltip("The disabled color value. Leave as None for no effect")]
		public FsmColor disabledColor;

		// Token: 0x04007720 RID: 30496
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007721 RID: 30497
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x04007722 RID: 30498
		private Selectable selectable;

		// Token: 0x04007723 RID: 30499
		private ColorBlock _colorBlock;

		// Token: 0x04007724 RID: 30500
		private ColorBlock originalColorBlock;
	}
}
