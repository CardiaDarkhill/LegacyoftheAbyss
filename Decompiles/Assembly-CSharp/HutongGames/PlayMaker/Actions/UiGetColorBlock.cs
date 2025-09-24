using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001130 RID: 4400
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the Color Block of a UI Selectable component.")]
	public class UiGetColorBlock : ComponentAction<Selectable>
	{
		// Token: 0x06007698 RID: 30360 RVA: 0x002429C4 File Offset: 0x00240BC4
		public override void Reset()
		{
			this.gameObject = null;
			this.fadeDuration = null;
			this.colorMultiplier = null;
			this.normalColor = null;
			this.highlightedColor = null;
			this.pressedColor = null;
			this.disabledColor = null;
			this.everyFrame = false;
		}

		// Token: 0x06007699 RID: 30361 RVA: 0x00242A00 File Offset: 0x00240C00
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600769A RID: 30362 RVA: 0x00242A48 File Offset: 0x00240C48
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x0600769B RID: 30363 RVA: 0x00242A50 File Offset: 0x00240C50
		private void DoGetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			if (!this.colorMultiplier.IsNone)
			{
				this.colorMultiplier.Value = this.selectable.colors.colorMultiplier;
			}
			if (!this.fadeDuration.IsNone)
			{
				this.fadeDuration.Value = this.selectable.colors.fadeDuration;
			}
			if (!this.normalColor.IsNone)
			{
				this.normalColor.Value = this.selectable.colors.normalColor;
			}
			if (!this.pressedColor.IsNone)
			{
				this.pressedColor.Value = this.selectable.colors.pressedColor;
			}
			if (!this.highlightedColor.IsNone)
			{
				this.highlightedColor.Value = this.selectable.colors.highlightedColor;
			}
			if (!this.disabledColor.IsNone)
			{
				this.disabledColor.Value = this.selectable.colors.disabledColor;
			}
		}

		// Token: 0x040076F4 RID: 30452
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040076F5 RID: 30453
		[Tooltip("The fade duration value. Leave as None for no effect")]
		[UIHint(UIHint.Variable)]
		public FsmFloat fadeDuration;

		// Token: 0x040076F6 RID: 30454
		[Tooltip("The color multiplier value. Leave as None for no effect")]
		[UIHint(UIHint.Variable)]
		public FsmFloat colorMultiplier;

		// Token: 0x040076F7 RID: 30455
		[Tooltip("The normal color value. Leave as None for no effect")]
		[UIHint(UIHint.Variable)]
		public FsmColor normalColor;

		// Token: 0x040076F8 RID: 30456
		[Tooltip("The pressed color value. Leave as None for no effect")]
		[UIHint(UIHint.Variable)]
		public FsmColor pressedColor;

		// Token: 0x040076F9 RID: 30457
		[Tooltip("The highlighted color value. Leave as None for no effect")]
		[UIHint(UIHint.Variable)]
		public FsmColor highlightedColor;

		// Token: 0x040076FA RID: 30458
		[Tooltip("The disabled color value. Leave as None for no effect")]
		[UIHint(UIHint.Variable)]
		public FsmColor disabledColor;

		// Token: 0x040076FB RID: 30459
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x040076FC RID: 30460
		private Selectable selectable;
	}
}
