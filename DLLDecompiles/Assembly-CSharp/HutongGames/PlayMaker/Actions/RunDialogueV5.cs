using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001362 RID: 4962
	[ActionCategory("Dialogue")]
	public class RunDialogueV5 : RunDialogueBase
	{
		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x06007FFD RID: 32765 RVA: 0x0025D01C File Offset: 0x0025B21C
		protected override string DialogueText
		{
			get
			{
				if (this.UsesCustomText())
				{
					return this.CustomText.Value;
				}
				if (!CheatManager.IsDialogueDebugEnabled)
				{
					return new LocalisedString(this.Sheet.Value, this.Key.Value).ToString(false);
				}
				return this.Sheet.Value + " / " + this.Key.Value;
			}
		}

		// Token: 0x06007FFE RID: 32766 RVA: 0x0025D089 File Offset: 0x0025B289
		public bool UsesCustomText()
		{
			return !this.CustomText.IsNone;
		}

		// Token: 0x06007FFF RID: 32767 RVA: 0x0025D09C File Offset: 0x0025B29C
		public override void Reset()
		{
			base.Reset();
			this.CustomText = new FsmString
			{
				UseVariable = true
			};
			this.Sheet = null;
			this.Key = null;
			this.TextColor = new FsmColor
			{
				UseVariable = true
			};
			this.NpcVoiceTableOverride = null;
			this.StopOffsetY = null;
		}

		// Token: 0x06008000 RID: 32768 RVA: 0x0025D0F0 File Offset: 0x0025B2F0
		protected override DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			if (!this.TextColor.IsNone)
			{
				defaultOptions.TextColor = this.TextColor.Value;
			}
			if (!this.StopOffsetY.IsNone)
			{
				defaultOptions.StopOffsetY = this.StopOffsetY.Value;
			}
			return defaultOptions;
		}

		// Token: 0x06008001 RID: 32769 RVA: 0x0025D13C File Offset: 0x0025B33C
		protected override void StartDialogue(PlayMakerNPC component)
		{
			RandomAudioClipTable randomAudioClipTable = this.NpcVoiceTableOverride.Value as RandomAudioClipTable;
			if (!this.NpcVoiceTableOverride.IsNone && randomAudioClipTable)
			{
				this.speakingAudio = component.GetComponent<NPCSpeakingAudio>();
				if (!this.speakingAudio)
				{
					this.speakingAudio = component.gameObject.AddComponent<NPCSpeakingAudio>();
				}
				if (this.speakingAudio)
				{
					this.previousTable = this.speakingAudio.Table;
					this.speakingAudio.Table = randomAudioClipTable;
				}
			}
			base.StartDialogue(component);
		}

		// Token: 0x06008002 RID: 32770 RVA: 0x0025D1CA File Offset: 0x0025B3CA
		public override void OnExit()
		{
			base.OnExit();
			if (this.speakingAudio)
			{
				this.speakingAudio.Table = this.previousTable;
				this.speakingAudio = null;
				this.previousTable = null;
			}
		}

		// Token: 0x04007F68 RID: 32616
		public FsmString CustomText;

		// Token: 0x04007F69 RID: 32617
		[HideIf("UsesCustomText")]
		public FsmString Sheet;

		// Token: 0x04007F6A RID: 32618
		[HideIf("UsesCustomText")]
		public FsmString Key;

		// Token: 0x04007F6B RID: 32619
		public FsmColor TextColor;

		// Token: 0x04007F6C RID: 32620
		public FsmFloat StopOffsetY;

		// Token: 0x04007F6D RID: 32621
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject NpcVoiceTableOverride;

		// Token: 0x04007F6E RID: 32622
		private NPCSpeakingAudio speakingAudio;

		// Token: 0x04007F6F RID: 32623
		private RandomAudioClipTable previousTable;
	}
}
