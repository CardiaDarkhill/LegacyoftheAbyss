using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0A RID: 3338
	public class ScreenFaderPreset : FsmStateAction
	{
		// Token: 0x060062BB RID: 25275 RVA: 0x001F3620 File Offset: 0x001F1820
		public override void Reset()
		{
			this.Preset = null;
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x001F362C File Offset: 0x001F182C
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			PlayMakerFSM screenFader_fsm = instance.screenFader_fsm;
			ScreenFaderPreset.PresetEvents presetEvents = (ScreenFaderPreset.PresetEvents)this.Preset.Value;
			if (presetEvents != ScreenFaderPreset.PresetEvents.FadeOut)
			{
				if (presetEvents == ScreenFaderPreset.PresetEvents.FadeIn)
				{
					screenFader_fsm.SendEvent("SCENE FADE IN");
				}
			}
			else
			{
				screenFader_fsm.SendEvent("SCENE FADE OUT");
			}
			base.Finish();
		}

		// Token: 0x04006128 RID: 24872
		[RequiredField]
		[ObjectType(typeof(ScreenFaderPreset.PresetEvents))]
		public FsmEnum Preset;

		// Token: 0x02001B88 RID: 7048
		public enum PresetEvents
		{
			// Token: 0x04009D82 RID: 40322
			FadeOut,
			// Token: 0x04009D83 RID: 40323
			FadeIn
		}
	}
}
