using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001361 RID: 4961
	[ActionCategory("Dialogue")]
	public class RunDialogueV4 : RunDialogueBase
	{
		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x06007FF6 RID: 32758 RVA: 0x0025CE60 File Offset: 0x0025B060
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

		// Token: 0x06007FF7 RID: 32759 RVA: 0x0025CECD File Offset: 0x0025B0CD
		public bool UsesCustomText()
		{
			return !this.CustomText.IsNone;
		}

		// Token: 0x06007FF8 RID: 32760 RVA: 0x0025CEE0 File Offset: 0x0025B0E0
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
		}

		// Token: 0x06007FF9 RID: 32761 RVA: 0x0025CF2C File Offset: 0x0025B12C
		protected override DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			if (!this.TextColor.IsNone)
			{
				defaultOptions.TextColor = this.TextColor.Value;
			}
			return defaultOptions;
		}

		// Token: 0x06007FFA RID: 32762 RVA: 0x0025CF50 File Offset: 0x0025B150
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

		// Token: 0x06007FFB RID: 32763 RVA: 0x0025CFDE File Offset: 0x0025B1DE
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

		// Token: 0x04007F61 RID: 32609
		public FsmString CustomText;

		// Token: 0x04007F62 RID: 32610
		[HideIf("UsesCustomText")]
		public FsmString Sheet;

		// Token: 0x04007F63 RID: 32611
		[HideIf("UsesCustomText")]
		public FsmString Key;

		// Token: 0x04007F64 RID: 32612
		public FsmColor TextColor;

		// Token: 0x04007F65 RID: 32613
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject NpcVoiceTableOverride;

		// Token: 0x04007F66 RID: 32614
		private NPCSpeakingAudio speakingAudio;

		// Token: 0x04007F67 RID: 32615
		private RandomAudioClipTable previousTable;
	}
}
