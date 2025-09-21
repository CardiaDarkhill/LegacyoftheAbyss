using System;
using GlobalEnums;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001341 RID: 4929
	public class ShowControlReminderSingleGroup : FsmStateAction
	{
		// Token: 0x06007F68 RID: 32616 RVA: 0x0025B4B0 File Offset: 0x002596B0
		public override void Reset()
		{
			this.DisappearEvent = null;
			this.PlayerDataBool = null;
			this.FadeInDelay = null;
			this.FadeInTime = 1f;
			this.FadeOutTime = 0.5f;
			this.Groups = null;
			this.DisappearOnButtonPress = null;
		}

		// Token: 0x06007F69 RID: 32617 RVA: 0x0025B500 File Offset: 0x00259700
		public override void OnEnter()
		{
			foreach (ShowControlReminderSingleGroup.Group group in this.Groups)
			{
				ControlReminder.PushSingle(new ControlReminder.SingleConfig
				{
					DisappearEvent = this.DisappearEvent.Value,
					PlayerDataBool = this.PlayerDataBool.Value,
					FadeInDelay = this.FadeInDelay.Value,
					FadeInTime = this.FadeInTime.Value,
					FadeOutTime = this.FadeOutTime.Value,
					Text = group.Text,
					Prompt = group.Prompt,
					Button = (HeroActionButton)group.Button.Value,
					DisappearOnButtonPress = this.DisappearOnButtonPress.Value
				});
			}
			ControlReminder.ShowPushed();
			base.Finish();
		}

		// Token: 0x04007EF1 RID: 32497
		public FsmString DisappearEvent;

		// Token: 0x04007EF2 RID: 32498
		public FsmString PlayerDataBool;

		// Token: 0x04007EF3 RID: 32499
		public FsmFloat FadeInDelay;

		// Token: 0x04007EF4 RID: 32500
		public FsmFloat FadeInTime;

		// Token: 0x04007EF5 RID: 32501
		public FsmFloat FadeOutTime;

		// Token: 0x04007EF6 RID: 32502
		public ShowControlReminderSingleGroup.Group[] Groups;

		// Token: 0x04007EF7 RID: 32503
		public FsmBool DisappearOnButtonPress;

		// Token: 0x02001BF5 RID: 7157
		[Serializable]
		public class Group
		{
			// Token: 0x04009FAE RID: 40878
			public LocalisedFsmString Text;

			// Token: 0x04009FAF RID: 40879
			public LocalisedFsmString Prompt;

			// Token: 0x04009FB0 RID: 40880
			[ObjectType(typeof(HeroActionButton))]
			public FsmEnum Button;
		}
	}
}
