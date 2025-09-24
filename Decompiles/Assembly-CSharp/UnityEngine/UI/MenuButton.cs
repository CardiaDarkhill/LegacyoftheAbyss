using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200086F RID: 2159
	public class MenuButton : MenuSelectable, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
	{
		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06004AEC RID: 19180 RVA: 0x00162D19 File Offset: 0x00160F19
		// (set) Token: 0x06004AED RID: 19181 RVA: 0x00162D21 File Offset: 0x00160F21
		public bool SkipNextFlashEffect { get; set; }

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06004AEE RID: 19182 RVA: 0x00162D2A File Offset: 0x00160F2A
		// (set) Token: 0x06004AEF RID: 19183 RVA: 0x00162D32 File Offset: 0x00160F32
		public bool SkipNextSubmitSound { get; set; }

		// Token: 0x06004AF0 RID: 19184 RVA: 0x00162D3B File Offset: 0x00160F3B
		private new void Start()
		{
			base.HookUpAudioPlayer();
			base.HookUpEventTrigger();
		}

		// Token: 0x06004AF1 RID: 19185 RVA: 0x00162D4C File Offset: 0x00160F4C
		public void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			if (this.buttonType != MenuButton.MenuButtonType.Activate)
			{
				base.ForceDeselect();
			}
			if (!this.SkipNextFlashEffect && this.flashEffect)
			{
				this.flashEffect.ResetTrigger(MenuButton._flashProp);
				this.flashEffect.SetTrigger(MenuButton._flashProp);
			}
			else
			{
				Debug.LogWarning("MenuButton missing flashEffect!", this);
			}
			this.SkipNextFlashEffect = false;
			if (this.SkipNextSubmitSound)
			{
				this.SkipNextSubmitSound = false;
			}
			else
			{
				base.PlaySubmitSound();
			}
			UnityEvent onSubmitPressed = this.OnSubmitPressed;
			if (onSubmitPressed == null)
			{
				return;
			}
			onSubmitPressed.Invoke();
		}

		// Token: 0x06004AF2 RID: 19186 RVA: 0x00162DE1 File Offset: 0x00160FE1
		public void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x06004AF3 RID: 19187 RVA: 0x00162DEA File Offset: 0x00160FEA
		public void SetNavigationOverride(Navigation nav)
		{
			if (!this.hasNavOverride)
			{
				this.originalNavigation = base.navigation;
				this.hasNavOverride = true;
			}
			base.navigation = nav;
		}

		// Token: 0x06004AF4 RID: 19188 RVA: 0x00162E0E File Offset: 0x0016100E
		public void RestoreOriginalNavigation()
		{
			if (this.hasNavOverride)
			{
				this.hasNavOverride = false;
				base.navigation = this.originalNavigation;
			}
		}

		// Token: 0x04004C95 RID: 19605
		public MenuButton.MenuButtonType buttonType;

		// Token: 0x04004C96 RID: 19606
		public Animator flashEffect;

		// Token: 0x04004C97 RID: 19607
		private static readonly int _flashProp = Animator.StringToHash("Flash");

		// Token: 0x04004C9A RID: 19610
		private Navigation originalNavigation;

		// Token: 0x04004C9B RID: 19611
		private bool hasNavOverride;

		// Token: 0x04004C9C RID: 19612
		public UnityEvent OnSubmitPressed;

		// Token: 0x02001AD6 RID: 6870
		public enum MenuButtonType
		{
			// Token: 0x04009AA2 RID: 39586
			Proceed,
			// Token: 0x04009AA3 RID: 39587
			Activate
		}
	}
}
