using System;
using GlobalEnums;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200087C RID: 2172
	public class PauseMenuButton : MenuSelectable, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ICancelHandler
	{
		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06004B94 RID: 19348 RVA: 0x001650B9 File Offset: 0x001632B9
		// (set) Token: 0x06004B95 RID: 19349 RVA: 0x001650C1 File Offset: 0x001632C1
		public new CancelAction cancelAction { get; private set; }

		// Token: 0x06004B96 RID: 19350 RVA: 0x001650CA File Offset: 0x001632CA
		protected override void Start()
		{
			base.Start();
			this.gm = GameManager.instance;
			this.ih = this.gm.inputHandler;
			this.ui = UIManager.instance;
			base.HookUpAudioPlayer();
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x00165100 File Offset: 0x00163300
		public void OnSubmit(BaseEventData eventData)
		{
			if (this.pauseButtonType == PauseMenuButton.PauseButtonType.Continue)
			{
				if (this.ih.PauseAllowed)
				{
					this.ui.TogglePauseGame();
					this.flashEffect.ResetTrigger("Flash");
					this.flashEffect.SetTrigger("Flash");
					base.ForceDeselect();
					base.PlaySubmitSound();
					return;
				}
			}
			else
			{
				if (this.pauseButtonType == PauseMenuButton.PauseButtonType.Options)
				{
					this.ui.UIGoToOptionsMenu();
					this.flashEffect.ResetTrigger("Flash");
					this.flashEffect.SetTrigger("Flash");
					base.ForceDeselect();
					base.PlaySubmitSound();
					return;
				}
				if (this.pauseButtonType == PauseMenuButton.PauseButtonType.Quit)
				{
					this.ui.UIShowReturnMenuPrompt();
					this.flashEffect.ResetTrigger("Flash");
					this.flashEffect.SetTrigger("Flash");
					base.ForceDeselect();
					base.PlaySubmitSound();
				}
			}
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x001651E0 File Offset: 0x001633E0
		public new void OnCancel(BaseEventData eventData)
		{
			if (this.ih.PauseAllowed)
			{
				this.ui.TogglePauseGame();
				this.flashEffect.ResetTrigger("Flash");
				this.flashEffect.SetTrigger("Flash");
				base.ForceDeselect();
				base.PlaySubmitSound();
			}
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x00165231 File Offset: 0x00163431
		public void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x04004CF6 RID: 19702
		public Animator flashEffect;

		// Token: 0x04004CF7 RID: 19703
		private GameManager gm;

		// Token: 0x04004CF8 RID: 19704
		private UIManager ui;

		// Token: 0x04004CF9 RID: 19705
		private InputHandler ih;

		// Token: 0x04004CFB RID: 19707
		public PauseMenuButton.PauseButtonType pauseButtonType;

		// Token: 0x02001AE5 RID: 6885
		public enum PauseButtonType
		{
			// Token: 0x04009AD7 RID: 39639
			Continue,
			// Token: 0x04009AD8 RID: 39640
			Options,
			// Token: 0x04009AD9 RID: 39641
			Quit
		}
	}
}
