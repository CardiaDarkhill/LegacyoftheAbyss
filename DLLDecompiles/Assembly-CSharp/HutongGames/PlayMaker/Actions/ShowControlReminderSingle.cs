using System;
using GlobalEnums;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001340 RID: 4928
	public class ShowControlReminderSingle : FsmStateAction
	{
		// Token: 0x06007F65 RID: 32613 RVA: 0x0025B364 File Offset: 0x00259564
		public override void Reset()
		{
			this.AppearEvent = null;
			this.DisappearEvent = null;
			this.PlayerDataBool = null;
			this.FadeInDelay = null;
			this.FadeInTime = 1f;
			this.FadeOutTime = 0.5f;
			this.Text = null;
			this.Prompt = null;
			this.Button = null;
			this.DisappearOnButtonPress = null;
		}

		// Token: 0x06007F66 RID: 32614 RVA: 0x0025B3CC File Offset: 0x002595CC
		public override void OnEnter()
		{
			ControlReminder.AddReminder(new ControlReminder.SingleConfig
			{
				AppearEvent = this.AppearEvent.Value,
				DisappearEvent = this.DisappearEvent.Value,
				PlayerDataBool = this.PlayerDataBool.Value,
				FadeInDelay = this.FadeInDelay.Value,
				FadeInTime = this.FadeInTime.Value,
				FadeOutTime = this.FadeOutTime.Value,
				Text = this.Text,
				Prompt = this.Prompt,
				Button = (HeroActionButton)this.Button.Value,
				DisappearOnButtonPress = this.DisappearOnButtonPress.Value
			}, string.IsNullOrEmpty(this.AppearEvent.Value));
			base.Finish();
		}

		// Token: 0x04007EE7 RID: 32487
		public FsmString AppearEvent;

		// Token: 0x04007EE8 RID: 32488
		public FsmString DisappearEvent;

		// Token: 0x04007EE9 RID: 32489
		public FsmString PlayerDataBool;

		// Token: 0x04007EEA RID: 32490
		public FsmFloat FadeInDelay;

		// Token: 0x04007EEB RID: 32491
		public FsmFloat FadeInTime;

		// Token: 0x04007EEC RID: 32492
		public FsmFloat FadeOutTime;

		// Token: 0x04007EED RID: 32493
		public LocalisedFsmString Text;

		// Token: 0x04007EEE RID: 32494
		public LocalisedFsmString Prompt;

		// Token: 0x04007EEF RID: 32495
		[ObjectType(typeof(HeroActionButton))]
		public FsmEnum Button;

		// Token: 0x04007EF0 RID: 32496
		public FsmBool DisappearOnButtonPress;
	}
}
